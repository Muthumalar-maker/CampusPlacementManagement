namespace CampusPlacementManagement.DTOs.Companies
{
    public class CompanyResponse
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? Industry { get; set; }

        public string? Website { get; set; }

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}