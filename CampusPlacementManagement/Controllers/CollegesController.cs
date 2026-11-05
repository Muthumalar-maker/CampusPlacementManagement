
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollegesController : ControllerBase
    {
        private readonly ICollegeService _service;

        public CollegesController(ICollegeService service)
        {
            _service = service;
        }

        // POST: api/Colleges/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            RegisterCollegeRequest request)
        {
            var result = await _service.RegisterAsync(request);

            if (result.StatusCode == 201)
            {
                return StatusCode(201, result);
            }

            return StatusCode(result.StatusCode, result);
        }

        // GET: api/Colleges
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return StatusCode(result.StatusCode, result);
        }

        // GET: api/Colleges/1
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/Colleges/1/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.ApproveAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
