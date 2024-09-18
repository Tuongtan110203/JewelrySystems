using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
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
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllStones()
        {
            var stones = await stoneRepository.GetAllStonesAsync();
            return Ok(mapper.Map<List<StoneDTO>>(stones));
        }

        // GET: api/Stones/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetStoneById(int id)
        {
            var stone = await stoneRepository.GetStoneByIdAsync(id);
            if (stone == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<StoneDTO>(stone));
        }

        [HttpGet("get-stones-by-name{name}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetStonesByName(string name)
        {
            var stone = await stoneRepository.GetStonesByStoneNameOrStoneCodeOrProductCodeAsync(name);
            if (stone == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<StoneDTO>>(stone));
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateStone(int id, UpdateStoneDTO updateStoneDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stone = await stoneRepository.GetStoneByIdAsync(id);
            var productSubtraction = await _context.Products
                                       .FirstOrDefaultAsync(p => p.ProductId == stone.ProductId);
            if (productSubtraction != null)
            {
                productSubtraction.Price -= stone.Price;
                _context.Products.Update(productSubtraction);
                await _context.SaveChangesAsync();
            }



            if (stone == null)
            {
                return NotFound("Đá không tồn tại");
            }

            if (await IsDuplicateStoneCode(updateStoneDTO.StoneCode, id))
            {
                return Conflict("Mã đá đã tồn tại.");
            }
            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes)
                                        .FirstOrDefaultAsync(p => p.ProductCode == updateStoneDTO.ProductCode);

            var hasWarranty = await _context.OrderDetails
                             .Where(od => od.ProductId == product.ProductId)
                             .AnyAsync(od => _context.Warranties.Any(w => w.OrderDetailId == od.OrderDetailId));

            if (hasWarranty)
            {
                return BadRequest("Đá không thể cập nhật vì sản phẩm đã có phiếu bảo hành.");
            }

            if (product == null)
            {
                return Conflict("Sản phẩm không tồn tại");
            }

            mapper.Map(updateStoneDTO, stone);
            stone.ProductId = product.ProductId;

            stone = await stoneRepository.UpdateStoneAsync(id, stone);

            var stoneDto = mapper.Map<StoneDTO>(stone);
            var stonePrice = product.Stones.Sum(s => s.Price);

            if (product.GoldTypes == null)
            {
                return BadRequest("Gold type information is missing for the product.");
            }

            product.Price = (product.GoldWeight * product.GoldTypes.SellPrice) + product.Wage + stonePrice;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(stoneDto);
        }



        // POST: api/Stones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<Stone>> PostStone(AddStoneDTO addStoneDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stone = mapper.Map<Stone>(addStoneDTO);
            if (stone == null)
            {
                return BadRequest("Đá không tồn tại");
            }

         


            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes)
                                        .FirstOrDefaultAsync(p => p.ProductCode == addStoneDTO.ProductCode);

            var hasWarranty = await _context.OrderDetails
                              .Where(od => od.ProductId == product.ProductId)
                              .AnyAsync(od => _context.Warranties.Any(w => w.OrderDetailId == od.OrderDetailId));

            if (hasWarranty)
            {
                return BadRequest("Đá không thể thêm vì sản phẩm đã có phiếu bảo hành.");
            }

            if (await IsDuplicateStoneCode(addStoneDTO.StoneCode))
            {
                return Conflict("Mã đá đã tồn tại.");
            }

            if (product == null)
            {
                return Conflict("Sản phẩm không tồn tại.");

            }

            stone.ProductId = product.ProductId;

            try
            {
                stone = await stoneRepository.CreateAsync(stone);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            var stonePrice = product.Stones.Where(x => x.Status == "active").Sum(s => s.Price);

            if (product.GoldTypes == null)
            {
                return BadRequest("Gold type information is missing for the product.");
            }

            product.Price = ((product.GoldWeight * product.GoldTypes.SellPrice) + product.Wage + stonePrice);

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(mapper.Map<StoneDTO>(stone));
        }




        // DELETE: api/Stones/5
        // DELETE: api/Stones/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteStone(int id)
        {
            var stone = await stoneRepository.GetStoneByIdAsync(id);
            if (stone == null)
            {
                return NotFound("Đá không tồn tại.");
            }

            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes)
                                        .FirstOrDefaultAsync(p => p.ProductId == stone.ProductId);

           
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }


            var stonePrice = stone.Price;

            if (product.GoldTypes == null)
            {
                return BadRequest("Thông tin loại vàng không có sẵn cho sản phẩm.");
            }

            product.Price -= stonePrice;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            await stoneRepository.DeleteStoneAsync(id);

            var stoneDto = mapper.Map<StoneDTO>(stone);
            return Ok(stoneDto);
        }

        [HttpPut("detach-stone")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DetachStoneFromProduct(int id)
        {
            var stone = await stoneRepository.DetachStoneFromProductAsync(id);
            if (stone == null)
            {
                return NotFound("Đá không tồn tại.");
            }


            var product = await _context.Products
                                        .Include(p => p.Stones)
                                        .Include(p => p.GoldTypes)
                                        .FirstOrDefaultAsync(p => p.ProductId == stone.ProductId);
            var hasWarranty = await _context.OrderDetails
                             .Where(od => od.ProductId == product.ProductId)
                             .AnyAsync(od => _context.Warranties.Any(w => w.OrderDetailId == od.OrderDetailId));

            if (hasWarranty)
            {
                return BadRequest("Đá không thể tách vì sản phẩm đã có phiếu bảo hành.");
            }


            if (product != null)
            {
                var stonePrice = stone.Price;
                product.Price -= stonePrice;
                stone.ProductId = null;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return Ok(mapper.Map<StoneDTO>(stone));
        }

        private async Task<bool> IsDuplicateStoneCode(string stoneCode, int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.Stones
                    .AnyAsync(x => x.StoneId != id.Value && x.StoneCode == stoneCode);
            }
            else
            {
                return await _context.Stones
                    .AnyAsync(x => x.StoneCode == stoneCode);
            }
        }

    }
}