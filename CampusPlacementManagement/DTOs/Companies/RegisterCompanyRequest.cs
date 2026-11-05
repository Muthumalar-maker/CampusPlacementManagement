using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Companies
{
    public class RegisterCompanyRequest
    {
        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Industry { get; set; }

        [Url]
        [MaxLength(250)]
        public string? Website { get; set; }

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
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}