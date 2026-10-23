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
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/payments
        // هنعمل عملية دفع جديدة و معاها الايصال الدفع
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var requestExists = await _context.Requests.AnyAsync(r => r.Id == dto.RequestId);
            if (!requestExists)
            {
                return NotFound(new { message = "طلب الصيانة غير موجود." });
            }

            var payment = new Payment
            {
                RequestId = dto.RequestId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                ReceiptUrl = dto.ReceiptUrl,
                PaymentDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تسجيل عملية الدفع وإرفاق الإيصال بنجاح بانتظار التأكيد.", paymentId = payment.Id });
        }

        // GET: api/payments
        // هات جميع عمليات الدفع (للأدمن)
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Request)
                .ToListAsync();

            return Ok(payments);
        }

        // GET: api/payments/request/{requestId}
        // هات المدفوعات الخاصة بطلب صيانة معين
        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetPaymentsByRequestId(int requestId)
        {
            var payments = await _context.Payments
                .Where(p => p.RequestId == requestId)
                .ToListAsync();

            return Ok(payments);
        }

        // PUT: api/payments/{id}/status
        // تعديل حالة الدفع (تأكيد/قبول الإيصال أو رفضه)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string status)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "سجل الدفع غير موجود." });
            }

            payment.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث حالة الدفع بنجاح.", status = payment.Status });
        }
    }
}