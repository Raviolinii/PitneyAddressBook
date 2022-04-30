using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PitneyAddressBook.Controllers;
using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    public class AddressBookControllerTests
    {
        private readonly DataPersistence _dataPersistence;

        public AddressBookControllerTests()
        {
            var testConfig = new Dictionary<string, string>()
            {
                {"BackupFolder:AddressBook", "TestingJson/AddressBook.json"}
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(testConfig).Build();
            _dataPersistence = new(configuration);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("NotIncludedCity")]
        public void GetByCityWhenNoAddressWasGivenOrNotFoundShouldReturnEmptyList(string? value)
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var receivedAddresses = controller.GetAddressesByCity(value);

            // Assert
            Assert.That(receivedAddresses, Is.Empty);
        }

        [Test]
        public void GetByCityShouldReturnListWithOneAddress()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var receivedAddresses = controller.GetAddressesByCity("City2");

            // Assert
            //Assert.That(receivedAddresses.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetByCityShouldReturnListWithTwoAddress()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var receivedAddresses = controller.GetAddressesByCity("City") as ObjectResult;

            // Assert
            List<Address> list = receivedAddresses.Value as List<Address>;
            Assert.That(list.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetLastAddressShouldReturnAddressWithId3()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var receivedAddress = controller.GetLastAddress() as ObjectResult;

            // Asserrt
            Address result = receivedAddress.Value as Address;
            Assert.That(result.AddressId, Is.EqualTo(3));
        }

        [Test]
        public void GetLastAddressShouldReturnNullIfListIsEmpty()
        {
            // Arrange
            var controller = GenerateControllerWithNoAddresses();

            // Act
            var receivedAddress = controller.GetLastAddress() as ObjectResult;

            // Assert
            Assert.That(receivedAddress.Value, Is.Null);
        }

        [Test]
        public async Task AddToAddressBookShoudlReturnBadRequestIfIdAlreadyExists()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();
            Address addressWithDuplicatedId = new() { AddressId = 1, AddressName = "Name",
                City = "City", Street = "Street", StreetNumber = "Number", PostalCode = "Code" };

            // Act
            var actionResult = await controller.AddToAddressBook(addressWithDuplicatedId) as ObjectResult;
            var res = actionResult;
            // Assert
            Assert.That(res.StatusCode, Is.EqualTo(((int)HttpStatusCode.BadRequest)));
        }

        AddressBookController GenerateControllerWithNoAddresses()
        {
            List<Address> addresses = new List<Address>();
            PopulateTestAddressBook(addresses);
            var repository = GenerateRepository();
            var controller = new AddressBookController(repository, new NullLogger<AddressBookController>());
            return controller;
        }
        AddressBookController GenerateController()
        {
            var repository = GenerateRepository();
            var controller = new AddressBookController(repository, new NullLogger<AddressBookController>());
            return controller;
        }
        AddressBookController GenerateControllerWithAddresses()
        {
            List<Address> addresses = GenerateListOfAddresses();
            PopulateTestAddressBook(addresses);
            var controller = GenerateController();
            return controller;
        }

        AddressBookRepository GenerateRepository()
        {
            AddressBookRepository result = new(_dataPersistence);
            return result;
        }

        List<Address> GenerateListOfAddresses()
        {
            List<Address> addresses = new List<Address>()
            {
                new Address(){AddressId = 1, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "Number", PostalCode = "Code"},
                new Address(){AddressId = 2, AddressName = "Name1", City = "City", Street = "Street1", StreetNumber = "Number1", PostalCode = "Code1"},
                new Address(){AddressId = 3, AddressName = "Name2", City = "City2", Street = "Street2", StreetNumber = "Number2", PostalCode = "Code2"}
            };
            return addresses;
        }

        void PopulateTestAddressBook(List<Address> addresses)
        {
            AddressBook addressBook = new();
            addressBook.addresses = addresses;
            _dataPersistence.SaveData(addressBook);
        }

    }
}
