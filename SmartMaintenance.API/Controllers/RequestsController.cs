using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenance.API.Models;
using SmartMaintenanceApi.Namespaces;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .Include(r => r.Worker)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Request>> CreateRequest([FromBody] CreateRequestDto dto)
        {
            var request = new Request
            {
                Description = dto.Description,
                ServiceId = dto.ServiceId,
                CustomerId = dto.CustomerId,
                WorkerId = null,
                BookingDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequests), new { id = request.Id }, request);
        }


        // PUT: api/requests/5/status (تحديث حالة الطلب)

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] UpdateRequestStatusDto dto)
        {
            var request = await _context.Requests
     .Include(r => r.Worker)
     .Include(r => r.Customer)
     .Include(r => r.Service)
     .FirstOrDefaultAsync(r => r.Id == id); 
            if (request == null)
            {
                return NotFound(new { message = "طلب الصيانة غير موجود." });
            }

            // تحديث الحالة الجديدة
            request.Status = dto.Status ?? string.Empty;
            // إذا تم إرسال WorkerId، قم بتحديثه أيضاً
            if (dto.WorkerId.HasValue)
            {
                request.WorkerId = dto.WorkerId.Value;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث الحالة بنجاح", request });
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound(new { message = "الطلب غير موجود" });
            }

            //    ( التأكد إن الطلب لسه متاح للإلغاء (مش مكتمل مثلاً)
            if (request.Status == "Completed" || request.Status == "Canceled")
            {
                return BadRequest(new { message = "لا يمكن إلغاء هذا الطلب لأنه مكتمل أو ملغي بالفعل" });
            }

            request.Status = "Canceled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إلغاء الطلب بنجاح" });
        }
    
    [HttpGet("filter")]
        [Authorize]
        public async Task<IActionResult> FilterRequests([FromQuery] string? status, [FromQuery] DateTime? date)
        {
            var query = _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Worker)
                .Include(r => r.Service)
                .AsQueryable();

            // التصفية بحالة الطلب (علشان الداشبورد )
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status.ToLower() == status.ToLower());
            }

            // التصفية بالتاريخ (علشان الداشبورد بردوا )
            if (date.HasValue)
            {
                query = query.Where(r => r.BookingDate.Date == date.Value.Date);
            }

            var requests = await query.ToListAsync();

            return Ok(requests);
        }
    }
    public class CreateRequestDto
    {
        public string Description { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public int CustomerId { get; set; }
    }
}