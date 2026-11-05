using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.CompanyHRs;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICompanyHRService
    {
        Task<ServiceResult<CompanyHRResponse>> RegisterAsync(
            RegisterCompanyHRRequest request);

        Task<ServiceResult<List<CompanyHRResponse>>> GetAllAsync();

        Task<ServiceResult<CompanyHRResponse>> GetByIdAsync(int companyHRId);

        Task<ServiceResult<CompanyHRResponse>> ApproveAsync(int companyHRId);
    }
}