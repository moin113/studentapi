using Microsoft.Extensions.Logging;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Persistence.Context;
using StudentManagement.Persistence.Repositories;
using System.Threading.Tasks;

namespace StudentManagement.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly StudentManagementDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public IStudentRepository Students { get; private set; }
    public IUserRepository Users { get; private set; }

    public UnitOfWork(StudentManagementDbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        Students = new StudentRepository(context, loggerFactory.CreateLogger<StudentRepository>());
        Users = new UserRepository(context, loggerFactory.CreateLogger<UserRepository>());
    }

    public async Task<int> SaveChangesAsync()
    {
        _logger.LogInformation("UnitOfWork: Saving changes to database.");
        var result = await _context.SaveChangesAsync();
        _logger.LogInformation("UnitOfWork: Successfully saved {Count} changes.", result);
        return result;
    }

    public void Dispose()
    {
        _context.Dispose();
        _logger.LogInformation("UnitOfWork: Disposed database context.");
    }
}