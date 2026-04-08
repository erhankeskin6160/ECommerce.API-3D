using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Auth.GetProfile
{
    public class GetProfileQuery : IRequest<AuthResult>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
