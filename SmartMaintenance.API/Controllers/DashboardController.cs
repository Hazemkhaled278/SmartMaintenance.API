using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //يعني للادمن بس
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // 1. داشبورد تحليل الفني و غيره
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalRequests = await _context.Requests.CountAsync();
            var completedRequests = await _context.Requests.CountAsync(r => r.Status == "Completed");
            var pendingRequests = await _context.Requests.CountAsync(r => r.Status == "Pending");
            var inProgressRequests = await _context.Requests.CountAsync(r => r.Status == "In Progress");

            // إجمالي الأرباح من المدفوعات المسجلة
            var totalRevenue = await _context.Payments.SumAsync(p => p.Amount);

            var totalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer");
            var totalWorkers = await _context.Users.CountAsync(u => u.Role == "Worker");

            return Ok(new
            {
                TotalRequests = totalRequests,
                CompletedRequests = completedRequests,
                PendingRequests = pendingRequests,
                InProgressRequests = inProgressRequests,
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalWorkers = totalWorkers
            });
        }

        // 2. أكتر الخدمات المطلوبة
        [HttpGet("top-services")]
        public async Task<IActionResult> GetTopServices()
        {
            var topServices = await _context.Requests
                .GroupBy(r => r.Service.Name)
                .Select(g => new
                {
                    ServiceName = g.Key,
                    RequestCount = g.Count()
                })
                .OrderByDescending(s => s.RequestCount)
                .Take(5)
                .ToListAsync();

            return Ok(topServices);
        }

        // 3. أحدث الطلبات لجدول اللوحة
        [HttpGet("recent-requests")]
        public async Task<IActionResult> GetRecentRequests()
        {
            var recentRequests = await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .OrderByDescending(r => r.BookingDate)
                .Take(5)
                .Select(r => new
                {
                    r.Id,
                    CustomerName = r.Customer.Name,
                    ServiceName = r.Service.Name,
                    r.Status,
                    r.TotalAmount,
                    r.BookingDate
                })
                .ToListAsync();

            return Ok(recentRequests);
        }
    }
}