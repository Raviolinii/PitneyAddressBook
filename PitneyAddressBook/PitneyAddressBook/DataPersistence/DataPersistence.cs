//using System.Text.Json;

using Newtonsoft.Json;

namespace PitneyAddressBook.DataPersistence
{
    public class DataPersistence : IDataPersistence
    {
        public async Task AddData(object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (StreamWriter file = new("BookAddress.json", append: true))
            {
                await file.WriteAsync(json);
            }
        }

        public Task ReadAllData()
        {
            throw new NotImplementedException();
        }
    }
}
