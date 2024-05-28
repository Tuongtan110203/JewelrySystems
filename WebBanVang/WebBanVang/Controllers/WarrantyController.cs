using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]
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
        public async Task<IActionResult> GetAllWarranty()
        {
            var warrantyDomain = await warrantyRepository.GetAllWarranty();
            return Ok(mapper.Map<List<WarrantyDTO>>(warrantyDomain));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarrantyById(int id)
        {
            var warrantyDomain = await warrantyRepository.GetWarrantyById(id);
            if (warrantyDomain == null) 
            {
                return NotFound();
            }
            return Ok(mapper.Map<WarrantyDTO>(warrantyDomain));

        }
     

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGWarranty(int id)
        {
            var checkExist = await warrantyRepository.DeleteWarranty(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WarrantyDTO>(checkExist));
        }


        [HttpPut("{id}")]
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
    }
}
