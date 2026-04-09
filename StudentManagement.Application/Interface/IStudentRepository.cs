using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Application.Interface
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByEmailAsync(string email);
        Task<IEnumerable<Student>> GetByCourseAsync(string course);
    }
}
