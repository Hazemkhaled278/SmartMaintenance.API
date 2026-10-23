using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenance.API.Models;
using SmartMaintenance.API.DTOs; 

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] WorkerReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = await _context.Requests.FindAsync(reviewDto.RequestId);
            if (request == null)
                return NotFound(new { message = "طلب الصيانة غير موجود." });

            // 2. نتأكد من أن الطلب  له فني فعلاً قبل التقييم
            if (request.WorkerId == null || request.WorkerId == 0)
            {
                return BadRequest(new { message = "لا يمكن تقييم طلب صيانة لم يتم إسناده لفني بعد." });
            }

            // 3. إنشاء التقييم مستخدماً IDs الطلب الحقيقية بكل دقة
            var review = new Review
            {
                RequestId = reviewDto.RequestId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow,
                CustomerId = request.CustomerId,
                WorkerId = request.WorkerId.Value
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إضافة التقييم بنجاح.", reviewId = review.Id });
        }
    }
}