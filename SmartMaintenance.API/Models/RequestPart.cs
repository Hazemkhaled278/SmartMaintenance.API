namespace SmartMaintenance.API.Models
{
    public class RequestPart
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Request? Request { get; set; }
    }
}