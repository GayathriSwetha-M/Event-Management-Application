namespace EventManagement.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        // Keep Token for backward compatibility (maps to AccessToken)
        public string Token 
        { 
            get => AccessToken; 
            set => AccessToken = value; 
        }
    }

    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class EventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public TimeOnly EventTime { get; set; }
        public string Venue { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSlots { get; set; }
    }

    public class BookingResponse
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public TimeOnly EventTime { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int NumberOfSeats { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardOverviewResponse
    {
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
    }
}

