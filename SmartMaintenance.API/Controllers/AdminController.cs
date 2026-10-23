using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenanceApi.DTOs;

namespace SmartMaintenanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalRequests = await _context.Requests.CountAsync();
            var pendingRequests = await _context.Requests.CountAsync(r => r.Status == "Pending");
            var completedRequests = await _context.Requests.CountAsync(r => r.Status == "Completed");
            var canceledRequests = await _context.Requests.CountAsync(r => r.Status == "Canceled");

            // حساب أجمالي الأرباح من الطلبات اللي كملت للنهاية
            var totalRevenue = await _context.Requests
                .Where(r => r.Status == "Completed")
                .SumAsync(r => r.TotalAmount);

            var totalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer");
            var totalWorkers = await _context.Users.CountAsync(u => u.Role == "Worker");

            var stats = new AdminStatsDto
            {
                TotalRequests = totalRequests,
                PendingRequests = pendingRequests,
                CompletedRequests = completedRequests,
                CanceledRequests = canceledRequests,
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalWorkers = totalWorkers
            };

            return Ok(stats);
        }
    }
}