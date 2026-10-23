namespace SmartMaintenance.API.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       // اسم الخدمة (مثل: تكييف، سباكة)
        public string? Description { get; set; }              // وصف تفصيلي للخدمة
        public bool IsActive { get; set; } = true;            // هل الخدمة متاحة حالياً أم لا
        public decimal Price { get; set; }

        // العلاقات: الخدمة الواحدة يقدمها أكثر من فني (Worker)
        public ICollection<Worker> Workers { get; set; } = new List<Worker>();
    }
}