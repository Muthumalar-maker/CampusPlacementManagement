namespace CampusPlacementManagement.DTOs.CompanyHRs
{
    public class CompanyHRResponse
    {
        public int CompanyHRId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}