using Newtonsoft.Json;

namespace PitneyAddressBook.DataPersistence
{
    public class DataPersistence<T> : IDataPersistence<T>
    {
        public void AddData(T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (StreamWriter file = new("BookAddress.json", append: true))
            {
                file.WriteAsync(json);
            }
        }

        public List<T> ReadAllData()
        {
            var json = File.ReadAllText("BookAddress.json");
            var result = JsonConvert.DeserializeObject<List<T>>(json);
            if (result is null)
                return new List<T>();

            return result;
        }
    }
}
