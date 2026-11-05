
using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollegesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CollegesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/colleges/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCollege(
            RegisterCollegeRequest request)
        {
            bool exists = await _context.Colleges
                .AnyAsync(c => c.CollegeCode == request.CollegeCode);

            if (exists)
            {
                return Conflict(new
                {
                    message = "College code already exists."
                });
            }

            var college = new College
            {
                CollegeName = request.CollegeName,
                CollegeCode = request.CollegeCode,
                Address = request.Address,
                City = request.City,
                State = request.State,
                ContactNumber = request.ContactNumber,
                Email = request.Email,
                Status = CollegeStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Colleges.Add(college);
            await _context.SaveChangesAsync();

            var response = new CollegeResponse
            {
                CollegeId = college.CollegeId,
                CollegeName = college.CollegeName,
                CollegeCode = college.CollegeCode,
                Address = college.Address,
                City = college.City,
                State = college.State,
                ContactNumber = college.ContactNumber,
                Email = college.Email,
                Status = college.Status,
                CreatedAt = college.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetCollegeById),
                new { id = college.CollegeId },
                response);
        }

        // GET: api/colleges
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllColleges()
        {
            var colleges = await _context.Colleges
                .Select(c => new CollegeResponse
                {
                    CollegeId = c.CollegeId,
                    CollegeName = c.CollegeName,
                    CollegeCode = c.CollegeCode,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    ContactNumber = c.ContactNumber,
                    Email = c.Email,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(colleges);
        }

        // GET: api/colleges/1
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetCollegeById(int id)
        {
            var college = await _context.Colleges
                .Where(c => c.CollegeId == id)
                .Select(c => new CollegeResponse
                {
                    CollegeId = c.CollegeId,
                    CollegeName = c.CollegeName,
                    CollegeCode = c.CollegeCode,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    ContactNumber = c.ContactNumber,
                    Email = c.Email,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (college == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            return Ok(college);
        }

        // PUT: api/colleges/1/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ApproveCollege(int id)
        {
            var college = await _context.Colleges.FindAsync(id);

            if (college == null)
            {
                return NotFound(new
                {
                    message = "College not found."
                });
            }

            if (college.Status != CollegeStatus.Pending)
            {
                return BadRequest(new
                {
                    message = "Only pending colleges can be approved."
                });
            }

            college.Status = CollegeStatus.Approved;
            college.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "College approved successfully."
            });
        }
    }
}
