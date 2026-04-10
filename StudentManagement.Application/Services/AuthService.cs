using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("AuthService: Attempting login for {Email}", request.Email);
        var users = await _unitOfWork.Users.FindAsync(
            u => u.Email == request.Email && u.IsActive);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("AuthService: Invalid login attempt for {Email}", request.Email);
            return ApiResponse<AuthResponse>.FailResult("Invalid email or password.");
        }

        if (user.Role != "Admin")
        {
            _logger.LogWarning("AuthService: Access denied for non-admin user {Email}", request.Email);
            return ApiResponse<AuthResponse>.FailResult("Access denied. Only administrators can log in.");
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var tokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.Users.AddRefreshTokenAsync(tokenEntity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AuthService: User {Email} logged in successfully.", request.Email);
        return ApiResponse<AuthResponse>.SuccessResult(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            Role = user.Role,
            FullName = user.FullName
        });
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        _logger.LogInformation("AuthService: Refreshing token.");
        var tokens = await _unitOfWork.Users.FindRefreshTokenAsync(request.RefreshToken);
        var token = tokens.FirstOrDefault();

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("AuthService: Invalid or expired refresh token used.");
            return ApiResponse<AuthResponse>.FailResult("Invalid or expired refresh token.");
        }

        var users = await _unitOfWork.Users.FindAsync(u => u.Id == token.UserId);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("AuthService: Refresh failed. User {UserId} not found or inactive.", token.UserId);
            return ApiResponse<AuthResponse>.FailResult("User not found or inactive.");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        await _unitOfWork.Users.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        });

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AuthService: Token refreshed successfully for User {UserId}.", user.Id);
        return ApiResponse<AuthResponse>.SuccessResult(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            Role = user.Role,
            FullName = user.FullName
        });
    }

    public async Task<ApiResponse<bool>> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("AuthService: Registering new user {Email}", request.Email);
        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
        if (existing.Any())
        {
            _logger.LogWarning("AuthService: Registration failed. Email {Email} already exists.", request.Email);
            return ApiResponse<bool>.FailResult("Email already registered.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AuthService: User {Email} registered successfully.", request.Email);
        return ApiResponse<bool>.SuccessResult(true, "User registered successfully.");
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
    {
        _logger.LogInformation("AuthService: Logging out.");
        var tokens = await _unitOfWork.Users.FindRefreshTokenAsync(refreshToken);
        var token = tokens.FirstOrDefault();

        if (token == null)
        {
            _logger.LogWarning("AuthService: Logout failed. Refresh token not found.");
            return ApiResponse<bool>.FailResult("Refresh token not found.");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("AuthService: Logged out successfully.");
        return ApiResponse<bool>.SuccessResult(true, "Logged out successfully.");
    }
}