using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Persistence.Context;
using StudentManagement.Persistence.Repositories;

namespace StudentManagement.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly StudentManagementDbContext _context;

    public IStudentRepository Students { get; private set; }
    public IUserRepository Users { get; private set; }

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