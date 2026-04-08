using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Auth.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<AuthResult>
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
