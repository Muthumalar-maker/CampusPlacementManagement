
using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Colleges
{
    public class CollegeResponse
    {
        public int CollegeId { get; set; }

        public string CollegeName { get; set; } = string.Empty;

        public string CollegeCode { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public CollegeStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
