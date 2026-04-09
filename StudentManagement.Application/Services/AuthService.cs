using System;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Interface;
using StudentManagement.Domain.Entities;
using BCrypt.Net;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email && u.IsActive);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.FailResult("Invalid email or password.");

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

        if (_unitOfWork.Users is IUserRepository userRepo)
        {
             await userRepo.AddRefreshTokenAsync(tokenEntity);
        }
        await _unitOfWork.SaveChangesAsync();

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
        if (_unitOfWork.Users is not IUserRepository userRepoCheck)
             return ApiResponse<AuthResponse>.FailResult("Internal Configuration Error.");
             
        var tokens = await userRepoCheck.FindRefreshTokenAsync(request.RefreshToken);
        var token = tokens.FirstOrDefault();

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            return ApiResponse<AuthResponse>.FailResult("Invalid or expired refresh token.");

        var users = await _unitOfWork.Users.FindAsync(u => u.Id == token.UserId);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
            return ApiResponse<AuthResponse>.FailResult("User not found or inactive.");

        // Revoke old token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        // Issue new tokens
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await userRepoCheck.AddRefreshTokenAsync(newTokenEntity);
        await _unitOfWork.SaveChangesAsync();

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
        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
        if (existing.Any())
            return ApiResponse<bool>.FailResult("Email already registered.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            Username = request.Email // Defaulting because Domain has Username
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "User registered successfully.");
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
    {
        if (_unitOfWork.Users is not IUserRepository userRepoCheck)
             return ApiResponse<bool>.FailResult("Internal Configuration Error.");

        var tokens = await userRepoCheck.FindRefreshTokenAsync(refreshToken);
        var token = tokens.FirstOrDefault();

        if (token == null)
            return ApiResponse<bool>.FailResult("Refresh token not found.");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Logged out successfully.");
    }
}
