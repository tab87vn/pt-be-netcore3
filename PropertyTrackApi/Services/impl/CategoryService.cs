using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropertyTrackApi.Models;
using PropertyTrackApi.ViewModels;
using PropertyTrackApi.Services.Exceptions;

namespace PropertyTrackApi.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private PropertyTrackContext _context;
        public CategoryService(PropertyTrackContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Include(cat => cat.Items)
                .Select(cat => new CategoryViewModel(cat))
                .ToListAsync();
        }

        public async Task<BaseCategoryViewModel> GetCategoryAsync(int id)
        {
            var cat = await _context.Categories.FindAsync((int)id);

            if (cat == null)
            {
                ThrowNotFoundException(id);
            }

            return new BaseCategoryViewModel(cat);

        }
        public async Task<CategoryWithItemsViewModel> GetCategoryWithItemsAsync(int categoryId)
        {
            var category = await _context.Categories
                .Where(c => c.Id == categoryId)
                .Include(c => c.Items)
                .Select(c => new CategoryWithItemsViewModel(c)
                {
                    Items = c.Items.Select(i => new ItemViewModel(i))
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                ThrowNotFoundException(categoryId);
            }

            return category;
        }

        public async Task UpdateCategoryAsync(int catId, Category category)
        {
            var catExists = await CategoryExistsAsync(catId);
            if (!catExists)
            {
                //return NotFound();
                ThrowNotFoundException(catId);
            }

            try
            {
                _context.Entry(category).State = EntityState.Modified;
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new CategoryServiceException($"Cannot update category {catId}", ex);
            }
        }

        public async Task CreateCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new CategoryServiceException("Cannot create new category.", ex);
            }            
        }

        public async Task DeleteCategoryAsync(int catId)
        {
            var category = await _context.Categories.FindAsync(catId);
            if (category == null)
            {
                ThrowNotFoundException(catId);
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new CategoryServiceException($"Cannot delete requested category: {catId}.", ex);
            }
        }

        private async Task<bool> CategoryExistsAsync(int id) 
         => await _context.Categories.AnyAsync(e => e.Id == id);

        private static void ThrowNotFoundException(int catId)
         => throw new NotFoundException($"The requested Category Id {catId} does not exist");
    }
}