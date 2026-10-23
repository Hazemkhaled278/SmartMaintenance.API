using System.ComponentModel.DataAnnotations;

namespace SmartMaintenance.API.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; } // التقييم مرتبط بطلب معين
        public Request? Request { get; set; }

        [Required]
        public int CustomerId { get; set; } // العميل اللي قيم
        public User? Customer { get; set; }

        [Required]
        public int WorkerId { get; set; } // الفني اللي اتقيم
        public User? Worker { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // من 1 إلى 5 نجوم

        public string? Comment { get; set; } // رأي العميل

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}