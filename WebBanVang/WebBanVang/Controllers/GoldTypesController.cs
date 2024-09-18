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
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[Authorize(Roles = "manager")]
    //[AllowAnonymous]
    public class GoldTypesController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly IGoldTypeRepository goldTypeRepository;
        private readonly IMapper mapper;

        public GoldTypesController(JewelrySalesSystemDbContext context, IGoldTypeRepository goldTypeRepository, IMapper mapper)
        {
            _context = context;
            this.goldTypeRepository = goldTypeRepository;
            this.mapper = mapper;
        }

        // GET: api/GoldTypes
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllGoldType()
        {
            var goldTypeDomain = await goldTypeRepository.GetAllGoldType();
            return Ok(mapper.Map<List<GoldTypeDTO>>(goldTypeDomain));
        }

        // GET: api/GoldTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetGoldTypeById(int id)
        {
            var goldTypeDomain = await goldTypeRepository.GetGoldTypeById(id);
            if (goldTypeDomain == null) return NotFound();
            return Ok(mapper.Map<GoldTypeDTO>(goldTypeDomain));
        }

        [HttpGet("get-gold-type-by-name/{name}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetGoldTypeByName(string name)
        {
            var goldTypeDomain = await goldTypeRepository.GetGoldTypeByNameAsync(name);
            if (goldTypeDomain == null) return NotFound();
            return Ok(mapper.Map<List<GoldTypeDTO>>(goldTypeDomain));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateGoldType(int id, UpdateGoldTypeDTO updateGoldTypeDTO)
        {
            var existingGoldType = await goldTypeRepository.GetGoldTypeById(id);
            if (existingGoldType == null)
            {
                return NotFound();
            }

            if (await IsDuplicateGoldCode(updateGoldTypeDTO.GoldCode, id))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mã loại vàng đã tồn tại");
            }

            var goldTypeDomainModel = mapper.Map<GoldType>(updateGoldTypeDTO);
            await goldTypeRepository.UpdateGoldTYpe(id, goldTypeDomainModel);

            return Ok(mapper.Map<GoldTypeDTO>(goldTypeDomainModel));
        }

        // POST: api/GoldTypes
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GoldType>> CreateGoldType(AddGoldTypeDTO addGoldTypeDTO)
        {
            if (await IsDuplicateGoldCode(addGoldTypeDTO.GoldCode))
            {
                return BadRequest("Mã loại vàng đã tồn tại");
            }

            var GoldTypeModel = mapper.Map<GoldType>(addGoldTypeDTO);
            if (GoldTypeModel == null) { return NotFound(); }
            GoldTypeModel = await goldTypeRepository.AddGoldType(GoldTypeModel);
            return Ok(mapper.Map<GoldTypeDTO>(GoldTypeModel));
        }

        // DELETE: api/GoldTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteGoldType(int id)
        {
            try
            {
                var checkExist = await goldTypeRepository.DeleteGoldType(id);
                if (checkExist == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<GoldTypeDTO>(checkExist));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Kiểm tra trùng lặp mã loại vàng
        private async Task<bool> IsDuplicateGoldCode(string goldCode, int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.GoldTypes
                    .AnyAsync(x => x.GoldId != id.Value && x.GoldCode == goldCode);
            }
            else
            {
                return await _context.GoldTypes
                    .AnyAsync(x => x.GoldCode == goldCode);
            }
        }

        [HttpGet("get-percentages-goldtype/{option}")]
        public async Task<ActionResult<List<GoldTypePercentageDTO>>> GetGoldTypePercentages(string option)
        {
            /*var percentages = await goldTypeRepository.CalculateGoldTypePercentagesAsync();
            return Ok(percentages);*/
            if (option == "today")
            {
                var percentages = await goldTypeRepository.GetGoldCodePercentagesForToday();
                return Ok(percentages);
            }
            else if (option == "this-week")
            {
                var percentages = await goldTypeRepository.GetGoldCodePercentagesForThisWeek();
                return Ok(percentages);
            }
            else if (option == "this-month")
            {
                var percentages = await goldTypeRepository.GetGoldCodePercentagesForThisMonth();
                return Ok(percentages);
            }
            else if (option == "this-year")
            {
                var percentages = await goldTypeRepository.GetGoldCodePercentagesForThisYear();
                return Ok(percentages);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }

        }
    }
}
