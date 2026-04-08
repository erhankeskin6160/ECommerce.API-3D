using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public RegisterHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, request.FullName);
            
            if (result.Succeeded && !string.IsNullOrEmpty(result.VerificationToken))
            {
                var frontendUrl = "http://localhost:4200/verify-email";
                var verificationLink = $"{frontendUrl}?userId={result.UserId}&token={result.VerificationToken}";

                var emailBody = $@"
                    <h3>E-Ticaret Sitemize Hoş Geldiniz!</h3>
                    <p>Hesabınızı doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                    <a href='{verificationLink}'>Hesabımı Doğrula</a>
                    <br/><br/>
                    <p>Eğer bu hesabı siz oluşturmadıysanız, bu e-postayı dikkate almayabilirsiniz.</p>";

                try
                {
                    await _emailService.SendEmailAsync(request.Email, "E-Ticaret Kayıt Doğrulaması", emailBody);
                }
                catch
                {
                    // Log error safely in production
                }
            }

            return result;
        }
    }
}
