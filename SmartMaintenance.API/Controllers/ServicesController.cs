using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data; // غير اسم الـ Namespace حسب مشروعك
using SmartMaintenance.API.Models;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Services (عرض كل الخدمات)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        // 2. GET: api/Services/5 (عرض خدمة معينة برقم الـ Id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new { message = "الخدمة غير موجودة" });
            }

            return service;
        }

        // 3. POST: api/Services (إضافة خدمة جديدة)
        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // مسموح للأدمن فقط
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service updatedService)
        {
            if (id != updatedService.Id)
            {
                return BadRequest(new { message = "معرف الخدمة غير مطابق" });
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new { message = "الخدمة غير موجودة" });
            }

            // تحديث البيانات
            service.Name = updatedService.Name;
            service.Description = updatedService.Description;
            service.Price = updatedService.Price;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث بيانات الخدمة بنجاح", service });
        }
    }
}