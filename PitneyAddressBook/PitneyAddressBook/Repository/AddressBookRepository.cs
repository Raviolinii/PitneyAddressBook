using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;

namespace PitneyAddressBook.Repository
{
    public class AddressBookRepository : IAddressBookRepository
    {
        private readonly AddressBook _addressBook;
        private readonly IDataPersistence _dataPersistence;
        public AddressBookRepository(IDataPersistence dataPersistence)
        {
            _dataPersistence = dataPersistence;
            _addressBook = new AddressBook();
        }
        public async void AddAddress(Address address)
        {
            await _dataPersistence.AddData(address);
            _addressBook.addresses.Add(address);
            _addressBook.LastAddress = address;
        }

        public Address GetAddress(int id) => _addressBook.addresses.First(r => r.AddressId == id);
        public Address? GetLastAddress() => _addressBook.LastAddress;

        public List<Address> GetAddresses(string city)
        {
            var result = _addressBook.addresses.Where(r => r.City == city).ToList();
            return result;
        }
    }
}
