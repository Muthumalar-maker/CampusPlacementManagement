
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface IPlacementOfficerRepository
    {
        Task<College?> GetCollegeByIdAsync(int collegeId);

        Task<List<PlacementOfficer>> GetAllAsync();

        Task<PlacementOfficer?> GetByIdAsync(int id);

        Task AddAsync(PlacementOfficer officer);

        Task SaveChangesAsync();
    }
}
