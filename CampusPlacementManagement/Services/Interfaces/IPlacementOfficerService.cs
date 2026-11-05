
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.PlacementOfficers;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface IPlacementOfficerService
    {
        Task<ServiceResult<PlacementOfficerRegistrationResponse>>
            RegisterAsync(RegisterPlacementOfficerRequest request);

        Task<ServiceResult<List<PlacementOfficerResponse>>>
            GetAllAsync();

        Task<ServiceResult<PlacementOfficerResponse>>
            GetByIdAsync(int id);

        Task<ServiceResult<PlacementOfficerResponse>>
            ApproveAsync(int id);
    }
}
