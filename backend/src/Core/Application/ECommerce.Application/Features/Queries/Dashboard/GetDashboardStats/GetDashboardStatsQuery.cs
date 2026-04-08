using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Dashboard.GetDashboardStats
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
    }
}
