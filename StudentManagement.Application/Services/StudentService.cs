using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Interface;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<StudentResponse>>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Service: Fetching all students from persistence.");
        var allStudents = await _unitOfWork.Students.GetAllAsync();
        var students = allStudents.Where(s => !s.IsDeleted).ToList();
        var response = students.Select(s => MapToResponse(s));
        
        _logger.LogInformation("Service: Successfully retrieved {Count} students.", students.Count);
        return ApiResponse<IEnumerable<StudentResponse>>.SuccessResult(response);
    }

    public async Task<ApiResponse<StudentResponse>> GetStudentByIdAsync(int id)
    {
        _logger.LogInformation("Service: Fetching student with ID {Id}.", id);
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
        {
            _logger.LogWarning("Service: Student with ID {Id} not found or deleted.", id);
            return ApiResponse<StudentResponse>.FailResult($"Student with ID {id} not found.");
        }

        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student));
    }

    public async Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request)
    {
        _logger.LogInformation("Service: Creating student with email {Email}.", request.Email);
        var existing = await _unitOfWork.Students.GetByEmailAsync(request.Email);
        if (existing != null)
        {
            _logger.LogWarning("Service: Create failed. Student with email {Email} already exists.", request.Email);
            return ApiResponse<StudentResponse>.FailResult("A student with this email already exists.");
        }

        var student = new Student
        {
            Name = request.Name,
            Email = request.Email,
            Age = request.Age,
            Course = request.Course,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Service: Student created successfully with ID {Id}.", student.Id);
        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student), "Student created successfully.");
    }

    public async Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
    {
        _logger.LogInformation("Service: Updating student with ID {Id}.", id);
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
        {
            _logger.LogWarning("Service: Update failed. Student with ID {Id} not found.", id);
            return ApiResponse<StudentResponse>.FailResult($"Student with ID {id} not found.");
        }

        var emailCheck = await _unitOfWork.Students.GetByEmailAsync(request.Email);
        if (emailCheck != null && emailCheck.Id != id)
        {
            _logger.LogWarning("Service: Update failed. Email {Email} is already taken by another student.", request.Email);
            return ApiResponse<StudentResponse>.FailResult("Another student with this email already exists.");
        }

        student.Name = request.Name;
        student.Email = request.Email;
        student.Age = request.Age;
        student.Course = request.Course;
        student.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Service: Student with ID {Id} updated successfully.", id);
        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student), "Student updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
    {
        _logger.LogInformation("Service: Deleting student with ID {Id}.", id);
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
        {
            _logger.LogWarning("Service: Delete failed. Student with ID {Id} not found.", id);
            return ApiResponse<bool>.FailResult($"Student with ID {id} not found.");
        }

        student.IsDeleted = true;
        student.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Service: Student with ID {Id} soft-deleted successfully.", id);
        return ApiResponse<bool>.SuccessResult(true, "Student deleted successfully.");
    }

    private static StudentResponse MapToResponse(Student s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Email = s.Email,
        Age = s.Age,
        Course = s.Course,
        CreatedDate = s.CreatedDate,
        UpdatedDate = s.UpdatedDate
    };
}
