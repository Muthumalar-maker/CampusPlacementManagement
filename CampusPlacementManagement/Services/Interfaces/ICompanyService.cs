using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Companies;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<CompanyResponse>> RegisterAsync(
            RegisterCompanyRequest request);

        Task<ServiceResult<List<CompanyResponse>>> GetAllAsync();

        Task<ServiceResult<CompanyResponse>> GetByIdAsync(int companyId);

        Task<ServiceResult<CompanyResponse>> ApproveAsync(int companyId);
    }
}