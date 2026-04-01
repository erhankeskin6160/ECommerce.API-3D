using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.API.DTOs
{
    public class CreateReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Değerlendirme 1 ile 5 arasında olmalıdır.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public IFormFile? Image { get; set; }
    }
}
