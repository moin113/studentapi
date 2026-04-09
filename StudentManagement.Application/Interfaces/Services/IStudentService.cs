using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;

namespace StudentManagement.Application.Interfaces.Services;

public interface IStudentService
{
    Task<ApiResponse<IEnumerable<StudentResponse>>> GetAllStudentsAsync();
    Task<ApiResponse<StudentResponse>> GetStudentByIdAsync(int id);
    Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request);
    Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
    Task<ApiResponse<bool>> DeleteStudentAsync(int id);
}
