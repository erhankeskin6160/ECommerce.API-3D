using MediatR;
using ECommerce.Application.DTOs;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Dashboard.GetDashboardStats
{
    public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IDashboardRepository _dashboardRepository;
        public GetDashboardStatsHandler(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardRepository.GetStatsAsync();
        }
    }
}
