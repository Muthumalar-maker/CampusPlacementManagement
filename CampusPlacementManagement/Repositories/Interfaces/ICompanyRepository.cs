using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> AddAsync(Company company);
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int companyId);
        Task SaveChangesAsync();
    }
}