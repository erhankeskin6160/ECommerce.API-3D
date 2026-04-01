using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using ECommerce.API.Models;
using ECommerce.API.DTOs;
using ECommerce.API.Services;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Bu e-posta adresi zaten kullanımda." });

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errors) });
            }

            // Generate Email Confirmation Token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            // Encode token for URL
            var encodedToken = HttpUtility.UrlEncode(token);
            
            // Assuming frontend URL for verification is http://localhost:4200/verify-email
            var frontendUrl = "http://localhost:4200/verify-email";
            var verificationLink = $"{frontendUrl}?userId={user.Id}&token={encodedToken}";

            var emailBody = $@"
                <h3>E-Ticaret Sitemize Hoş Geldiniz!</h3>
                <p>Hesabınızı doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                <a href='{verificationLink}'>Hesabımı Doğrula</a>
                <br/><br/>
                <p>Eğer bu hesabı siz oluşturmadıysanız, bu e-postayı dikkate almayabilirsiniz.</p>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "E-Ticaret Kayıt Doğrulaması", emailBody);
            }
            catch (Exception ex)
            {
                // In production, we should log this properly.
                Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
            }

            return Ok(new { message = "Kayıt başarılı. Lütfen e-postanızı kontrol ederek hesabınızı doğrulayın." });
        }

        // GET: api/auth/verify-email
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Geçersiz doğrulama isteği." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest(new { message = "Kullanıcı bulunamadı." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if (result.Succeeded)
                return Ok(new { message = "E-posta adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz." });

            return BadRequest(new { message = "E-posta doğrulaması başarısız oldu." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });

            // Check if email is confirmed before allowing login
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new { message = "Lütfen önce e-posta adresinizi doğrulayın." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var expiryMinutes = _config.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName ?? user.Email!,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            });
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes", 60);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("fullName", user.FullName ?? user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound();
            return Ok(new { user.FullName, email = user.Email });
        }

        // PUT: api/auth/profile
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound();

            user.FullName = dto.FullName?.Trim() ?? user.FullName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Profil güncellenemedi." });

            return Ok(new { message = "Profiliniz başarıyla güncellendi.", user.FullName, email = user.Email });
        }

        // PUT: api/auth/change-password
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Şifre değiştirilemedi.";
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Şifreniz başarıyla değiştirildi." });
        }
    }
}
