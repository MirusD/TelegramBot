using _102techBot.Domain.Entities;
using _102techBot.Common;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Categories
{
    internal class CategoryService
    {
        public readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> AddNewCategoryAsync(string categoryName)
        {
            var newCategory = new Category()
            {
                Name = categoryName,
                Description = null
            };

            return await _categoryRepository.AddAsync(newCategory);
        }

        public async Task<Category?> GetCategoryByIdAsync(long categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }
    }
}
