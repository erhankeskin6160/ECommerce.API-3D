using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public ChangePasswordHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
        }
    }
}
