using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    private readonly ILogger<StudentRepository> _logger;

    public StudentRepository(StudentManagementDbContext context, ILogger<StudentRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("Repository: Finding student by email {Email}", email);
        return await _dbSet.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
    }

    public async Task<IEnumerable<Student>> GetByCourseAsync(string course)
    {
        _logger.LogInformation("Repository: Finding students by course {Course}", course);
        return await _dbSet
            .Where(s => s.Course == course && !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
    {
        _logger.LogInformation("Repository: Fetching active students.");
        return await _dbSet
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.CreatedDate)
            .ToListAsync();
    }
}