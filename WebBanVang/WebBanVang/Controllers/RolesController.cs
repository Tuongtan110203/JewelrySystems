using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    public class RolesController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRolesRepository rolesRepository;

        public RolesController(JewelrySalesSystemDbContext dbContext, IMapper mapper, IRolesRepository rolesRepository)
        {
            dbContext = dbContext;
            this.mapper = mapper;
            this.rolesRepository = rolesRepository;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<IActionResult> GetlAllRoles()
        {
            var roleDomain = await rolesRepository.GetAllRoles();
            return Ok(mapper.Map<List<RolesDTO>>(roleDomain));
        }
        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolesById(int id)
        {
            var roleDomain = await rolesRepository.GetRolesById(id);
            if (roleDomain == null) NotFound();
            return Ok(mapper.Map<RolesDTO>(roleDomain));

        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoles(int id, UpdateRoleDTO updateRoleDTO)
        {
            var checkExist = await rolesRepository.GetRolesById(id);
            if (checkExist == null)
            {
                return NotFound();
            }

            var roleDomain = mapper.Map<Roles>(updateRoleDTO);
            await rolesRepository.UpdateRoles(id, roleDomain);

            return Ok(mapper.Map<RolesDTO>(roleDomain));
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Roles>> CreateRoles(AddRolesDTO addRolesDTO)
        {
            var roleDomain = mapper.Map<Roles>(addRolesDTO);
            if (roleDomain == null) { return NotFound(); }
            roleDomain = await rolesRepository.AddRoles(roleDomain);
            return Ok(mapper.Map<RolesDTO>(roleDomain));
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoles(int id)
        {
            var checkExist = await rolesRepository.DeleteRoles(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<RolesDTO>(checkExist));
        }


    }
}
