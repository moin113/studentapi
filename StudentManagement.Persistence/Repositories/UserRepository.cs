using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Application.Interface;
using StudentManagement.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(StudentManagementDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
            => await _dbSet
                   .AsNoTracking()
                   .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet
                   .AsNoTracking()
                   .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task AddRefreshTokenAsync(RefreshToken token)
            => await _context.Set<RefreshToken>().AddAsync(token);

        public async Task<IEnumerable<RefreshToken>> FindRefreshTokenAsync(string token)
            => await _context.Set<RefreshToken>()
                   .AsNoTracking()
                   .Where(t => t.Token == token)
                   .ToListAsync();

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var tokens = await _context.Set<RefreshToken>()
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                _context.Set<RefreshToken>().Update(token);
            }
        }
    }
}
