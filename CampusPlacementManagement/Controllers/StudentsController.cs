using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "PlacementOfficer")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Enroll a new student
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(
            [FromBody] EnrollStudentRequest request)
        {
            var result = await _studentService.EnrollAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        // Get all students belonging to the Placement Officer's college
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _studentService.GetAllAsync();

            return StatusCode(result.StatusCode, result);
        }

        // Get student by ID
        [HttpGet("{studentId:int}")]
        public async Task<IActionResult> GetById(int studentId)
        {
            var result = await _studentService.GetByIdAsync(studentId);

            return StatusCode(result.StatusCode, result);
        }

        // Approve student
        [HttpPut("{studentId:int}/approve")]
        public async Task<IActionResult> Approve(int studentId)
        {
            var result = await _studentService.ApproveAsync(studentId);

            return StatusCode(result.StatusCode, result);
        }

        // Reject student
        [HttpPut("{studentId:int}/reject")]
        public async Task<IActionResult> Reject(int studentId)
        {
            var result = await _studentService.RejectAsync(studentId);

            return StatusCode(result.StatusCode, result);
        }

        // Update student
        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> Update(
            int studentId,
            [FromBody] UpdateStudentRequest request)
        {
            var result = await _studentService.UpdateAsync(studentId, request);

            return StatusCode(result.StatusCode, result);
        }
    }
}