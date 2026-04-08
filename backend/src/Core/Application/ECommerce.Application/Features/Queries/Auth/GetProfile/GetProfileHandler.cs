using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Auth.GetProfile
{
    public class GetProfileHandler : IRequestHandler<GetProfileQuery, AuthResult>
    {
        private readonly IAuthService _authService;

        public GetProfileHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            return await _authService.GetProfileAsync(request.UserId);
        }
    }
}
