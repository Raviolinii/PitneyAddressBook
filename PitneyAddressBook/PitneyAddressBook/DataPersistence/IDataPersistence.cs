using PitneyAddressBook.Models;

namespace PitneyAddressBook.DataPersistence
{
    public interface IDataPersistence
    {
        public Task SaveData(AddressBook data);
        public AddressBook ReadAllData();
    }
}
