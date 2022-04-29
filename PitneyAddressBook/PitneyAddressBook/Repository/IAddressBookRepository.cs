using PitneyAddressBook.Models;

namespace PitneyAddressBook.Repository
{
    public interface IAddressBookRepository
    {
        public void AddAddress(Address address);
        public Address GetAddress(int id);
        public Address? GetLastAddress();
        public List<Address> GetAddresses(string city);
        public bool IdExists(int id);
        public bool IsAddressValid(Address address);
    }
}
