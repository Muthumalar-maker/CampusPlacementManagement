
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class CollegeRepository : ICollegeRepository
    {
        private readonly ApplicationDbContext _context;

        public CollegeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<College>> GetAllAsync()
        {
            return await _context.Colleges
                .AsNoTracking()
                .OrderBy(c => c.CollegeId)
                .ToListAsync();
        }

        public async Task<College?> GetByIdAsync(int id)
        {
            return await _context.Colleges
                .FirstOrDefaultAsync(c => c.CollegeId == id);
        }

        public async Task<College?> GetByCodeAsync(string collegeCode)
        {
            return await _context.Colleges
                .FirstOrDefaultAsync(c =>
                    c.CollegeCode == collegeCode);
        }

        public async Task AddAsync(College college)
        {
            await _context.Colleges.AddAsync(college);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
