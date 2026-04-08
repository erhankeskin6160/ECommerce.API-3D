using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace ECommerce.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        
        public AuthService(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string fullName)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return new AuthResult { Succeeded = false, ErrorMessage = "Bu e-posta adresi zaten kullanımda." };

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return new AuthResult { Succeeded = false, ErrorMessage = errors };
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            return new AuthResult 
            { 
                Succeeded = true, 
                UserId = user.Id, 
                VerificationToken = encodedToken,
                Email = user.Email
            };
        }

        public async Task<AuthResult> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthResult { Succeeded = false, ErrorMessage = "Kullanıcı bulunamadı." };

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return new AuthResult { Succeeded = true };

            return new AuthResult { Succeeded = false, ErrorMessage = "E-posta doğrulaması başarısız oldu." };
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return new AuthResult { Succeeded = false, ErrorMessage = "E-posta veya şifre hatalı." };

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthResult { Succeeded = false, ErrorMessage = "Lütfen önce e-posta adresinizi doğrulayın." };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var expiryMinutes = _config.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

            return new AuthResult
            {
                Succeeded = true,
                Token = token,
                Email = user.Email!,
                FullName = user.FullName ?? user.Email!,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
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

        public async Task<AuthResult> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new AuthResult { Succeeded = false };
            
            return new AuthResult 
            { 
                Succeeded = true, 
                FullName = user.FullName, 
                Email = user.Email 
            };
        }

        public async Task<AuthResult> UpdateProfileAsync(string userId, string fullName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new AuthResult { Succeeded = false, ErrorMessage = "Kullanıcı bulunamadı." };

            user.FullName = fullName?.Trim() ?? user.FullName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new AuthResult { Succeeded = false, ErrorMessage = "Profil güncellenemedi." };

            return new AuthResult { Succeeded = true, FullName = user.FullName, Email = user.Email };
        }

        public async Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new AuthResult { Succeeded = false, ErrorMessage = "Kullanıcı bulunamadı." };

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Şifre değiştirilemedi.";
                return new AuthResult { Succeeded = false, ErrorMessage = error };
            }

            return new AuthResult { Succeeded = true };
        }
    }
}
