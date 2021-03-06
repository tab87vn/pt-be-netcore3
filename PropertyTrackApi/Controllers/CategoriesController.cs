using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyTrackApi.Models;
using PropertyTrackApi.ViewModels;
using PropertyTrackApi.Services;
using PropertyTrackApi.Services.Exceptions;

namespace PropertyTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryViewModel>>> GetCategoriesAsync()
        {
            return await _categoryService.GetCategoriesAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseCategoryViewModel>> GetCategoryAsync(int id)
        {
            try
            {
                return await _categoryService.GetCategoryAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<CategoryWithItemsViewModel>> GetCategoryWithItems(int id)
        {
            try
            {
                return await _categoryService.GetCategoryWithItemsAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(id, category);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category category)
        {
            await _categoryService.CreateCategoryAsync(category);

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (CategoryServiceException)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
