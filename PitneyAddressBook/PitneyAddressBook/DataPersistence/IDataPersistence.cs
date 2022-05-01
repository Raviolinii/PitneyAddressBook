using PitneyAddressBook.Models;

namespace PitneyAddressBook.DataPersistence
{
    public interface IDataPersistence
    {
        public Task SaveDataAsync(AddressBook data);
        public AddressBook ReadAllData();
    }
}
