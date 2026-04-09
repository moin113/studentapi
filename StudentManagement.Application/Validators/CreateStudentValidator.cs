using FluentValidation;
using StudentManagement.Application.DTOs.Request;

namespace StudentManagement.Application.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Age)
            .InclusiveBetween(1, 100).WithMessage("Age must be between 1 and 100.");

        RuleFor(x => x.Course)
            .NotEmpty().WithMessage("Course is required.")
            .MaximumLength(100).WithMessage("Course cannot exceed 100 characters.");
    }
}
