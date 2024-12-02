using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface ICategoryRepository
    {
        public Task<Category> AddAsync(Category newCategory);
        public Task<Category?> DeleteAsync(long categoryId);

        public Task<List<Category>> GetAllAsync();

        public Task<Category?> GetByIdAsync(long categoryId);

        public Task<Category?> UpdateAsync(Category updatedCategory);
    }
}
