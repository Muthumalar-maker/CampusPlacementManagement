
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services
{
    public class CollegeService : ICollegeService
    {
        private readonly ICollegeRepository _repository;

        public CollegeService(ICollegeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<CollegeResponse>> RegisterAsync(
            RegisterCollegeRequest request)
        {
            string collegeCode = request.CollegeCode.Trim()
                .ToUpperInvariant();

            var existingCollege = await _repository
                .GetByCodeAsync(collegeCode);

            if (existingCollege != null)
            {
                return ServiceResult<CollegeResponse>.Failure(
                    "College code is already registered.", 409);
            }

            var college = new College
            {
                CollegeName = request.CollegeName.Trim(),
                CollegeCode = collegeCode,
                Address = request.Address.Trim(),
                City = request.City.Trim(),
                State = request.State.Trim(),
                ContactNumber = request.ContactNumber.Trim(),
                Email = request.Email.Trim(),
                Status = CollegeStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(college);
            await _repository.SaveChangesAsync();

            return ServiceResult<CollegeResponse>.Success(
                MapToResponse(college),
                "College registration submitted successfully. Waiting for SuperAdmin approval.",
                201);
        }

        public async Task<ServiceResult<List<CollegeResponse>>> GetAllAsync()
        {
            var colleges = await _repository.GetAllAsync();

            var response = colleges.Select(MapToResponse).ToList();

            return ServiceResult<List<CollegeResponse>>.Success(
                response, "Colleges retrieved successfully.");
        }

        public async Task<ServiceResult<CollegeResponse>> GetByIdAsync(
            int id)
        {
            var college = await _repository.GetByIdAsync(id);

            if (college == null)
            {
                return ServiceResult<CollegeResponse>.Failure(
                    "College not found.", 404);
            }

            return ServiceResult<CollegeResponse>.Success(
                MapToResponse(college),
                "College retrieved successfully.");
        }

        public async Task<ServiceResult<CollegeResponse>> ApproveAsync(
            int id)
        {
            var college = await _repository.GetByIdAsync(id);

            if (college == null)
            {
                return ServiceResult<CollegeResponse>.Failure(
                    "College not found.", 404);
            }

            if (college.Status != CollegeStatus.Pending)
            {
                return ServiceResult<CollegeResponse>.Failure(
                    "Only pending colleges can be approved.", 400);
            }

            college.Status = CollegeStatus.Approved;
            college.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return ServiceResult<CollegeResponse>.Success(
                MapToResponse(college),
                "College approved successfully.");
        }

        private static CollegeResponse MapToResponse(College college)
        {
            return new CollegeResponse
            {
                CollegeId = college.CollegeId,
                CollegeName = college.CollegeName,
                CollegeCode = college.CollegeCode,
                Address = college.Address,
                City = college.City,
                State = college.State,
                ContactNumber = college.ContactNumber,
                Email = college.Email,
                //Status = college.Status.ToString(),
                Status = college.Status,
                CreatedAt = college.CreatedAt
            };
        }
    }
}
