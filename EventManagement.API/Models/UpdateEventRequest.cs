using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.Models
{
    public class UpdateEventRequest
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateOnly EventDate { get; set; }

        [Required]
        public TimeOnly EventTime { get; set; }

        [Required]
        [MaxLength(150)]
        public string Venue { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}

