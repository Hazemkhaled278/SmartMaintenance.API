using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenance.API.Models;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestPartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestPartsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/requestparts (إضافة قطعة غيار لطلب معين وتحديث الإجمالي)

        [HttpPost]
        public async Task<ActionResult<RequestPart>> AddPart(RequestPart part)
        {
            var request = await _context.Requests.FindAsync(part.RequestId);
            if (request == null)
            {
                return NotFound("طلب الصيانة غير موجود!");
            }

            // إضافة قطعة الغيار
            _context.RequestParts.Add(part);

            // زيادة سعر قطعة الغيار على السعر الإجمالي للطلب أوتوماتيك
            request.TotalAmount += part.Price;

            await _context.SaveChangesAsync();

            return Ok(part);
        }

        // GET: api/requestparts/request/1 (عرض كل قطع الغيار لطلب محدد)
        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<RequestPart>>> GetPartsByRequestId(int requestId)
        {
            return await _context.RequestParts
                .Where(p => p.RequestId == requestId)
                .ToListAsync();
        }
    }
}