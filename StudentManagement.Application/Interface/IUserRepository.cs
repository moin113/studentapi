using StudentManagement.Application.Interface;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<IEnumerable<RefreshToken>> FindRefreshTokenAsync(string token);
    Task RevokeAllUserTokensAsync(int userId);
}