
using System.Security.Claims;
using CampusPlacementManagement.Common;
using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPlacementOfficerRepository _officerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentService(
            IStudentRepository studentRepository,
            IPlacementOfficerRepository officerRepository,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _officerRepository = officerRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // Get logged-in user's Identity ID
        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Get logged-in Placement Officer
        private async Task<PlacementOfficer?> GetCurrentOfficerAsync()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return null;

            return await _officerRepository.GetByUserIdAsync(userId);
        }

        // 1. Enroll Student
        public async Task<ServiceResult<StudentResponse>> EnrollAsync(
            EnrollStudentRequest request)
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Placement Officer not found.", 403);
            }

            // Check college permission
            if (officer.CollegeId != request.CollegeId)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "You can enroll students only in your college.", 403);
            }

            // Check duplicate register number
            var existingStudent =
                await _studentRepository.GetByRegisterNumberAsync(
                    request.CollegeId,
                    request.RegisterNumber.Trim());

            if (existingStudent != null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Register number already exists in this college.", 409);
            }

            var email = request.Email.Trim();

            // Check duplicate email
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Email already exists.", 409);
            }

            // Create Identity user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                PhoneNumber = request.ContactNumber.Trim(),
                AccountStatus = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(
                user, request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    identityResult.Errors.Select(e => e.Description));

                return ServiceResult<StudentResponse>.Failure(
                    errors, 400);
            }

            try
            {
                // Create Student profile
                var student = new Student
                {
                    UserId = user.Id,
                    CollegeId = request.CollegeId,
                    PlacementOfficerId = officer.PlacementOfficerId,
                    RegisterNumber = request.RegisterNumber.Trim(),
                    Department = request.Department.Trim(),
                    Degree = request.Degree.Trim(),
                    GraduationYear = request.GraduationYear,
                    CGPA = request.CGPA,
                    ActiveBacklogs = request.ActiveBacklogs,
                    ResumeUrl = request.ResumeUrl,
                    Status = StudentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                var createdStudent =
                    await _studentRepository.AddAsync(student);

                var savedStudent =
                    await _studentRepository.GetByIdAsync(
                        createdStudent.StudentId);

                if (savedStudent == null)
                {
                    await _userManager.DeleteAsync(user);

                    return ServiceResult<StudentResponse>.Failure(
                        "Unable to retrieve enrolled student.", 500);
                }

                return ServiceResult<StudentResponse>.Success(
                    MapToResponse(savedStudent),
                    "Student enrolled successfully. Waiting for approval.",
                    201);
            }
            catch
            {
                // Remove Identity user if profile creation fails
                await _userManager.DeleteAsync(user);

                return ServiceResult<StudentResponse>.Failure(
                    "Unable to enroll student.", 500);
            }
        }

        // 2. Get all students of logged-in officer's college
        public async Task<ServiceResult<List<StudentResponse>>> GetAllAsync()
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<List<StudentResponse>>.Failure(
                    "Placement Officer not found.", 403);
            }

            var students =
                await _studentRepository.GetByCollegeIdAsync(
                    officer.CollegeId);

            var response = students
                .Select(MapToResponse)
                .ToList();

            return ServiceResult<List<StudentResponse>>.Success(
                response,
                "Students retrieved successfully.");
        }

        // 3. Get Student by ID
        public async Task<ServiceResult<StudentResponse>> GetByIdAsync(
            int studentId)
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Placement Officer not found.", 403);
            }

            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null ||
                student.CollegeId != officer.CollegeId)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Student not found in your college.", 404);
            }

            return ServiceResult<StudentResponse>.Success(
                MapToResponse(student),
                "Student retrieved successfully.");
        }

        // 4. Approve Student
        public async Task<ServiceResult<StudentResponse>> ApproveAsync(
            int studentId)
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Placement Officer not found.", 403);
            }

            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null ||
                student.CollegeId != officer.CollegeId)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Student not found in your college.", 404);
            }

            if (student.Status == StudentStatus.Approved)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Student is already approved.", 400);
            }

            var user =
                await _userManager.FindByIdAsync(student.UserId);

            if (user == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Identity user not found.", 404);
            }

            // Assign Student role if not already assigned
            var isStudentRole =
                await _userManager.IsInRoleAsync(user, "Student");

            if (!isStudentRole)
            {
                var roleResult =
                    await _userManager.AddToRoleAsync(user, "Student");

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(e => e.Description));

                    return ServiceResult<StudentResponse>.Failure(
                        errors, 400);
                }
            }

            // Update Identity account
            user.AccountStatus = AccountStatus.Approved;
            user.EmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Unable to update Identity account.", 400);
            }

            // Update Student status
            student.Status = StudentStatus.Approved;
            student.ApprovedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;

            await _studentRepository.SaveChangesAsync();

            return ServiceResult<StudentResponse>.Success(
                MapToResponse(student),
                "Student approved successfully.");
        }

        // 5. Reject Student
        public async Task<ServiceResult<StudentResponse>> RejectAsync(
            int studentId)
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Placement Officer not found.", 403);
            }

            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null ||
                student.CollegeId != officer.CollegeId)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Student not found in your college.", 404);
            }

            if (student.Status == StudentStatus.Approved)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "An approved student cannot be rejected.", 400);
            }

            student.Status = StudentStatus.Rejected;
            student.UpdatedAt = DateTime.UtcNow;

            await _studentRepository.SaveChangesAsync();

            return ServiceResult<StudentResponse>.Success(
                MapToResponse(student),
                "Student rejected successfully.");
        }

        // 6. Update Student
        public async Task<ServiceResult<StudentResponse>> UpdateAsync(
            int studentId,
            UpdateStudentRequest request)
        {
            var officer = await GetCurrentOfficerAsync();

            if (officer == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Placement Officer not found.", 403);
            }

            var student =
                await _studentRepository.GetByIdAsync(studentId);

            if (student == null ||
                student.CollegeId != officer.CollegeId)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Student not found in your college.", 404);
            }

            var user =
                await _userManager.FindByIdAsync(student.UserId);

            if (user == null)
            {
                return ServiceResult<StudentResponse>.Failure(
                    "Identity user not found.", 404);
            }

            // Update Student academic details
            student.Department = request.Department.Trim();
            student.Degree = request.Degree.Trim();
            student.GraduationYear = request.GraduationYear;
            student.CGPA = request.CGPA;
            student.ActiveBacklogs = request.ActiveBacklogs;
            student.ResumeUrl = request.ResumeUrl;
            student.UpdatedAt = DateTime.UtcNow;

            // Update Identity user details
            user.FullName = request.FullName.Trim();
            user.PhoneNumber = request.ContactNumber.Trim();

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    updateResult.Errors.Select(e => e.Description));

                return ServiceResult<StudentResponse>.Failure(
                    errors, 400);
            }

            await _studentRepository.SaveChangesAsync();

            var updatedStudent =
                await _studentRepository.GetByIdAsync(studentId);

            return ServiceResult<StudentResponse>.Success(
                MapToResponse(updatedStudent!),
                "Student updated successfully.");
        }

        // Map Student model to response DTO
        private static StudentResponse MapToResponse(Student student)
        {
            return new StudentResponse
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                CollegeId = student.CollegeId,
                CollegeName = student.College?.CollegeName ?? string.Empty,
                PlacementOfficerId = student.PlacementOfficerId,
                RegisterNumber = student.RegisterNumber,
                FullName = student.User?.FullName ?? string.Empty,
                Email = student.User?.Email ?? string.Empty,
                ContactNumber = student.User?.PhoneNumber ?? string.Empty,
                Department = student.Department,
                Degree = student.Degree,
                GraduationYear = student.GraduationYear,
                CGPA = student.CGPA,
                ActiveBacklogs = student.ActiveBacklogs,
                ResumeUrl = student.ResumeUrl,
                Status = student.Status.ToString(),
                CreatedAt = student.CreatedAt,
                ApprovedAt = student.ApprovedAt
            };
        }
    }
}
