using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.Application.Features.Queries.Dashboard.GetDashboardStats;
using ECommerce.Application.Features.Queries.Dashboard.GetLowStockProducts;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            var result = await _mediator.Send(new GetLowStockProductsQuery());
            return Ok(result);
        }
    }
}
