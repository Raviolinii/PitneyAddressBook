using Microsoft.AspNetCore.Mvc;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;

namespace PitneyAddressBook.Controllers
{
    [Route("/addressbook")]
    public class AddressBookController : Controller
    {
        private readonly IAddressBookRepository _addressBookRepository;

        public AddressBookController(IAddressBookRepository addressBookRepository)
        {
            _addressBookRepository = addressBookRepository;
        }

        [HttpPost("addaddress")]
        public IActionResult AddToAddressBook([FromBody] Address address)
        {
            try
            {
                if (address == null)
                    return BadRequest();

                _addressBookRepository.AddAddress(address);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getlastaddress")]
        public Address? GetLastAddress()
        {
            var result = _addressBookRepository.GetLastAddress();

            return result;
        }

        [HttpGet("getwithcity")]
        public List<Address> GetAddressesWithCity(string city)
        {
            if (city is null or "")
                return new List<Address>();

            var result = _addressBookRepository.GetAddresses(city);
            return result;
        }
    }
}