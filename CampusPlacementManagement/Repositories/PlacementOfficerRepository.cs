
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class PlacementOfficerRepository
        : IPlacementOfficerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacementOfficerRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<College?> GetCollegeByIdAsync(int collegeId)
        {
            return await _context.Colleges
                .FirstOrDefaultAsync(c => c.CollegeId == collegeId);
        }

        public async Task<List<PlacementOfficer>> GetAllAsync()
        {
            return await _context.PlacementOfficers
                .AsNoTracking()
                .Include(po => po.User)
                .Include(po => po.College)
                .OrderBy(po => po.PlacementOfficerId)
                .ToListAsync();
        }

        public async Task<PlacementOfficer?> GetByIdAsync(int id)
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .FirstOrDefaultAsync(po =>
                    po.PlacementOfficerId == id);
        }

        public async Task AddAsync(PlacementOfficer officer)
        {
            await _context.PlacementOfficers.AddAsync(officer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
