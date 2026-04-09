using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}
