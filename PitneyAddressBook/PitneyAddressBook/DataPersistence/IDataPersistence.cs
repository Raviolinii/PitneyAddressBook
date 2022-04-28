namespace PitneyAddressBook.DataPersistence
{
    public interface IDataPersistence
    {
        public Task AddData(Object data);
        public Task ReadAllData();
    }
}
