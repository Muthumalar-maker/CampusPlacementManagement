
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Colleges
{
    public class RegisterCollegeRequest
    {
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
    }
}
