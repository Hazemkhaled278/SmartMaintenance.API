namespace SmartMaintenance.API.DTOs
{
    public class CreatePaymentDto
    {
        public int RequestId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? ReceiptUrl { get; set; }
    }
}