using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    //[AllowAnonymous]
    public class CategoryController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(JewelrySalesSystemDbContext context, ICategoryRepository categoryRepository, IMapper mapper,
            ILogger<CategoryController> logger)
        {
            _context = context;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            _logger = logger;
        }

        // GET: api/Category
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var categories = await categoryRepository.GetAllCategoriesAsync();
                return Ok(mapper.Map<List<CategoryDTO>>(categories));
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Error in GetAll");
                return StatusCode(StatusCodes.Status500InternalServerError, "Database error. Please try again later.");
            }
            catch (ArgumentNullException argEx)
            {
                _logger.LogError(argEx, "Argument Null Error in GetAll");
                return BadRequest("Invalid argument. Please check your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Error in GetAll");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later.");
            }
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CategoryDTO>(category));
        }

        [HttpGet("get-categories-by-name/{name}")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> GetCategoriesByName(string name)
        {
            var category = await categoryRepository.GetCategoriesByNameAsync(name);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<List<CategoryDTO>>(category));
        }

        //[HttpGet("get-category-by-name/{name}")]
        //[Authorize(Roles = "Staff,Manager")]
        //public async Task<IActionResult> GetCategoryByName(string name)
        //{
        //    var category = await categoryRepository.GetCategoryByNameAsync(name);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(mapper.Map<CategoryDTO>(category));
        //}

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            var existingItem = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingItem == null)
            {
                return NotFound();
            }

            if (await IsDuplicateCategoryCode(updateCategoryDTO.Code, id))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mã danh mục đã tồn tại");
            }

            existingItem.CategoryCode = updateCategoryDTO.Code;
            existingItem.Name = updateCategoryDTO.Name;
            existingItem.Status = updateCategoryDTO.Status;

            await _context.SaveChangesAsync();

            var categoryDto = mapper.Map<CategoryDTO>(existingItem);
            return Ok(categoryDto);
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {
            if (await IsDuplicateCategoryCode(addCategoryDTO.CategoryCode))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mã danh mục đã tồn tại");
            }

            var category = mapper.Map<Category>(addCategoryDTO);
            if (category == null)
            {
                return NotFound();
            }

            category = await categoryRepository.CreateAsync(category);
            return Ok(mapper.Map<CategoryDTO>(category));
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var checkExistProduct = await categoryRepository.DeleteCategoryAsync(id);
                if (checkExistProduct == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<CategoryDTO>(checkExistProduct));
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Kiểm tra trùng lặp mã danh mục
        private async Task<bool> IsDuplicateCategoryCode(string categoryCode, int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.Categories
                    .AnyAsync(x => x.CategoryId != id.Value && x.CategoryCode == categoryCode);
            }
            else
            {
                return await _context.Categories
                    .AnyAsync(x => x.CategoryCode == categoryCode);
            }
        }

        [HttpGet("get-percentages-category/{option}")]
        public async Task<ActionResult<List<GoldTypePercentageDTO>>> GetGoldTypePercentages(string option)
        {
            /*var percentages = await goldTypeRepository.CalculateGoldTypePercentagesAsync();
            return Ok(percentages);*/
            if (option == "today")
            {
                var percentages = await categoryRepository.GetCategoryCodePercentagesForToday();
                return Ok(percentages);
            }
            else if (option == "this-week")
            {
                var percentages = await categoryRepository.GetCategoryCodePercentagesForThisWeek();
                return Ok(percentages);
            }
            else if (option == "this-month")
            {
                var percentages = await categoryRepository.GetCategoryCodePercentagesForThisMonth();
                return Ok(percentages);
            }
            else if (option == "this-year")
            {
                var percentages = await categoryRepository.GetCategoryCodePercentagesForThisYear();
                return Ok(percentages);
            }
            else
            {
                return NotFound(new { message = "Option not found" });
            }

        }
    }
}
