using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.Models
{
    public class BookEventRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of seats must be at least 1")]
        public int NumberOfSeats { get; set; } = 1;
    }
}

