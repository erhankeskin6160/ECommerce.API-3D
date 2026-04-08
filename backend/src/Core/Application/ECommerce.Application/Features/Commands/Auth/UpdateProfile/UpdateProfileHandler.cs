using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.UpdateProfile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public UpdateProfileHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            return await _authService.UpdateProfileAsync(request.UserId, request.FullName);
        }
    }
}
