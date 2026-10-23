using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Models;

namespace SmartMaintenance.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestPart> RequestParts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. حل مشكلة Cascade Delete
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.Requests)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Worker)
                .WithMany()
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Service)
                .WithMany()
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // حل مشكلة Cascade Delete لجدول التقييمات
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Worker)
                .WithMany()
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Data Seeding (مع الأسعار الجديدة للخدمات)
            modelBuilder.Entity<User>().HasData(
    new User { Id = 1, Name = "Hazem Mohamed", Email = "hazem@example.com", PasswordHash = "pass123", Role = "Customer", Phone = "01032039680" }
);
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "صيانة تكييف سبليت", Description = "تنظيف وغسيل وفحص فريون", Price = 350.00m, IsActive = true },
                new Service { Id = 2, Name = "اصلاح سباكة منزلية", Description = "كشف تسريبات وتغيير خلاطات", Price = 200.00m, IsActive = true },
                new Service { Id = 3, Name = "تركيب وتوصيل كهرباء", Description = "تركيب مفاتيح وإضاءة ولوحات", Price = 250.00m, IsActive = true }
            );

            modelBuilder.Entity<Worker>().HasData(
                new Worker { Id = 1, Name = "أحمد السباك", Phone = "01111111111", Specialization = "سباكة", IsAvailable = true, ServiceId = 2 },
                new Worker { Id = 2, Name = "محمد الكهربائي", Phone = "01222222222", Specialization = "كهرباء", IsAvailable = true, ServiceId = 3 }
            );
        }
    }
}