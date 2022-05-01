using Newtonsoft.Json;
using PitneyAddressBook.Models;

namespace PitneyAddressBook.DataPersistence
{
    public class DataPersistence : IDataPersistence
    {
        string _addressBookPath;
        public DataPersistence(IConfiguration configuration)
        {
            _addressBookPath = configuration.GetValue<string>("PersistentData:AddressBook");
        }

        public async Task SaveDataAsync(AddressBook data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (StreamWriter file = new(_addressBookPath))
            {
                await file.WriteAsync(json);
            }
        }
        public void SaveData(AddressBook data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (StreamWriter file = new(_addressBookPath))
            {
                file.Write(json);
            }
        }

        public AddressBook ReadAllData()
        {            
            var json = File.ReadAllText(_addressBookPath);
            var result = JsonConvert.DeserializeObject<AddressBook>(json); 
            if (result is null)
                return new AddressBook();

            return result;
        }
    }
}
