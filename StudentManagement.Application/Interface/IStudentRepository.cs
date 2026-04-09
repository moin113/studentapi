using StudentManagement.Application.Interface;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Repositories;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetByCourseAsync(string course);
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
}