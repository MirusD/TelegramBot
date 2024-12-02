using _102techBot.Domain.Entities;

namespace _102techBot.Domain.Repositories
{
    internal interface IAddressRepository
    {
        public Task<Address> AddAsync(Address newAddress);
        public Task<Address?> DeleteAsync(long addressId);
        public Task<List<Address>> GetAllAsync();
        public Task<Address?> GetByIdAsync(long addressId);
        public Task<Address> UpdateAsync(Address updatedAddress);
    }
}
