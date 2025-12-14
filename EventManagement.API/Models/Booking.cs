using System.ComponentModel.DataAnnotations;

public class Booking
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public string Status { get; set; } // booked / cancelled

    [Required]
    public int NumberOfSeats { get; set; } = 1; // Number of seats booked

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public Event Event { get; set; }
}
