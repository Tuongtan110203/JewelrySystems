using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenRepository authenRepository;

        public AuthController(IConfiguration configuration, IAuthenRepository authenRepository)
        {
            _configuration = configuration;
            this.authenRepository = authenRepository;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var token = await authenRepository.Login(model);
            if (token == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(new { token });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await authenRepository.Register(registerDTO);
            if (result.StartsWith("Internal server error"))
            {
                return StatusCode(500, result);
            }
            else if (result.StartsWith("Email already exists"))
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
