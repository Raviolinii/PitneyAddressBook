using PitneyAddressBook.Models;

namespace PitneyAddressBook.Repository
{
    public interface IAddressBookRepository
    {
        public Task AddAddressAsync(Address address);
        public Address GetAddress(int id);
        public Task<Address?> GetLastAddressAsync();
        public Task<List<Address>> GetAddressesAsync(string city);
        public bool IdExists(int id);
        public bool IsAddressValid(Address address);
    }
}
