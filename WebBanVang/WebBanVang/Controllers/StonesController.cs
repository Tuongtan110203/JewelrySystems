using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors("AllowLocalhost3000")]
    public class StonesController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly IStoneRepository stoneRepository;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public StonesController(JewelrySalesSystemDbContext context, IStoneRepository stoneRepository, IMapper mapper, IProductRepository productRepository)
        {
            _context = context;
            this.stoneRepository = stoneRepository;
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        // GET: api/Stones
        [HttpGet]
        public async Task<IActionResult> GetStones()
        {
            var stones = await stoneRepository.GetAllStonesAsync();
            return Ok(mapper.Map<List<StoneDTO>>(stones));
        }

        // GET: api/Stones/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStone(int id)
        {
            var stone = await stoneRepository.GetStoneByIdAsync(id);
            if (stone == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<StoneDTO>(stone));
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStone(int id, UpdateStoneDTO updateStoneDTO)
        {
            var stone = mapper.Map<Stone>(updateStoneDTO);

            stone = await stoneRepository.UpdateStoneAsync(id, stone);

            if (stone == null)
            {
                return NotFound();
            }

            var stoneDto = mapper.Map<StoneDTO>(stone);
            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes)
                                        .FirstOrDefaultAsync(p => p.ProductId == stone.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var stonePrice = product.Stones.Sum(s => s.Price);

            if (product.GoldTypes == null)
            {
                return BadRequest("Gold type information is missing for the product.");
            }

            product.Price = ((product.GoldWeight * product.GoldTypes.SellPrice) + product.Wage + stonePrice) * product.PriceRatio;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(stoneDto);
        }


        // POST: api/Stones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stone>> PostStone(AddStoneDTO addStoneDTO)
        {
            var stone = mapper.Map<Stone>(addStoneDTO);
            if (stone == null)
            {
                return BadRequest("Invalid stone data.");
            }

            stone = await stoneRepository.CreateAsync(stone);


            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes) 
                                        .FirstOrDefaultAsync(p => p.ProductId == addStoneDTO.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var stonePrice = product.Stones.Sum(s => s.Price); 

            if (product.GoldTypes == null)
            {
                return BadRequest("Gold type information is missing for the product.");
            }

            product.Price = ((product.GoldWeight * product.GoldTypes.SellPrice) + product.Wage + stonePrice) * product.PriceRatio;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(mapper.Map<StoneDTO>(stone));
        }




        // DELETE: api/Stones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStone(int id)
        {
            var checkExistProduct = await stoneRepository.DeleteStoneAsync(id);
            if (checkExistProduct == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<StoneDTO>(checkExistProduct));
        }

     
      
    }
}