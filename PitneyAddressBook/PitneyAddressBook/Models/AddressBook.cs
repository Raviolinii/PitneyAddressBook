namespace PitneyAddressBook.Models
{
    public class AddressBook
    {
        public List<Address> addresses { get; set; }
        public AddressBook()
        {
            addresses = new List<Address>();
        }
    }
}
