using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Students;

namespace CampusPlacementManagement.Services.Interfaces
{
    public interface IStudentService
    {
        Task<ServiceResult<StudentResponse>> EnrollAsync(
            EnrollStudentRequest request);

        Task<ServiceResult<List<StudentResponse>>> GetAllAsync();

        Task<ServiceResult<StudentResponse>> GetByIdAsync(int studentId);

        Task<ServiceResult<StudentResponse>> ApproveAsync(int studentId);

        Task<ServiceResult<StudentResponse>> RejectAsync(int studentId);

        Task<ServiceResult<StudentResponse>> UpdateAsync(
            int studentId,
            UpdateStudentRequest request);
    }
}