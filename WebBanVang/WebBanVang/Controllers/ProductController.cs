using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using WebBanVang.CustomActionFilters;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;
using static WebBanVang.Repository.SQLProductRepository;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    // [Authorize(Roles = "manager")]
    //[AllowAnonymous]

    public class ProductController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly IProductRepository productRepository;
        private readonly IStoneRepository stoneRepository;
        private readonly IFileRepository fileRepository;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IStorageRepository storageRepository;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductController(JewelrySalesSystemDbContext context, IProductRepository productRepository,
            IMapper mapper, IStoneRepository stoneRepository, IFileRepository fileRepository,
            BlobServiceClient blobServiceClient)
        {
            _context = context;
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.stoneRepository = stoneRepository;
            this.fileRepository = fileRepository;
            this.storageRepository = storageRepository;
            this._hostingEnvironment = _hostingEnvironment;
            this.blobServiceClient = blobServiceClient;
        }

        // GET: api/Product
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllProductAsync([FromQuery] List<string>? filterOn, [FromQuery] List<string>? filterQuery,
        [FromQuery] string? sortBy, [FromQuery] bool isAscending,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000,
        [FromQuery] double? minPrice = null,
        [FromQuery] double? maxPrice = null)
        {
            List<FilterCriteria> filters = new List<FilterCriteria>();

            if (filterOn != null && filterQuery != null && filterOn.Count == filterQuery.Count)
            {
                for (int i = 0; i < filterOn.Count; i++)
                {
                    filters.Add(new FilterCriteria { FilterOn = filterOn[i], FilterQuery = filterQuery[i] });
                }
            }

            var productDomains = await productRepository.GetAllProductAsync(filters, sortBy,
                isAscending, pageNumber, pageSize, minPrice, maxPrice);

            var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);

            return Ok(productDTOs);
        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateModel]
        public async Task<IActionResult> AddProductAsync([FromForm] AddProductDTO addProductDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == addProductDTO.CategoryId);
            if (!categoryExists)
            {
                return NotFound("Không tìm thấy danh mục");
            }

            var goldTypoesExist = await _context.GoldTypes.AnyAsync(c => c.GoldId == addProductDTO.GoldId);
            if (!goldTypoesExist)
            {
                return NotFound("Không tìm thấy loại vàng");
            }

            var product = mapper.Map<Product>(addProductDTO);

            if (await IsDuplicateProductCode(addProductDTO.ProductCode))
            {
                return Conflict("Mã sản phẩm đã tồn tại.");
            }
            try
            {
                if (addProductDTO.Image != null)
                {
                    var containerClient = blobServiceClient.GetBlobContainerClient("productcontainer");
                    await containerClient.CreateIfNotExistsAsync();

                    var blobClient = containerClient.GetBlobClient(addProductDTO.Image.FileName);
                    var blobHttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = addProductDTO.Image.ContentType
                    };

                    using (var stream = addProductDTO.Image.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
                    }

                    product.Image = blobClient.Uri.ToString();
                }

                // Create the product
                var createdProduct = await productRepository.CreateProductAsync(product);
                await _context.SaveChangesAsync();

                return Ok(createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // [HttpGet("FilterSellPriceGoldTypes")]
        // public async Task<IActionResult> FilterSellPriceGoldTypes(
        //[FromQuery] string? filterOn = null,
        //[FromQuery] string? filterQuery = null,
        //[FromQuery] double? minPrice = null,
        //[FromQuery] double? maxPrice = null)
        // {
        //     var productDomains = await productRepository.FilterSellPriceGoldTypes(filterOn, filterQuery, minPrice, maxPrice);

        //     var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);

        //     return Ok(productDTOs);
        // }
        [HttpGet("GetProductByNameOrCode")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetProductByNameOrCode(
       [FromQuery] string? nameCode,
      [FromQuery, DefaultValue(1)] int pageNumber = 1,
      [FromQuery, DefaultValue(1000)] int pageSize = 1000)
        {
            var productDomains = await productRepository.GetByProductByNameOrCode(nameCode, pageNumber, pageSize);
            var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);
            return Ok(productDTOs);
        }


        //[HttpGet]
        //[Route("barcode/{id}")]
        //[Authorize(Roles = "Staff,Manager")]
        //public  Task<IActionResult> GetBarcode([FromRoute] int id)
        //{
        //    string webRootPath = _hostingEnvironment.WebRootPath;
        //    string fileName = $"productID{id}.png";
        //    string filePath = Path.Combine(webRootPath, fileName);

        //    // Ensure the wwwroot directory exists
        //    if (!Directory.Exists(webRootPath))
        //    {
        //        Directory.CreateDirectory(webRootPath);
        //    }
        //    IronBarCode.License.LicenseKey = "IRONSUITE.TAMVTTQE180092.FPT.EDU.VN.26746-B480268579-C6H7X7E-QY3W5F2AEGBQ-3G3ZQ776A3DP-K4HD4JIHYVMZ-GR4BBB7WN3OM-UOQ2FPVXVPVQ-TXMXQXWS25XN-RQEYHD-TR3TG45WHXCMUA-DEPLOYMENT.TRIAL-WFHPFQ.TRIAL.EXPIRES.04.JUL.2024";
        //    // BarcodeWriter.CreateBarcode("123", BarcodeEncoding.Code128, 200, 100).SaveAsPng("D:\\" + "productID" + id + ".png");
        //    BarcodeWriter.CreateBarcode("123", BarcodeEncoding.Code128, 200, 100).SaveAsPng(filePath);
        //    string imageUrl = $"{Request.Scheme}://{Request.Host}/{fileName}";

        //    // Return the URL of the saved image
        //    return Ok(new { Url = imageUrl });
        //}

        // GET: api/Product/5
        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<ActionResult<Product>> GetByIdProductAsync([FromRoute] int id)
        {
            var productDomain = await productRepository.GetByIdProductAsync(id);
            if (productDomain == null) { return NotFound(); }
            var productDTO = mapper.Map<ProductDTO>(productDomain);
            return Ok(productDTO);
        }
        //[HttpGet("GetProductByProductCode")]
        //public async Task<ActionResult<Product>> GetProductByProductCode(string code)
        //{
        //    var productDomain = await productRepository.GetByProductCodeAsync(code);
        //    if (productDomain == null) { return NotFound(); }
        //    var productDTO = mapper.Map<ProductDTO>(productDomain);
        //    return Ok(productDTO);
        //}
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
        [Authorize(Roles = "Manager")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProductNormal(int id, [FromForm] UpdateProductNormalDTO updateProductNormalDTO)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == updateProductNormalDTO.CategoryId);
            if (!categoryExists)
            {
                return NotFound("Không tìm thấy danh mục");
            }

            var goldTypoesExist = await _context.GoldTypes.AnyAsync(c => c.GoldId == updateProductNormalDTO.GoldId);
            if (!goldTypoesExist)
            {
                return NotFound("Không tìm thấy loại vàng");
            }

            var containerClient = blobServiceClient.GetBlobContainerClient("productcontainer");
            var existingProduct = await productRepository.GetByIdProductAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            var hasWarranty = await _context.OrderDetails
                                   .Where(od => od.ProductId == id)
                                   .AnyAsync(od => _context.Warranties.Any(w => w.OrderDetailId == od.OrderDetailId));

            if (hasWarranty)
            {
                if (updateProductNormalDTO.Price != existingProduct.Price)
                {
                    existingProduct.Price = updateProductNormalDTO.Price;
                    existingProduct.Image = existingProduct.Image;
                    await _context.SaveChangesAsync();
                    return Ok(mapper.Map<ProductDTO>(existingProduct));
                }
                else if (updateProductNormalDTO.Image == null)
                {
                    existingProduct.Image = existingProduct.Image;
                    await _context.SaveChangesAsync();
                    return Ok(mapper.Map<ProductDTO>(existingProduct));
                }
                else
            {
                return BadRequest("Sản phẩm đã có phiếu bảo hành, chỉ có thể cập nhật giá.");
            }
            }

            if (await IsDuplicateProductCode(updateProductNormalDTO.ProductCode, id))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sản phẩm đã tồn tại");
            }


            existingProduct.ProductCode = updateProductNormalDTO.ProductCode;
            existingProduct.CategoryId = updateProductNormalDTO.CategoryId;
            existingProduct.GoldId = updateProductNormalDTO.GoldId;
            existingProduct.ProductName = updateProductNormalDTO.ProductName;
            existingProduct.Description = updateProductNormalDTO.Description;
            existingProduct.Quantity = updateProductNormalDTO.Quantity;
            existingProduct.GoldWeight = updateProductNormalDTO.GoldWeight;
            existingProduct.Wage = updateProductNormalDTO.Wage;
            existingProduct.Price = updateProductNormalDTO.Price;
            existingProduct.Size = updateProductNormalDTO.Size;
            existingProduct.WarrantyPeriod = updateProductNormalDTO.WarrantyPeriod;
            existingProduct.Status = updateProductNormalDTO.Status;

            var updatedProduct = await productRepository.UpdateProductAsync(id, existingProduct);


            await _context.SaveChangesAsync();


            if (updateProductNormalDTO.Image != null)
            {
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    try
                    {
                        var oldBlobName = Path.GetFileName(new Uri(existingProduct.Image).AbsolutePath);
                        var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
                        await oldBlobClient.DeleteIfExistsAsync();
                    }
                    catch (UriFormatException)
                    {
                    }
                }


                var newBlobClient = containerClient.GetBlobClient(updateProductNormalDTO.Image.FileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = updateProductNormalDTO.Image.ContentType
                };

                using (var stream = updateProductNormalDTO.Image.OpenReadStream())
                {
                    await newBlobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
                }

                existingProduct.Image = newBlobClient.Uri.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    try
                    {
                        var oldBlobName = Path.GetFileName(new Uri(existingProduct.Image).AbsolutePath);
                        var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
                        await oldBlobClient.DeleteIfExistsAsync();
                        existingProduct.Image = null;
                    }
                    catch (UriFormatException)
                    {
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(mapper.Map<ProductDTO>(existingProduct));
        }




        [HttpPut("UpdateTickOne/{id}")]
        [Authorize(Roles = "Manager")]
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

            double priceStone = await productRepository.GetStonePriceByProductIdAsync(id);
            updateProduct.StonePrice = priceStone;
            var productDomainModel = mapper.Map<Product>(updateProduct);
            await productRepository.UpdateProductByTickOne(id, productDomainModel);

            return Ok(mapper.Map<ProductDTO>(productDomainModel));
        }


        [HttpPut("UpdateTickALL")]
        [Authorize(Roles = "Manager")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProductByTickALL()
        {
            var products = await productRepository.UpdateProductByTickALL();
            if (products == null || products.Count == 0)
            {
                return Conflict("Sản phẩm không tồn tại");
            }
            return Ok(products.Select(mapper.Map<ProductDTO>).ToList());
        }


        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var checkExistProduct = await productRepository.DeleteProductAsync(id);
            if (checkExistProduct == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ProductDTO>(checkExistProduct));
        }

        [HttpGet("GetOutOfStockProduct")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetOutOfStockProduct()
        {
            var productDomains = await productRepository.GetOutOfStockProductsAsync();
            var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);
            return Ok(productDTOs);
        }

        [HttpGet("get-top-best-seller-product/{topNum}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetTopBestSellerProduct(int topNum)
        {
            var productDomains = await productRepository.GetTopBestSellerProductsAsync(topNum);
            var productDTOs = mapper.Map<List<ProductDTO>>(productDomains);
            return Ok(productDTOs);
        }
        private async Task<bool> IsDuplicateProductCode(string productCode, int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.Products
                    .AnyAsync(x => x.ProductId != id.Value && x.ProductCode == productCode);
            }
            else
            {
                return await _context.Products
                    .AnyAsync(x => x.ProductCode == productCode);
            }
        }

        [HttpPut("update-not-sold-product-to-active-and-active-not-sold")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateNotSoldProductToActive(int id)
        {
            var existingProduct = await productRepository.UpdateNotSoldProductToActiveAsync(id);
            return Ok(mapper.Map<ProductDTO>(existingProduct));
        }
    }
}
