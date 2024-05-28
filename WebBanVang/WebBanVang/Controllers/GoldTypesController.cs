using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]
    //[Authorize(Roles = "manager")]
    public class GoldTypesController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly IGoldTypeRepository goldTypeRepository;
        private readonly IMapper mapper;

        public GoldTypesController(JewelrySalesSystemDbContext context,IGoldTypeRepository goldTypeRepository,IMapper mapper)
        {
            _context = context;
            this.goldTypeRepository = goldTypeRepository;
            this.mapper = mapper;
        }

        // GET: api/GoldTypes
        [HttpGet]
        public async Task<IActionResult> GetAllGoldType()
        {
            var goldTypeDomain = await goldTypeRepository.GetAllGoldType();
            return Ok(mapper.Map<List<GoldTypeDTO>>(goldTypeDomain));
        }

        // GET: api/GoldTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoldTypeById(int id)
        {
            var goldTypeDomain = await goldTypeRepository.GetGoldTypeById(id);
            if (goldTypeDomain == null) NotFound();
            return Ok(mapper.Map<GoldTypeDTO>(goldTypeDomain));
           
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoldType(int id, UpdateGoldTypeDTO updateGoldTypeDTO)
        {
            var existingGoldType = await goldTypeRepository.GetGoldTypeById(id);
            if (existingGoldType == null)
            {
                return NotFound();
            }

            var goldTypeDomainModel = mapper.Map<GoldType>(updateGoldTypeDTO);
            await goldTypeRepository.UpdateGoldTYpe(id, goldTypeDomainModel);

            return Ok(mapper.Map<GoldTypeDTO>(goldTypeDomainModel));
        }

        // POST: api/GoldTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GoldType>> CreateGoldType(AddGoldTypeDTO addGoldTypeDTO)
        {
            var GoldTypeModel = mapper.Map<GoldType>(addGoldTypeDTO);
            if (GoldTypeModel == null) { return NotFound(); }
            GoldTypeModel = await goldTypeRepository.AddGoldType(GoldTypeModel);
            return Ok(mapper.Map<GoldTypeDTO>(GoldTypeModel));
        }

        // DELETE: api/GoldTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoldType(int id)
        {
            var checkExist = await goldTypeRepository.DeleteGoldType(id);
            if (checkExist == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<GoldTypeDTO>(checkExist));
        }

     
    }
}
