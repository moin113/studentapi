using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;
using StudentManagement.Persistence.Context;

namespace StudentManagement.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(string email)
        => await _dbSet
            .FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);

    public async Task<IEnumerable<Student>> GetByCourseAsync(string course)
        => await _dbSet
            .Where(s => s.Course == course && !s.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
        => await _dbSet
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.CreatedDate)
            .ToListAsync();
}