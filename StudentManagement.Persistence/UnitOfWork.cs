using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Interface;
using StudentManagement.Persistence.Context;
using StudentManagement.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentManagementDbContext _context;

        public IStudentRepository Students { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(StudentManagementDbContext context)
        {
            _context = context;
            Students = new StudentRepository(context);
            Users = new UserRepository(context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
