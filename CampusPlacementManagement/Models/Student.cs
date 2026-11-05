using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required]
        public int CollegeId { get; set; }

        public College? College { get; set; }

        [Required]
        public int PlacementOfficerId { get; set; }

        public PlacementOfficer? PlacementOfficer { get; set; }

        [Required]
        [MaxLength(30)]
        public string RegisterNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Degree { get; set; } = string.Empty;

        [Required]
        public int GraduationYear { get; set; }

        [Range(0, 10)]
        public decimal CGPA { get; set; }

        [Range(0, 100)]
        public int ActiveBacklogs { get; set; }

        [Url]
        [MaxLength(500)]
        public string? ResumeUrl { get; set; }

        public Enums.StudentStatus Status { get; set; }
            = Enums.StudentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}