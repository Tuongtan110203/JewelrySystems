using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanVang.CustomActionFilters;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000")]
    // [Authorize(Roles = "manager")]

    public class ProductController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly IProductRepository productRepository;
        private readonly IStoneRepository stoneRepository;
        private readonly IMapper mapper;

        public ProductController(JewelrySalesSystemDbContext context, IProductRepository productRepository, IMapper mapper, IStoneRepository stoneRepository)
        {
            _context = context;
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.stoneRepository = stoneRepository;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAllProductAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
          [FromQuery] string? sortBy, [FromQuery] bool isAscending,
          [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var productDomains = await productRepository.GetAllProductAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);
/*

            foreach (var productDTO in productDTOs)
            {
                double totalStonePrice = 0;
                foreach (var stone in productDTO.Stones)
                {
                    totalStonePrice += stone.Price;
                }
                productDTO.StonePrice = totalStonePrice;
            }*/

            return Ok(productDTOs);
        }

        // GET: api/Product/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetByIdProductAsync([FromRoute] int id)
        {
            var productDomain = await productRepository.GetByIdProductAsync(id);
            if (productDomain == null) { return NotFound(); }
            var productDTO = mapper.Map<ProductDTO>(productDomain);
            return Ok(productDTO);
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //[ValidateModel]
        //public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductDTO updateProductDTO)
        //{
        //    double stonePrice = await stoneRepository.GetStonePriceByProductIdAsync(id);
        //    updateProductDTO.StonePrice = stonePrice;
        //    double sellPrice = await productRepository.GetSellPriceByProductIdAsync(id);
        //    if (sellPrice == 0)
        //    {
        //        return BadRequest("Invalid ProductID");
        //    }
        //    updateProductDTO.SellPrice = sellPrice;
        //    var existingProduct = await productRepository.GetByIdProductAsync(id);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }
        //    var productDomainModel = mapper.Map<Product>(updateProductDTO);

        //    await productRepository.UpdateProductAsync(id, productDomainModel);

        //    return Ok(mapper.Map<ProductDTO>(productDomainModel));
        //}

        [HttpPut("UpdateProductNormal/{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProductNormal(int id, [FromBody] UpdateProductNormalDTO updateProductNormalDTO)
        {
            var existingProduct = await productRepository.GetByIdProductAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            var productDomainModel = mapper.Map<Product>(updateProductNormalDTO);

            await productRepository.UpdateProductAsync(id, productDomainModel);

            return Ok(mapper.Map<ProductDTO>(productDomainModel));
        }



        [HttpPut("UpdateTickOne/{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProductByTickOne(int id, [FromBody] UpdateProductOnlyPriceOrAllPriceDTO updateProduct)
        {
            double sellPrice = await productRepository.GetSellPriceByProductIdAsync(id);
            if (sellPrice == 0)
            {
                return BadRequest("Invalid ProductID");
            }
            updateProduct.SellPrice = sellPrice;
            double goldWeight = await productRepository.GetGoldWeightByProductIdAsync(id);
            updateProduct.GoldWeight = goldWeight;
            double wage = await productRepository.GetWageByProductIdAsync(id);
            updateProduct.Wage = wage;
            double priceRatio = await productRepository.GetPriceRatioByProductIdAsync(id);
            updateProduct.PriceRatio = priceRatio;
            double priceStone = await productRepository.GetStonePriceByProductIdAsync(id);
            updateProduct.StonePrice = priceStone;
            var productDomainModel = mapper.Map<Product>(updateProduct);
            await productRepository.UpdateProductByTickOne(id, productDomainModel);

            return Ok(mapper.Map<ProductDTO>(productDomainModel));
        }


        [HttpPut("UpdateTickALL")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProductByTickALL()
        {
            var products = await productRepository.UpdateProductByTickALL();
            if (products == null || products.Count == 0)
            {
                return NotFound("No active products found to update.");
            }

            return Ok(products.Select(mapper.Map<ProductDTO>).ToList());
        }


        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> AddProductAsync([FromBody] AddProductDTO addProductDTO)
        {
            double sellPrice = await productRepository.GetSellPriceByGoldIdAsync(addProductDTO.GoldId);
            if (sellPrice == 0)
            {
                return BadRequest("Invalid GoldId");
            }
            addProductDTO.SellPrice = sellPrice;
    
            var product = mapper.Map<Product>(addProductDTO);

            try
            {
                var createdProduct = await productRepository.CreateProductAsync(product);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/Product/5
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var checkExistProduct = await productRepository.DeleteProductAsync(id);
            if (checkExistProduct == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ProductDTO>(checkExistProduct));
        }

      



    }
}
