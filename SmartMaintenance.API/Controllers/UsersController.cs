using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMaintenance.API.Data;
using SmartMaintenance.API.DTOs;
using SmartMaintenance.API.Models;
using System.Security.Claims;

namespace SmartMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Requests).ToListAsync();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        
        //  إدارة الملف الشخصي 
        

        // 1. GET: api/users/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "غير مصرح" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "المستخدم غير موجود" });
            }

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name, // تأكد إن اسم الخاصية عندك Name أو UserName
                Email = user.Email,
                PhoneNumber = user.Phone,
                Role = user.Role
            };

            return Ok(profile);
        }

        // 2. PUT: api/users/profile
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "غير مصرح" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "المستخدم غير موجود" });
            }

            // تحديث الاسم ورقم الهاتف
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                user.Name = dto.Name;
            }

            if (dto.PhoneNumber != null)
            {
                user.Phone = dto.PhoneNumber;
            }

            // تحديث كلمة المرور إن وجدت
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || user.PasswordHash != dto.CurrentPassword)
                {
                    return BadRequest(new { message = "كلمة المرور الحالية غير صحيحة" });
                }
                user.PasswordHash = dto.NewPassword;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث الملف الشخصي بنجاح" });
        }
    }
}