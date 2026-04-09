using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Entities;
using Xunit;

namespace StudentManagement.Tests.Services;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepo;
    private readonly StudentService _sut; 

    public StudentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepo = new Mock<IStudentRepository>();

        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepo.Object);

        _sut = new StudentService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetStudentByIdAsync_ShouldReturnSuccess_WhenStudentExists()
    {
        // Arrange
        var studentId = 1;
        var student = new Student 
        { 
            Id = studentId, 
            Name = "John Doe", 
            Email = "john@test.com", 
            IsDeleted = false 
        };
        
        _mockStudentRepo.Setup(r => r.GetByIdAsync(studentId))
            .ReturnsAsync(student);

       
        var result = await _sut.GetStudentByIdAsync(studentId);

    
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("John Doe");
        _mockStudentRepo.Verify(r => r.GetByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task GetStudentByIdAsync_ShouldReturnFail_WhenStudentDoesNotExist()
    {
       
        var studentId = 99;
        _mockStudentRepo.Setup(r => r.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);


        var result = await _sut.GetStudentByIdAsync(studentId);

   
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetAllStudentsAsync_ShouldReturnOnlyActiveStudents()
    {
       
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "Active", IsDeleted = false },
            new Student { Id = 2, Name = "Deleted", IsDeleted = true }
        };

        _mockStudentRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(students);

        
        var result = await _sut.GetAllStudentsAsync();

        
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("Active");
    }
}
