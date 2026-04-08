namespace ECommerce.Application.Abstractions
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string? UserId { get; set; }
        public string? VerificationToken { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string email, string password, string fullName);
        Task<AuthResult> VerifyEmailAsync(string userId, string token);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> GetProfileAsync(string userId);
        Task<AuthResult> UpdateProfileAsync(string userId, string fullName);
        Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
