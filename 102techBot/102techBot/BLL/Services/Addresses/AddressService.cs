using _102techBot.Domain.Entities;
using _102techBot.Domain.Repositories;

namespace _102techBot.BLL.Services.Addresses
{
    internal class AddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<Address> AddAddressAsync(Address newAddress)
        {
            return await _addressRepository.AddAsync(newAddress);
        }

        public async Task<List<Address>> GetAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }
    }
}
