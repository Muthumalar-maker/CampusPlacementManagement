using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterCompanyRequest request)
        {
            var result = await _companyService.RegisterAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _companyService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{companyId:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(int companyId)
        {
            var result = await _companyService.GetByIdAsync(companyId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{companyId:int}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Approve(int companyId)
        {
            var result = await _companyService.ApproveAsync(companyId);
            return StatusCode(result.StatusCode, result);
        }
    }
}