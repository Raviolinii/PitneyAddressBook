namespace PitneyAddressBook.DataPersistence
{
    public interface IDataPersistence<T>
    {
        public void AddData(T data);
        public List<T> ReadAllData();
    }
}
