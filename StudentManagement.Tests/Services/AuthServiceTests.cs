using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.Interface;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Entities;
using Xunit;

namespace StudentManagement.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockJwtGenerator = new Mock<IJwtTokenGenerator>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _sut = new AuthService(_mockUnitOfWork.Object, _mockJwtGenerator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenUserNotFound()
    {
        var request = new LoginRequest { Email = "notfound@test.com", Password = "password" };
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(new List<User>()); // Return empty list

    
        var result = await _sut.LoginAsync(request);

       
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        
        var password = "CorrectPassword123";
        var user = new User 
        { 
            Id = 1, 
            Email = "admin@test.com", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true,
            Role = "Admin",
            FullName = "Admin User"
        };

        var request = new LoginRequest { Email = "admin@test.com", Password = password };

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        
        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(user)).Returns("fake-jwt-token");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("fake-refresh-token");

     
        var result = await _sut.LoginAsync(request);

        
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("fake-jwt-token");
    }
}
