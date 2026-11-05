using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICompanyHRRepository
    {
        Task<CompanyHR> AddAsync(CompanyHR companyHR);

        Task<List<CompanyHR>> GetAllAsync();

        Task<CompanyHR?> GetByIdAsync(int companyHRId);

        Task<CompanyHR?> GetByUserIdAsync(string userId);

        Task SaveChangesAsync();
    }
}