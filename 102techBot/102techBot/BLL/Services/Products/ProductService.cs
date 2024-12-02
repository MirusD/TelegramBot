using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Products
{
    internal class ProductService
    {
        public readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) 
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateProductAsync(Product newProduct)
        {
            return await _productRepository.AddAsync(newProduct);
        }

        public async Task<Product?> GetProductByIdAsync(long productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(long categoryId)
        {
           return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }
    }
}
