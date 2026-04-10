using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interfaces.Services;

namespace StudentManagement.Controllers;

/// <summary>
/// Controller for managing student records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    /// <returns>A list of student records.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all students.");
        var result = await _studentService.GetAllStudentsAsync();
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    /// <summary>
    /// Retrieves a specific student by ID.
    /// </summary>
    /// <param name="id">The student ID.</param>
    /// <returns>The student record if found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting student with ID {Id}.", id);
        var result = await _studentService.GetStudentByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }

    /// <summary>
    /// Creates a new student record.
    /// </summary>
    /// <param name="request">The student creation details.</param>
    /// <returns>The newly created student record.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        _logger.LogInformation("Creating a new student with email {Email}.", request.Email);
        var result = await _studentService.CreateStudentAsync(request);
        return result.Success 
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data) 
            : BadRequest(result);
    }

    /// <summary>
    /// Updates an existing student record.
    /// </summary>
    /// <param name="id">The identification of the student to update.</param>
    /// <param name="request">The updated student details.</param>
    /// <returns>The updated student record.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        _logger.LogInformation("Updating student with ID {Id}.", id);
        var result = await _studentService.UpdateStudentAsync(id, request);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    /// <summary>
    /// Deletes a student record (Soft Delete).
    /// </summary>
    /// <param name="id">The identification of the student to delete.</param>
    /// <returns>A success indicator.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting student with ID {Id}.", id);
        var result = await _studentService.DeleteStudentAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }
}
