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

        public async Task AddAsync(Address address)
        {
            _addressBook.addresses.Add(address);
            await _dataPersistence.SaveDataAsync(_addressBook);
        }

        public async Task<Address?> GetLastAsync()
        {
            if (_addressBook.addresses.Any())
                return await Task.Run(() =>
                {
                    return _addressBook.addresses.Last();
                });

            return null;
        }

        public async Task<List<Address>> GetByCityAsync(string city)
        {
            var result = await Task.Run(() =>
            {
                return _addressBook.addresses.Where(r => r.City == city).ToList();
            });
            return result;
        }

        public bool IdExists(int id)
        {
            if (_addressBook.addresses.Exists(r => r.AddressId == id))
                return true;

            else
                return false;
        }
        public bool IsAddressValid(Address address)
        {
            if (
                address.City is not null and not ""
                && address.AddressName is not null and not ""
                && address.Street is not null and not ""
                && address.StreetNumber is not null and not ""
                && address.PostalCode is not null and not ""
                )
                return true;
            else return false;
        }
    }
}
