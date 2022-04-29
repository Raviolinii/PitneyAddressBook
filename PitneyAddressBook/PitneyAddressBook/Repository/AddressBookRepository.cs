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
            _addressBook = _dataPersistence.ReadAllData();
        }
        public void AddAddress(Address address)
        {
            _addressBook.addresses.Add(address);

            _dataPersistence.SaveData(_addressBook);
        }

        public Address GetAddress(int id) => _addressBook.addresses.First(r => r.AddressId == id);
        public Address? GetLastAddress()
        {
            if (_addressBook.addresses.Any())
                return _addressBook.addresses.Last();

            return null;
        }

        public List<Address> GetAddresses(string city)
        {
            var result = _addressBook.addresses.Where(r => r.City == city).ToList();
            return result;
        }
    }
}
