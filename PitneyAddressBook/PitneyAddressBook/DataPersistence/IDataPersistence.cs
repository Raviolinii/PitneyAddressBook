using PitneyAddressBook.Models;

namespace PitneyAddressBook.DataPersistence
{
    public interface IDataPersistence
    {
        public void SaveData(AddressBook data);
        public AddressBook ReadAllData();
    }
}
