using FluentValidation.TestHelper;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.Validators;
using Xunit;

namespace StudentManagement.Tests.Validators;

public class CreateStudentValidatorTests
{
    private readonly CreateStudentValidator _validator;

    public CreateStudentValidatorTests()
    {
        _validator = new CreateStudentValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new CreateStudentRequest { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new CreateStudentRequest { Email = "invalid-email" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Age_Is_Out_Of_Range()
    {
        var model = new CreateStudentRequest { Age = 150 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new CreateStudentRequest 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Age = 25, 
            Course = "Computer Science" 
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
