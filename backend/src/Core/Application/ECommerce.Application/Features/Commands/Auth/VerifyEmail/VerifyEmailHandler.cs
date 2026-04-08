using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.VerifyEmail
{
    public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public VerifyEmailHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.VerifyEmailAsync(request.UserId, request.Token);
        }
    }
}
