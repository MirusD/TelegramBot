using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Product> AddAsync(Product newProduct)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newProduct);
                    await transaction.CommitAsync();

                    newProduct.Id = id;
                    return newProduct;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении нового товара", ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Product?> DeleteAsync(long productId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingProduct = await _db.GetTable<Product>()
                        .FirstOrDefaultAsync(p => p.Id == productId);

                    if (existingProduct == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Cart>()
                              .DeleteAsync(p => p.Id == productId);

                    await transaction.CommitAsync();
                    return existingProduct;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении товара" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                return await _db.GetTable<Product>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех товаров" + ex.ToString());
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(long productId)
        {
            try
            {
                return await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товара по ID: {productId}" + ex.ToString());
                throw;
            }
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(long categoryId)
        {
            try
            {
                return await _db.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении товара по категории" + ex.ToString());
                throw;
            }
        }

        public async Task<List<Product>> GetProductsByUserAsync(long userId)
        {
            try
            {
                return await _db.Products.Where(p => p.UserId == userId).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при получении товара по пользователю" + ex.ToString());
                throw;
            }
        }

        public async Task<Product?> UpdateAsync(Product updatedProduct)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingProduct = await _db.GetTable<Product>()
                                                .FirstOrDefaultAsync(c => c.Id == updatedProduct.Id);

                    if (existingProduct == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedProduct);
                    await transaction.CommitAsync();
                    return updatedProduct;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении товара" + ex.ToString());
                    throw;
                }
            }
        }
    }
}
