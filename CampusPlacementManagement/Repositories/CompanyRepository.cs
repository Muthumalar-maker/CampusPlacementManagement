using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .OrderBy(company => company.CompanyName)
                .ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int companyId)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(company => company.CompanyId == companyId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}