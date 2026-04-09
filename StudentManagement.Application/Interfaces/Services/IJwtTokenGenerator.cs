using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
