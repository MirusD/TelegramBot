using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;
using LinqToDB;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _db;

        public AddressRepository(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Address> AddAsync(Address newAddress)
        {
            using (var transaction = await _db.BeginTransactionAsync())
            {
                try
                {
                    var id = await _db.InsertWithInt64IdentityAsync(newAddress);
                    await transaction.CommitAsync();

                    newAddress.Id = id;
                    return newAddress;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Ошибка при добавлении нового адреса " + ex.ToString());
                    throw;
                }
            }
        }

        public Task<Address?> DeleteAsync(long addressId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Address>> GetAllAsync()
        {
            try
            {
                var address = await _db.GetTable<Address>().ToListAsync();
                return address;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении адресов: " + ex.ToString());
                throw;
            }
        }

        public Task<Address?> GetByIdAsync(long addressId)
        {
            throw new NotImplementedException();
        }

        public Task<Address> UpdateAsync(Address updatedAddress)
        {
            throw new NotImplementedException();
        }
    }
}
