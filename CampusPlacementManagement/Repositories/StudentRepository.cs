using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add Student
        public async Task<Student> AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return student;
        }

        // Get All Students
        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.College)
                .Include(s => s.User)
                .AsNoTracking()
                .ToListAsync();
        }

        // Get Student By ID
        public async Task<Student?> GetByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.College)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        // Get Student By User ID
        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.College)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        // Get Student By Register Number
        public async Task<Student?> GetByRegisterNumberAsync(
            int collegeId,
            string registerNumber)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s =>
                    s.CollegeId == collegeId &&
                    s.RegisterNumber == registerNumber);
        }

        // Get Students By College ID
        public async Task<List<Student>> GetByCollegeIdAsync(int collegeId)
        {
            return await _context.Students
                .Include(s => s.College)
                .Include(s => s.User)
                .Where(s => s.CollegeId == collegeId)
                .AsNoTracking()
                .ToListAsync();
        }

        // Save Changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}