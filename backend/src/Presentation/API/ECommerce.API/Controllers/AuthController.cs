using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerce.Application.DTOs;
using ECommerce.Application.Features.Commands.Auth.Register;
using ECommerce.Application.Features.Commands.Auth.Login;
using ECommerce.Application.Features.Commands.Auth.VerifyEmail;
using ECommerce.Application.Features.Commands.Auth.UpdateProfile;
using ECommerce.Application.Features.Commands.Auth.ChangePassword;
using ECommerce.Application.Features.Queries.Auth.GetProfile;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _mediator.Send(new RegisterCommand 
            { 
                Email = dto.Email, 
                Password = dto.Password, 
                FullName = dto.FullName 
            });

            if (!result.Succeeded) return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Kayıt başarılı. Lütfen e-postanızı kontrol ederek hesabınızı doğrulayın." });
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Geçersiz doğrulama isteği." });

            var result = await _mediator.Send(new VerifyEmailCommand { UserId = userId, Token = token });
            if (result.Succeeded)
                return Ok(new { message = "E-posta adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz." });

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _mediator.Send(new LoginCommand { Email = dto.Email, Password = dto.Password });

            if (!result.Succeeded) return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new AuthResponseDto
            {
                Token = result.Token!,
                Email = result.Email!,
                FullName = result.FullName!,
                ExpiresAt = result.ExpiresAt
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new GetProfileQuery { UserId = userId });
            if (!result.Succeeded) return NotFound();

            return Ok(new { FullName = result.FullName, email = result.Email });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new UpdateProfileCommand { UserId = userId, FullName = dto.FullName });
            if (!result.Succeeded) return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Profiliniz başarıyla güncellendi.", FullName = result.FullName, email = result.Email });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new ChangePasswordCommand 
            { 
                UserId = userId, 
                CurrentPassword = dto.CurrentPassword, 
                NewPassword = dto.NewPassword 
            });

            if (!result.Succeeded) return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Şifreniz başarıyla değiştirildi." });
        }
    }
}
