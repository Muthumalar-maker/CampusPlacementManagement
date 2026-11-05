using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.CompanyHRs
{
    public class UpdateCompanyHRRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;
    }
}