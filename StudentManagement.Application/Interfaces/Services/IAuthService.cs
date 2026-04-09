using System.Threading.Tasks;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;

namespace StudentManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<bool>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<bool>> LogoutAsync(string refreshToken);
}
