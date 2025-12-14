using System.ComponentModel.DataAnnotations;

public class Event
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    public string Description { get; set; }

    [Required]
    public DateOnly EventDate { get; set; }

    [Required]
    public TimeOnly EventTime { get; set; }

    [Required]
    [MaxLength(150)]
    public string Venue { get; set; }

    [Required]
    public int Capacity { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; }
}
