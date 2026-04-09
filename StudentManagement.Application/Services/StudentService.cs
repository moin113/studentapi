using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Interface;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IEnumerable<StudentResponse>>> GetAllStudentsAsync()
    {
        // Notice: Use GetAllAsync and filter rather than GetActiveStudentsAsync since interface might not have it yet
        var allStudents = await _unitOfWork.Students.GetAllAsync();
        var students = allStudents.Where(s => !s.IsDeleted).ToList();
        var response = students.Select(s => MapToResponse(s));
        return ApiResponse<IEnumerable<StudentResponse>>.SuccessResult(response);
    }

    public async Task<ApiResponse<StudentResponse>> GetStudentByIdAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
            return ApiResponse<StudentResponse>.FailResult($"Student with ID {id} not found.");

        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student));
    }

    public async Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request)
    {
        var existing = await _unitOfWork.Students.GetByEmailAsync(request.Email);
        if (existing != null)
            return ApiResponse<StudentResponse>.FailResult("A student with this email already exists.");

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

        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student), "Student created successfully.");
    }

    public async Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
            return ApiResponse<StudentResponse>.FailResult($"Student with ID {id} not found.");

        // Check email conflict with another student
        var emailCheck = await _unitOfWork.Students.GetByEmailAsync(request.Email);
        if (emailCheck != null && emailCheck.Id != id)
            return ApiResponse<StudentResponse>.FailResult("Another student with this email already exists.");

        student.Name = request.Name;
        student.Email = request.Email;
        student.Age = request.Age;
        student.Course = request.Course;
        student.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<StudentResponse>.SuccessResult(MapToResponse(student), "Student updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null || student.IsDeleted)
            return ApiResponse<bool>.FailResult($"Student with ID {id} not found.");

        // Soft delete — never physically remove
        student.IsDeleted = true;
        student.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync();

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
