using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        private static string? _resetToken;
        private static DateTime? _resetTokenExpiry;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public class LoginDto
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Kullanıcı adı ve şifre gereklidir.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());
            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var token = _configuration["JWT_SECRET"] ?? "super_secret_development_token_key_123456";
            return Ok(new { success = true, token = token });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true });
        }

        // GET: api/auth/status
        [Authorize]
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { isAuthenticated = true });
        }

        public class ForgotPasswordDto
        {
            public string Email { get; set; } = "";
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            _resetToken = Guid.NewGuid().ToString("N");
            _resetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            var origin = Request.Headers["Origin"].ToString();
            if (string.IsNullOrEmpty(origin)) origin = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(origin)) origin = "http://localhost:4200"; // fallback

            origin = origin.TrimEnd('/');
            var resetLink = $"{origin}/reset-password?token={_resetToken}";

            var subject = "Portfolio Admin Panel - Şifre Sıfırlama Talebi";
            var body = $"Merhaba,\n\nAdmin paneli şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:\n\n{resetLink}\n\nBu bağlantı 15 dakika boyunca geçerlidir.\n\nEğer bu talebi siz göndermediyseniz lütfen bu e-postayı dikkate almayın.";

            await _emailService.SendEmailAsync(subject, body);

            return Ok(new { success = true });
        }

        public class ResetPasswordDto
        {
            public string Token { get; set; } = "";
            public string NewPassword { get; set; } = "";
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || 
                dto.Token != _resetToken || 
                !_resetTokenExpiry.HasValue || 
                _resetTokenExpiry.Value < DateTime.UtcNow)
            {
                return BadRequest("Geçersiz veya süresi dolmuş şifre sıfırlama anahtarı.");
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            {
                return BadRequest("Şifre en az 6 karakter olmalıdır.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == "admin");
            if (user == null)
            {
                return NotFound("Admin kullanıcısı bulunamadı.");
            }

            user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Clear token
            _resetToken = null;
            _resetTokenExpiry = null;

            return Ok(new { success = true });
        }

        public class UpdateAccountDto
        {
            public string Username { get; set; } = "";
            public string? CurrentPassword { get; set; }
            public string? NewPassword { get; set; }
        }

        // GET: api/auth/account
        [Authorize]
        [HttpGet("account")]
        public async Task<IActionResult> GetAccount()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null) return NotFound("Kullanıcı bulunamadı.");
            return Ok(new { username = user.Username });
        }

        // PUT: api/auth/account
        [Authorize]
        [HttpPut("account")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return BadRequest("Kullanıcı adı boş olamaz.");
            }

            user.Username = dto.Username.Trim();

            // If password change is requested
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                {
                    return BadRequest("Şifrenizi değiştirmek için mevcut şifrenizi girmeniz gerekmektedir.");
                }

                if (!PasswordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest("Mevcut şifreniz yanlış.");
                }

                if (dto.NewPassword.Length < 6)
                {
                    return BadRequest("Yeni şifre en az 6 karakter olmalıdır.");
                }

                user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
