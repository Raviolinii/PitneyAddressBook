using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;

namespace PitneyAddressBook.Repository
{
    public class AddressBookRepository : IAddressBookRepository
    {
        private readonly AddressBook _addressBook;
        private readonly IDataPersistence<Address> _dataPersistence;
        public AddressBookRepository(IDataPersistence<Address> dataPersistence)
        {
            _dataPersistence = dataPersistence;
            var addresses = _dataPersistence.ReadAllData();
            _addressBook = new AddressBook();
            foreach (var address in addresses)
            {
                _addressBook.addresses.Add(address);
            }

            /*if (addresses is not null)
            {
                _addressBook.addresses = addresses;
                *//*foreach (var address in addresses)
                {
                    _addressBook.Add
                }*//*
            }*/
        }
        public void AddAddress(Address address)
        {
            _dataPersistence.AddData(address);
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
