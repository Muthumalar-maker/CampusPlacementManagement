
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Colleges;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface ICollegeService
    {
        Task<ServiceResult<CollegeResponse>> RegisterAsync(
            RegisterCollegeRequest request);

        Task<ServiceResult<List<CollegeResponse>>> GetAllAsync();

        Task<ServiceResult<CollegeResponse>> GetByIdAsync(int id);

        Task<ServiceResult<CollegeResponse>> ApproveAsync(int id);
    }
}
