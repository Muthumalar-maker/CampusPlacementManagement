using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<ServiceResult<CompanyResponse>> RegisterAsync(
            RegisterCompanyRequest request)
        {
            var company = new Company
            {
                CompanyName = request.CompanyName.Trim(),
                Industry = request.Industry?.Trim(),
                Website = request.Website?.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim(),
                State = request.State.Trim(),
                Country = request.Country.Trim(),
                ContactNumber = request.ContactNumber.Trim(),
                Email = request.Email.Trim(),
                Status = CompanyStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdCompany = await _companyRepository.AddAsync(company);

            return ServiceResult<CompanyResponse>.Success(
                MapToResponse(createdCompany),
                "Company registered successfully. It is waiting for SuperAdmin approval.",
                201);
        }

        public async Task<ServiceResult<List<CompanyResponse>>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();

            var response = companies
                .Select(MapToResponse)
                .ToList();

            return ServiceResult<List<CompanyResponse>>.Success(
                response,
                "Companies retrieved successfully.");
        }

        public async Task<ServiceResult<CompanyResponse>> GetByIdAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);

            if (company is null)
            {
                return ServiceResult<CompanyResponse>.Failure(
                    "Company not found.",
                    404);
            }

            return ServiceResult<CompanyResponse>.Success(
                MapToResponse(company),
                "Company retrieved successfully.");
        }

        public async Task<ServiceResult<CompanyResponse>> ApproveAsync(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);

            if (company is null)
            {
                return ServiceResult<CompanyResponse>.Failure(
                    "Company not found.",
                    404);
            }

            if (company.Status == CompanyStatus.Approved)
            {
                return ServiceResult<CompanyResponse>.Failure(
                    "Company is already approved.",
                    400);
            }

            company.Status = CompanyStatus.Approved;
            company.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.SaveChangesAsync();

            return ServiceResult<CompanyResponse>.Success(
                MapToResponse(company),
                "Company approved successfully.");
        }

        private static CompanyResponse MapToResponse(Company company)
        {
            return new CompanyResponse
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Industry = company.Industry,
                Website = company.Website,
                Address = company.Address,
                City = company.City,
                State = company.State,
                Country = company.Country,
                ContactNumber = company.ContactNumber,
                Email = company.Email,
                Status = company.Status.ToString(),
                CreatedAt = company.CreatedAt
            };
        }
    }
}