using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;

namespace StudentManagement.Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }
}
