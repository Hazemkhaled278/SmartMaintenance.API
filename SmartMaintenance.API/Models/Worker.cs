namespace SmartMaintenance.API.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        // علاقة مع جدول الـ Service
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}