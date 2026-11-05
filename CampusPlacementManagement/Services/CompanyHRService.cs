using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services
{
    public class CompanyHRService : ICompanyHRService
    {
        private readonly ICompanyHRRepository _companyHRRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyHRService(
            ICompanyHRRepository companyHRRepository,
            ICompanyRepository companyRepository,
            UserManager<ApplicationUser> userManager)
        {
            _companyHRRepository = companyHRRepository;
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        public async Task<ServiceResult<CompanyHRResponse>> RegisterAsync(
            RegisterCompanyHRRequest request)
        {
            // Check whether the company exists
            var company = await _companyRepository.GetByIdAsync(request.CompanyId);

            if (company == null)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "Company not found.", 404);
            }

            // Only approved companies can register HR accounts
            if (company.Status != CompanyStatus.Approved)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "Company is not approved. HR registration is not allowed.", 400);
            }

            // Check duplicate email
            var email = request.Email.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "Email already exists.", 409);
            }

            // Create Identity user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                PhoneNumber = request.ContactNumber.Trim(),
                AccountStatus = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(
                user, request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    identityResult.Errors.Select(error => error.Description));

                return ServiceResult<CompanyHRResponse>.Failure(
                    errors, 400);
            }

            // Create CompanyHR profile
            var companyHR = new CompanyHR
            {
                UserId = user.Id,
                CompanyId = company.CompanyId,
                FullName = request.FullName.Trim(),
                Email = email,
                ContactNumber = request.ContactNumber.Trim(),
                Status = CompanyHRStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var createdHR = await _companyHRRepository.AddAsync(companyHR);

                return ServiceResult<CompanyHRResponse>.Success(
                    MapToResponse(createdHR),
                    "CompanyHR registered successfully. Waiting for SuperAdmin approval.",
                    201);
            }
            catch
            {
                // Remove Identity user if profile creation fails
                await _userManager.DeleteAsync(user);

                return ServiceResult<CompanyHRResponse>.Failure(
                    "Unable to register CompanyHR.", 500);
            }
        }

        public async Task<ServiceResult<List<CompanyHRResponse>>> GetAllAsync()
        {
            var companyHRs = await _companyHRRepository.GetAllAsync();

            var response = companyHRs
                .Select(MapToResponse)
                .ToList();

            return ServiceResult<List<CompanyHRResponse>>.Success(
                response,
                "CompanyHR records retrieved successfully.");
        }

        public async Task<ServiceResult<CompanyHRResponse>> GetByIdAsync(
            int companyHRId)
        {
            var companyHR = await _companyHRRepository.GetByIdAsync(companyHRId);

            if (companyHR == null)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "CompanyHR not found.", 404);
            }

            return ServiceResult<CompanyHRResponse>.Success(
                MapToResponse(companyHR),
                "CompanyHR retrieved successfully.");
        }

        public async Task<ServiceResult<CompanyHRResponse>> ApproveAsync(
            int companyHRId)
        {
            var companyHR = await _companyHRRepository.GetByIdAsync(companyHRId);

            if (companyHR == null)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "CompanyHR not found.", 404);
            }

            if (companyHR.Status == CompanyHRStatus.Approved)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "CompanyHR is already approved.", 400);
            }

            var user = await _userManager.FindByIdAsync(companyHR.UserId);

            if (user == null)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "Identity user not found.", 404);
            }

            // Assign CompanyHR role
            var roleResult = await _userManager.AddToRoleAsync(user, "CompanyHR");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(error => error.Description));

                return ServiceResult<CompanyHRResponse>.Failure(errors, 400);
            }

            // Update account status
            user.AccountStatus = AccountStatus.Approved;
            user.EmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return ServiceResult<CompanyHRResponse>.Failure(
                    "Unable to update Identity account.", 400);
            }

            companyHR.Status = CompanyHRStatus.Approved;
            companyHR.UpdatedAt = DateTime.UtcNow;

            await _companyHRRepository.SaveChangesAsync();

            return ServiceResult<CompanyHRResponse>.Success(
                MapToResponse(companyHR),
                "CompanyHR approved successfully.");
        }

        private static CompanyHRResponse MapToResponse(CompanyHR companyHR)
        {
            return new CompanyHRResponse
            {
                CompanyHRId = companyHR.CompanyHRId,
                UserId = companyHR.UserId,
                CompanyId = companyHR.CompanyId,
                CompanyName = companyHR.Company?.CompanyName ?? string.Empty,
                FullName = companyHR.FullName,
                Email = companyHR.Email,
                ContactNumber = companyHR.ContactNumber,
                Status = companyHR.Status.ToString(),
                CreatedAt = companyHR.CreatedAt
            };
        }
    }
}