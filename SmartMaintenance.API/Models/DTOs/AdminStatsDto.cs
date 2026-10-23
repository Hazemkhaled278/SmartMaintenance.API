namespace SmartMaintenanceApi.DTOs
{
    public class AdminStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CanceledRequests { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalWorkers { get; set; }
    }
}