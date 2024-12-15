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
    //[AllowAnonymous]
    public class WarrantyController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IWarrantyRepository warrantyRepository;

        public WarrantyController(JewelrySalesSystemDbContext dbContext, IMapper mapper, IWarrantyRepository warrantyRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.warrantyRepository = warrantyRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllWarranty()
        {
            var warrantyDomain = await warrantyRepository.GetAllWarranty();
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetWarrantyById(int id)
        {
            var warrantyDomain = await warrantyRepository.GetWarrantyById(id);
            if (warrantyDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WarrantyDTO>(warrantyDomain));

        }


        [HttpGet("get-warranties-by-id{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetWarrantiesById(int id)
        {
            var warrantyDomain = await warrantyRepository.GetWarrantiesByIdAsync(id);
            if (warrantyDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));

        }
        [HttpGet("get-warranties-by-product-code")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetWarrantyByProductCode(string code)
        {
            var warrantyDomain = await warrantyRepository.GetWarrantyByProductCode(code);
            if (warrantyDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteGWarranty(int id)
        {
            var checkExist = await warrantyRepository.DeleteWarranty(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WarrantyDTO>(checkExist));
        }

        [HttpGet("searchWarrantyByCode")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> SearchWarrantyByCode([FromQuery] string code)
        {
            var warrantyDomain = await warrantyRepository.GetWarrantyByCodeAsync(code);
            if (warrantyDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateWarranty(int id, UpdateWarrantyDTO updateWarrantyDTO)
        {


            var checkExistWarranty = await warrantyRepository.GetWarrantyById(id);
            if (checkExistWarranty == null)
            {
                return NotFound();
            }

            var warrantyDomainModel = mapper.Map<Warranty>(updateWarrantyDTO);
            await warrantyRepository.UpdateWarranty(id, warrantyDomainModel);

            return Ok(mapper.Map<WarrantyDTO>(warrantyDomainModel));
        }

        [HttpGet("GetOutOfDateWarranty")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOutOfDateWarranty()
        {
            var warrantyDomain = await warrantyRepository.GetOutOfDateWarrantyAsync();
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));
        }
    }
}
