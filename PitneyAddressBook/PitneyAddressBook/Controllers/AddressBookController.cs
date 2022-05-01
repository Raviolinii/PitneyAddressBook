﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddToAddressBook([FromBody] Address address)
        {
            try
            {
                if (_addressBookRepository.IdExists(address.AddressId))
                {
                    _logger.LogError($"Address with provided Id ({address.AddressId}) already exists in Address Book");
                    return BadRequest("Provided Id already taken");
                }
                if (!_addressBookRepository.IsAddressValid(address))
                {
                    _logger.LogError("Address sent is invalid");
                    return BadRequest("Invalid Address");
                }

                await _addressBookRepository.AddAddressAsync(address);
                _logger.LogInformation($"Address with Id: {address.AddressId}'s been added successfully");
                return Ok("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught in AddToAddressBook in AddressBookController");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getlastaddress")]
        public async Task<IActionResult> GetLastAddress()
        {
            var result = await _addressBookRepository.GetLastAddressAsync();
            _logger.LogInformation("Last address request");
            return Ok(result);
        }

        [HttpGet("getbycity")]
        public async Task<IActionResult> GetAddressesByCity(string city)
        {
            if (city is null or "")
            {
                _logger.LogInformation("Requested addresses with unspecified city (null or empty string)");
                List<Address> empty = new();
                return Ok(empty);
            }

            _logger.LogInformation($"Requested addresses containing city {city}");
            var result = await _addressBookRepository.GetAddressesAsync(city);
            return Ok(result);
        }
    }
}