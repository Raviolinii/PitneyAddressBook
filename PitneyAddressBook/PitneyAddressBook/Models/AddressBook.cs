namespace PitneyAddressBook.Models
{
    public class AddressBook
    {
        public List<Address> addresses { get; set; }
        public Address? LastAddress { get; set; }

        public AddressBook()
        {
            addresses = new List<Address>();
            LastAddress = null;
        }
    }
}
