using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(StudentManagementDbContext context, ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("Repository: Finding user by email {Email}", email);
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _logger.LogInformation("Repository: Adding new refresh token for UserId {UserId}", token.UserId);
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task<IEnumerable<RefreshToken>> FindRefreshTokenAsync(string token)
    {
        _logger.LogInformation("Repository: Searching for refresh token.");
        return await _context.RefreshTokens
            .Where(t => t.Token == token)
            .ToListAsync();
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        _logger.LogInformation("Repository: Revoking all tokens for UserId {UserId}", userId);
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