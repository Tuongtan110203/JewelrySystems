using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //  [Authorize(Roles = "Admin")]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;

        public UsersController(JewelrySalesSystemDbContext dbContext, IMapper mapper, IUsersRepository usersRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.usersRepository = usersRepository;
        }

        // GET: api/Users
        [HttpGet]
        [Route("GetALL")]

        public async Task<ActionResult> GetAllUsers()
        {
            var userDomain = await usersRepository.GetAllUsersAsync();
            return Ok(mapper.Map<List<UsersDTO>>(userDomain));
        }

        // GET: api/Users/5
        [HttpGet]
        [Route("get-user-by-username/{name}")]
        public async Task<ActionResult> GetUserByName([FromRoute] string name)
        {
            var userDomain = await usersRepository.GetUserByUserNameAsync(name);
            if (userDomain == null) NotFound();
            return Ok(mapper.Map<UsersDTO>(userDomain));

        }

        [HttpGet]
        [Route("get-users-by-fullname/{name}")]
        public async Task<ActionResult> GetUsersByFullName([FromRoute] string name)
        {
            var userDomain = await usersRepository.GetUsersByFullNameAsync(name);
            if (userDomain == null) NotFound();
            return Ok(mapper.Map<List<UsersDTO>>(userDomain));

        }

        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult> GetUserByEmail(string email)
        {
            var userDomain = await usersRepository.GetUserByEmailAsync(email);
            if (userDomain == null) NotFound();
            return Ok(mapper.Map<UsersDTO>(userDomain));
        }
        [HttpGet("GetUserByPhone")]
        public async Task<ActionResult> GetUserByPhone(string phone)
        {
            var userDomain = await usersRepository.GetUserByPhoneAsync(phone);
            if (userDomain == null) NotFound();
            return Ok(mapper.Map<UsersDTO>(userDomain));
        }
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{name}")]
        public async Task<IActionResult> PutUsers(string name, UpdateUsersDTO updateUsersDTO)
        {
            var existCheck = await usersRepository.GetUserByUserNameAsync(name);
            if (existCheck == null)
            {
                return NotFound();
            }

            if (updateUsersDTO.Email != existCheck.Email)
            {
                var existingEmail = await usersRepository.GetUserByEmailAsync(updateUsersDTO.Email);
                if (existingEmail != null)
                {
                    return Conflict("Email người dùng đã tồn tại.");
                }
            }

            if (updateUsersDTO.Phone != existCheck.Phone)
            {
                var existingPhone = await usersRepository.GetUserByPhoneAsync(updateUsersDTO.Phone);
                if (existingPhone != null)
                {
                    return Conflict("Số điện thoại người dùng đã tồn tại.");
                }
            }

            var RoleExists = await dbContext.Roles.AnyAsync(c => c.RoleId == updateUsersDTO.RoleId);
            if (!RoleExists)
            {
                return Conflict("Role không tồn tại.");
            }
            var userDomainModel = mapper.Map<Users>(updateUsersDTO);
            await usersRepository.UpdateUsers(name, userDomainModel);

            return Ok(mapper.Map<UsersDTO>(userDomainModel));
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Users>> CreateUser(AddUserDTO addUserDTO)
        {
            var existingEmail = await usersRepository.GetUserByEmailAsync(addUserDTO.Email);
            if (existingEmail != null)
            {
                return Conflict("Email người dùng đã tồn tại.");
            }

            // Check if phone number already exists
            var existingPhone = await usersRepository.GetUserByPhoneAsync(addUserDTO.Phone);
            if (existingPhone != null)
            {
                return Conflict("Số điện thoại người dùng đã tồn tại.");
            }

            // Check if username already exists
            var existingUser = await usersRepository.GetUserByUserNameAsync(addUserDTO.UserName);
            if (existingUser != null)
            {
                return Conflict("Tên đăng nhập đã tồn tại.");
            }
            var RoleExists = await dbContext.Roles.AnyAsync(c => c.RoleId == addUserDTO.RoleId);
            if (!RoleExists)
            {
                return Conflict("Role không tồn tại.");
            }
            var userModelDomain = mapper.Map<Users>(addUserDTO);
            if (userModelDomain == null) { return NotFound(); }
            userModelDomain = await usersRepository.AddUsers(userModelDomain);
            return Ok(mapper.Map<UsersDTO>(userModelDomain));
        }

        // DELETE: api/Users/5
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteUsers(string name)
        {
            var checkExist = await usersRepository.DeleteUsers(name);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<UsersDTO>(checkExist));
        }


    }
}
