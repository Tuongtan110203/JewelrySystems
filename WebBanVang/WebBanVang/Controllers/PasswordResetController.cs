using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WebBanVang.Models.Domain;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[Authorize(Roles = "Admin,Manager,Staff")]
    //[AllowAnonymous]

    public class PasswordResetController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SmtpResetPassword _smtpSettings;
        private readonly IUsersRepository _usersRepository;

        public PasswordResetController(IOptions<JwtSettings> jwtSettings, IOptions<SmtpResetPassword> smtpSettings, IUsersRepository usersRepository)
        {
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email address is required.");
            }

            Console.WriteLine($"Processing password reset for email: {model.Email}");

            var user = await _usersRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine($"No user found with email: {model.Email}");
                return BadRequest("Invalid email address.");
            }

            var token = GenerateResetToken(user.Email);

            var resetLink = $"http://kimhoanngan.shop/reset-password?token={token}";
            await SendResetEmail(model.Email, resetLink);

            return Ok(new { Token = token, Message = "Password reset link has been sent to your email." });
        }


        private string GenerateResetToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _jwtSettings.Key;

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(_jwtSettings.Key), "JWT Key cannot be null or empty");
            }

            var keyBytes = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task SendResetEmail(string email, string resetLink)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = "Password Reset",
                Body = $"Please reset your password by clicking <a href=\"{resetLink}\">here</a>.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            var principal = GetPrincipalFromToken(model.Token);
            if (principal == null)
            {
                return BadRequest("Yêu cầu đặt lại mật khẩu đã hết hạn. Vui lòng thử lại");
            }

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return BadRequest("Invalid token.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            await UpdatePasswordAsync(email, model.NewPassword);
            return Ok("Password has been reset.");
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return validatedToken is JwtSecurityToken ? principal : null;
            }
            catch
            {
                return null;
            }
        }


        private async Task UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _usersRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Password = newPassword;

            await _usersRepository.UpdateUsers(user.UserName, user);
        }

    }
}

