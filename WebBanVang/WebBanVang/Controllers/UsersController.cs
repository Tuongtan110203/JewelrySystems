using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class UsersController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IUsersRepository usersRepository;

        public UsersController(JewelrySalesSystemDbContext dbContext, IMapper mapper,IUsersRepository usersRepository)
        {
            dbContext = dbContext;
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
        [Route("GetUserByName{name}")]
        public async Task<ActionResult> GetUsersByName([FromRoute] string name)
        {
            var userDomain = await usersRepository.GetUsersByNameAsync(name);
            if (userDomain == null) NotFound();
            return Ok(mapper.Map<UsersDTO>(userDomain));

        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{name}")]
        public async Task<IActionResult> PutUsers(string name, UpdateUsersDTO updateUsersDTO)
        {
            var existCheck = await usersRepository.GetUsersByNameAsync(name);
            if (existCheck == null)
            {
                return NotFound();
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
