using SmartMaintenance.API.Models;

public class Request
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }

    public int? WorkerId { get; set; }
    public User? Worker { get; set; }

    public int CustomerId { get; set; }
    public User? Customer { get; set; }

    public int ServiceId { get; set; }
    public Service? Service { get; set; }
}