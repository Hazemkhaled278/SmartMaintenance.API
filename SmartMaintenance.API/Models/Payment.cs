namespace SmartMaintenance.API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // e.g., Vodafone Cash, Bank Transfer, Cash
        public string? ReceiptUrl { get; set; }                   // رابط إيصال التحويل
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";           // Pending, Approved, Rejected

        // Navigation Property
        public Request? Request { get; set; }
    }
}