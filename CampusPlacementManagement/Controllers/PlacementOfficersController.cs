
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacementOfficersController : ControllerBase
    {
        private readonly IPlacementOfficerService _service;

        public PlacementOfficersController(
            IPlacementOfficerService service)
        {
            _service = service;
        }

        // POST: api/PlacementOfficers/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            RegisterPlacementOfficerRequest request)
        {
            var result = await _service.RegisterAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        // GET: api/PlacementOfficers
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return StatusCode(result.StatusCode, result);
        }

        // GET: api/PlacementOfficers/1
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/PlacementOfficers/1/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.ApproveAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
