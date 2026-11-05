using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> AddAsync(Student student);

        Task<List<Student>> GetAllAsync();

        Task<Student?> GetByIdAsync(int studentId);

        Task<Student?> GetByUserIdAsync(string userId);

        Task<Student?> GetByRegisterNumberAsync(
            int collegeId,
            string registerNumber);

        Task<List<Student>> GetByCollegeIdAsync(int collegeId);

        Task SaveChangesAsync();
    }
}