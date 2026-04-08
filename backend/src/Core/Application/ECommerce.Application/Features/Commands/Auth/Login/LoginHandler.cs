using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public LoginHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.Email, request.Password);
        }
    }
}
