using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenance.API.DTOs;
using SmartMaintenance.API.Models;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Workers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
        {
            return await _context.Workers.Include(w => w.Service).ToListAsync();
        }

        // GET: api/Workers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Worker>> GetWorker(int id)
        {
            var worker = await _context.Workers.Include(w => w.Service).FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null)
            {
                return NotFound();
            }

            return worker;
        }

        // POST: api/Workers
        [HttpPost]
        public async Task<ActionResult<Worker>> PostWorker(Worker worker)
        {
            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorker", new { id = worker.Id }, worker);
        }

        // GET: api/workers/browse?serviceId=1&sortBy=rating
        [HttpGet("browse")]
        [AllowAnonymous] // أو [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> BrowseWorkers([FromQuery] int? serviceId, [FromQuery] string? sortBy)
        {
            var query = _context.Users
                .Where(u => u.Role == "Worker")
                .AsQueryable();

            // هنجيب البيانات مع حساب avg  التقييمات وعدد التقييمات لكل فني
            var workers = await query.Select(w => new
            {
                WorkerId = w.Id,
                WorkerName = w.Name,
                WorkerEmail = w.Email,
                // حساب avg  التقييم للفني
                AverageRating = _context.Reviews.Where(r => r.WorkerId == w.Id).Any()
                                ? _context.Reviews.Where(r => r.WorkerId == w.Id).Average(r => r.Rating)
                                : 0,
                TotalReviews = _context.Reviews.Count(r => r.WorkerId == w.Id),
                // خدمات الفني من خلال الطلبات اللي نفذها
                Services = _context.Requests
                            .Where(r => r.WorkerId == w.Id)
                            .Select(r => r.Service.Name)
                            .Distinct()
                            .ToList()
            }).ToListAsync();

            // فلترة حسب الخدمة لو العميل اختار خدمة معينة
            if (serviceId.HasValue)
            {
                var serviceName = await _context.Services
                    .Where(s => s.Id == serviceId.Value)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                if (serviceName != null)
                {
                    workers = workers.Where(w => w.Services.Contains(serviceName)).ToList();
                }
            }

            // الترتيب حسب الأعلى تقييماً
            if (sortBy?.ToLower() == "rating")
            {
                workers = workers.OrderByDescending(w => w.AverageRating).ToList();
            }

            return Ok(workers);
        }

        // POST: api/workers/rate (إضافة تقييم للفني)
        [HttpPost("rate")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddRating([FromBody] Review review)
        {
            // التأكد إن الطلب موجود ومكتمل
            var request = await _context.Requests.FindAsync(review.RequestId);
            if (request == null)
                return NotFound("الطلب غير موجود");

            if (request.Status != "Completed")
            {
                return BadRequest("لا يمكنك تقييم الفني إلا بعد إتمام الخدمة");
            }

            review.CustomerId = request.CustomerId;
            review.WorkerId = request.WorkerId.Value;
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إرسال التقييم بنجاح" });
        }
    
    [HttpGet("{workerId}/reviews")]
        public async Task<IActionResult> GetWorkerReviews(int workerId)
        {
            var worker = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == workerId && u.Role == "Worker");

            if (worker == null)
            {
                return NotFound(new { message = "الفني غير موجود" });
            }

            // جلب التقييمات الخاصة بالفني ده
            var reviews = await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.WorkerId == workerId)
                .ToListAsync();

            var totalReviews = reviews.Count;
            var averageRating = totalReviews > 0 ? reviews.Average(r => r.Rating) : 0;

            var result = new WorkerRatingSummaryDto
            {
                WorkerId = worker.Id,
                WorkerName = worker.Name,
                AverageRating = Math.Round(averageRating, 1), // تقريب لرقم عشري واحد (مثلاً 4.5)
                TotalReviews = totalReviews,
                Reviews = reviews.Select(r => new WorkerReviewDto
                {
                    CustomerName = r.Customer != null ? r.Customer.Name : "عميل",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };

            return Ok(result);
        }
    }
}