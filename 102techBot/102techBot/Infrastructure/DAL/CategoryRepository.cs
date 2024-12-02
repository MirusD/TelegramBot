using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext dbContext) 
        {
            _db = dbContext;
        }
        public async Task<Category> AddAsync(Category newCategory)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newCategory);
                    await transaction.CommitAsync();

                    newCategory.Id = id;
                    return newCategory;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении новой категории", ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Category?> DeleteAsync(long categoryId)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCategory = await _db.GetTable<Category>()
                        .FirstOrDefaultAsync(p => p.Id == categoryId);

                    if (existingCategory == null)
                    {
                        return null;
                    }

                    await _db.GetTable<Category>()
                              .DeleteAsync(p => p.Id == categoryId);

                    await transaction.CommitAsync();
                    return existingCategory;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при удалении категории" + ex.ToString());
                    throw;
                }
            }
        }

        public async Task <List<Category>> GetAllAsync()
        {
            try
            {
                return await _db.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении всех категории: " + ex.ToString());
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(long categoryId)
        {
            try
            {
                return await _db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении товара по ID: {categoryId}" + ex.ToString());
                throw;
            }
        }

        public async Task<Category?> UpdateAsync(Category updatedCategory)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var existingCategory = await _db.GetTable<Product>()
                                                .FirstOrDefaultAsync(c => c.Id == updatedCategory.Id);

                    if (existingCategory == null)
                    {
                        return null;
                    }

                    await _db.UpdateAsync(updatedCategory);
                    await transaction.CommitAsync();
                    return updatedCategory;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при обновлении категории" + ex.ToString());
                    throw;
                }
            }
        }
    }
}
