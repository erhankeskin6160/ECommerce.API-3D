using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using ECommerce.Application.DTOs;
using ECommerce.Application.Features.Queries.Products.GetProducts;
using ECommerce.Application.Features.Queries.Products.GetCategories;
using ECommerce.Application.Features.Queries.Products.GetProductById;
using ECommerce.Application.Features.Commands.ProductC.CreateProduct;
using ECommerce.Application.Features.Commands.Products.UpdateProduct;
using ECommerce.Application.Features.Commands.Products.DeleteProduct;
using ECommerce.Application.Features.Queries.Products.GetProductReviews;
using ECommerce.Application.Features.Commands.Products.AddReview;
using ECommerce.Domain.Entities;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? search, [FromQuery] string? category)
        {
            var response = await _mediator.Send(new GetProductsQuery(search, category));
            return Ok(response.Products);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var response = await _mediator.Send(new GetCategoriesQuery());
            return Ok(response.Categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var response = await _mediator.Send(new GetProductByIdQuery(id));
            if (response.Product == null) return NotFound();
            return response.Product;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductRequest request)
        {
            var response = await _mediator.Send(request);
            if (!response.IsSuccess) return BadRequest(response.Message);
            return CreatedAtAction(nameof(GetProduct), new { id = response.ProductId }, new { id = response.ProductId, message = response.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest();
            var response = await _mediator.Send(command);
            if (!response.IsSuccess) return NotFound(response.Message);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _mediator.Send(new DeleteProductCommand { Id = id });
            if (!response.IsSuccess) return NotFound(response.Message);
            return NoContent();
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetProductReviews(int id)
        {
            var response = await _mediator.Send(new GetProductReviewsQuery(id));
            return Ok(response.Reviews);
        }

        [Authorize]
        [HttpPost("{id}/reviews")]
        public async Task<ActionResult<ReviewDto>> PostProductReview(int id, [FromForm] CreateReviewDto reviewDto, IFormFile? image = null)
        {
            if (id != reviewDto.ProductId) return BadRequest("Ürün ID uyumsuz.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            string? imageUrl = null;
            if (image != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "reviews");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                imageUrl = $"/uploads/reviews/{uniqueFileName}";
            }

            var command = new AddReviewCommand
            {
                ProductId = id,
                UserId = userId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                ImageUrl = imageUrl
            };

            var response = await _mediator.Send(command);
            if (!response.IsSuccess) return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetProductReviews), new { id = response.Review?.ProductId }, response.Review);
        }
    }
}
