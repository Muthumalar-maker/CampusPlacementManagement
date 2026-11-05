
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services
{
    public class PlacementOfficerService : IPlacementOfficerService
    {
        private readonly IPlacementOfficerRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlacementOfficerService(
            IPlacementOfficerRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<ServiceResult<PlacementOfficerRegistrationResponse>>
            RegisterAsync(RegisterPlacementOfficerRequest request)
        {
            // Check college
            var college = await _repository.GetCollegeByIdAsync(
                request.CollegeId);

            if (college == null)
            {
                return ServiceResult<PlacementOfficerRegistrationResponse>
                    .Failure("College not found.", 404);
            }

            if (college.Status != CollegeStatus.Approved)
            {
                return ServiceResult<PlacementOfficerRegistrationResponse>
                    .Failure(
                        "Only approved colleges can register Placement Officers.",
                        400);
            }

            // Check duplicate email
            var email = request.Email.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return ServiceResult<PlacementOfficerRegistrationResponse>
                    .Failure("Email address is already registered.", 409);
            }

            // Create Identity user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                AccountStatus = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(
                user, request.Password);

            if (!identityResult.Succeeded)
            {
                string errors = string.Join(
                    ", ",
                    identityResult.Errors.Select(e => e.Description));

                return ServiceResult<PlacementOfficerRegistrationResponse>
                    .Failure(errors, 400);
            }

            // Create Placement Officer profile
            var officer = new PlacementOfficer
            {
                UserId = user.Id,
                CollegeId = request.CollegeId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _repository.AddAsync(officer);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                // Avoid leaving an Identity account without a profile
                await _userManager.DeleteAsync(user);
                throw;
            }

            var response = new PlacementOfficerRegistrationResponse
            {
                PlacementOfficerId = officer.PlacementOfficerId,
                FullName = user.FullName,
                Email = user.Email!,
                AccountStatus = user.AccountStatus.ToString()
            };

            return ServiceResult<PlacementOfficerRegistrationResponse>
                .Success(
                    response,
                    "Registration submitted. Waiting for SuperAdmin approval.",
                    201);
        }

        public async Task<ServiceResult<List<PlacementOfficerResponse>>>
            GetAllAsync()
        {
            var officers = await _repository.GetAllAsync();

            var response = officers.Select(MapToResponse).ToList();

            return ServiceResult<List<PlacementOfficerResponse>>.Success(
                response,
                "Placement Officers retrieved successfully.");
        }

        public async Task<ServiceResult<PlacementOfficerResponse>>
            GetByIdAsync(int id)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
            {
                return ServiceResult<PlacementOfficerResponse>.Failure(
                    "Placement Officer not found.", 404);
            }

            return ServiceResult<PlacementOfficerResponse>.Success(
                MapToResponse(officer),
                "Placement Officer retrieved successfully.");
        }

        public async Task<ServiceResult<PlacementOfficerResponse>>
            ApproveAsync(int id)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
            {
                return ServiceResult<PlacementOfficerResponse>.Failure(
                    "Placement Officer not found.", 404);
            }

            if (officer.College.Status != CollegeStatus.Approved)
            {
                return ServiceResult<PlacementOfficerResponse>.Failure(
                    "The officer's college must be approved first.", 400);
            }

            if (officer.User.AccountStatus != AccountStatus.Pending)
            {
                return ServiceResult<PlacementOfficerResponse>.Failure(
                    "Only pending accounts can be approved.", 400);
            }

            // Assign role
            var roleResult = await _userManager.AddToRoleAsync(
                officer.User,
                "PlacementOfficer");

            if (!roleResult.Succeeded)
            {
                string errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                return ServiceResult<PlacementOfficerResponse>.Failure(
                    errors, 400);
            }

            officer.User.AccountStatus = AccountStatus.Approved;
            officer.User.EmailConfirmed = true;
            officer.User.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return ServiceResult<PlacementOfficerResponse>.Success(
                MapToResponse(officer),
                "Placement Officer approved successfully.");
        }

        private static PlacementOfficerResponse MapToResponse(
            PlacementOfficer officer)
        {
            return new PlacementOfficerResponse
            {
                PlacementOfficerId = officer.PlacementOfficerId,
                UserId = officer.UserId,
                FullName = officer.User.FullName,
                Email = officer.User.Email ?? string.Empty,
                PhoneNumber = officer.User.PhoneNumber ?? string.Empty,
                CollegeId = officer.CollegeId,
                CollegeName = officer.College.CollegeName,
                AccountStatus = officer.User.AccountStatus.ToString(),
                CreatedAt = officer.CreatedAt
            };
        }
    }
}
