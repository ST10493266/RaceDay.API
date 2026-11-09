using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthSuccessResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EventDisplayDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerUrl { get; set; }
        public int TotalEnrollments { get; set; }
    }

    public class EnrolmentItemDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string EnrolmentStatus { get; set; } = "Pending";
        public string? FinishTime { get; set; }
        public int? PlacementPosition { get; set; }
        public int? TotalFieldFinishers { get; set; }
    }

    public class CreateEventFormWrapper
    {
        public EventDisplayDto EventDetails { get; set; } = new();
        public IFormFile? BannerImageFile { get; set; }
    }
}
