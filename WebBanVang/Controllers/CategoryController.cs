using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class CategoryController : ControllerBase
    {
        private readonly JewelrySalesSystemDbContext _context;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryController(JewelrySalesSystemDbContext context, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _context = context;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await categoryRepository.GetAllCategoriesAsync();
            return Ok(mapper.Map<List<CategoryDTO>>(categories));
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CategoryDTO>(category));
        }


        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO)
        {

            var category = mapper.Map<Category>(updateCategoryDTO);

            category = await categoryRepository.UpdateCategoryAsync(id, category);

            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = mapper.Map<CategoryDTO>(category);


            _context.SaveChanges();
            return Ok(categoryDto);

        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {
            var category = mapper.Map<Category>(addCategoryDTO);
            category = await categoryRepository.CreateAsync(category);
            var categoryDto = mapper.Map<CategoryDTO>(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var checkExistProduct = await categoryRepository.DeleteCategoryAsync(id);
            if (checkExistProduct == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CategoryDTO>(checkExistProduct));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}