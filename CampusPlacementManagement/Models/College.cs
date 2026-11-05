
using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Enums;


namespace CampusPlacementManagement.Models
{
    public class College
    {
        public int CollegeId { get; set; }

        [Required]
        [MaxLength(150)]
        public string CollegeName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string CollegeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public CollegeStatus Status { get; set; } = CollegeStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
