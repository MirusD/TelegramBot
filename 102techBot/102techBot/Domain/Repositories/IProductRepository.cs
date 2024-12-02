using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface IProductRepository
    {
        public Task<Product> AddAsync(Product newProduct);
        public Task<Product?> DeleteAsync(long productId);
        public Task<List<Product>> GetAllAsync();
        public Task<Product?> GetByIdAsync(long productId);
        public Task<List<Product>> GetProductsByCategoryAsync(long categoryId);
        public Task<List<Product>> GetProductsByUserAsync(long userId);
        public Task<Product?> UpdateAsync(Product product);
    }
}
