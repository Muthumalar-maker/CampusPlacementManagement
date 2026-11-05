using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.CompanyHRs
{
    public class RegisterCompanyHRRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}