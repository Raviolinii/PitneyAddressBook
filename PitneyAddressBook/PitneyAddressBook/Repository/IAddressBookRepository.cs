using PitneyAddressBook.Models;

namespace PitneyAddressBook.Repository
{
    public interface IAddressBookRepository
    {
        public Task AddAsync(Address address);
        public Task<Address?> GetLastAsync();
        public Task<List<Address>> GetByCityAsync(string city);
        public bool IdExists(int id);
        public bool IsAddressValid(Address address);
    }
}
