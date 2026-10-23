namespace SmartMaintenance.API.DTOs
{
    public class WorkerReviewDto
    {
        public int RequestId { get; set; } // <--- السطر الجديد المضاف
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WorkerRatingSummaryDto
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<WorkerReviewDto> Reviews { get; set; } = new();
    }
}