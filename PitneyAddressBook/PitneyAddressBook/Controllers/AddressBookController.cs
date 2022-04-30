using Microsoft.AspNetCore.Mvc;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;

namespace PitneyAddressBook.Controllers
{
    [Route("/addressbook")]
    public class AddressBookController : Controller
    {
        private readonly IAddressBookRepository _addressBookRepository;
        private readonly ILogger<AddressBookController> _logger;

        public AddressBookController(IAddressBookRepository addressBookRepository, ILogger<AddressBookController> logger)
        {
            _addressBookRepository = addressBookRepository;
            _logger = logger;
        }

        [HttpPost("addaddress")]
        public IActionResult AddToAddressBook([FromBody] Address address)
        {
            try
            {
                if (_addressBookRepository.IdExists(address.AddressId))
                {
                    _logger.LogError($"Address with provided Id ({address.AddressId}) already exists in Address Book");
                    return BadRequest();
                }
                if (!_addressBookRepository.IsAddressValid(address))
                {
                    _logger.LogError("Address sent is invalid");
                    return BadRequest();
                }

                _addressBookRepository.AddAddress(address);
                _logger.LogInformation($"Address with Id: {address.AddressId}'s been added successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught in AddToAddressBook(ADdress address) in AddressBookController");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getlastaddress")]
        public Address? GetLastAddress()
        {
            var result = _addressBookRepository.GetLastAddress();
            _logger.LogInformation("Last address request");
            return result;
        }

        [HttpGet("getwithcity")]
        public List<Address> GetAddressesWithCity(string city)
        {
            if (city is null or "")
            {
                _logger.LogInformation("Requested addresses with unspecified city (null or empty string)");
                return new List<Address>();
            }

            _logger.LogInformation($"Requested addresses containing city {city}");
            var result = _addressBookRepository.GetAddresses(city);
            return result;
        }
    }
}