using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Application.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<IEnumerable<RefreshToken>> FindRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
