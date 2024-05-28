using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Repository
{
    public class SQLAuthenRepository : IAuthenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly JewelrySalesSystemDbContext _context;

        public SQLAuthenRepository(IConfiguration configuration, JewelrySalesSystemDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<string> Login(Login model)
        {
            var user = await _context.Users.Include(u => u.Roles)
                            .SingleOrDefaultAsync(u => u.UserName.Equals(model.UserName) && u.Password == model.Password);

            if (user == null)
            {
                return null; 
            }
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
            new Claim(ClaimTypes.Role, user.Roles.RoleName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(300),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> Register(RegisterDTO registerDTO)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == registerDTO.UserName);
                if (existingUser != null)
                {
                    return "UserName already exists";
                }

                var newUser = new Users
                {
                    UserName = registerDTO.UserName,
                    FullName = registerDTO.FullName,
                    Password = registerDTO.Password,
                    Address = registerDTO.Address,
                    Phone = registerDTO.Phone,
                    Email = registerDTO.Email,
                    Dob = registerDTO.Dob,
                    Level = registerDTO.Level,
                    Status = "active",
                    RoleId = registerDTO.RoleId
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return "User registered successfully";
            }
            catch (Exception ex)
            {
                return $"Internal server error: {ex.Message}";
            }
        }
    }
 }

