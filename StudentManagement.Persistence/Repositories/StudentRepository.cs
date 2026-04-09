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
    

    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(StudentManagementDbContext context) : base(context) { }

        public async Task<Student?> GetByEmailAsync(string email)
            => await _dbSet
                   .AsNoTracking()
                   .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

        public async Task<IEnumerable<Student>> GetByCourseAsync(string course)
            => await _dbSet
                   .AsNoTracking()
                   .Where(s => s.Course.ToLower() == course.ToLower())
                   .ToListAsync();
    }
}
