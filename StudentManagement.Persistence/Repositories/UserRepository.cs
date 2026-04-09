using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

    public async Task AddRefreshTokenAsync(RefreshToken token)
        => await _context.RefreshTokens.AddAsync(token);

    public async Task<IEnumerable<RefreshToken>> FindRefreshTokenAsync(string token)
        => await _context.RefreshTokens
            .Where(t => t.Token == token)
            .ToListAsync();

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }
    }
}