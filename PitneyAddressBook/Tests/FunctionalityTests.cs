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
    public class FunctionalityTests
    {
        private readonly DataPersistence _dataPersistence;

        public FunctionalityTests()
        {
            var testConfig = new Dictionary<string, string>()
            {
                {"PersistentData:AddressBook", "TestingJson/AddressBook.json"}
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(testConfig).Build();
            _dataPersistence = new(configuration);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("NotIncludedCity")]
        public async Task GetByCityWhenNoAddressWasGivenOrNotFoundShouldReturnEmptyList(string? value)
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var actionResult = await controller.GetByCity(value) as ObjectResult;
            var result = actionResult.Value as List<Address>;

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByCityShouldReturnListWithOneAddress()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var actionResult = await controller.GetByCity("City2") as ObjectResult;
            var result = actionResult.Value as List<Address>;

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByCityShouldReturnListWithTwoAddresses()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var actionResult = await controller.GetByCity("City") as ObjectResult;
            List<Address> list = actionResult.Value as List<Address>;

            // Assert
            Assert.That(list.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetLastAddressShouldReturnAddressWithId3()
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var actionResult = await controller.GetLast() as ObjectResult;
            Address result = actionResult.Value as Address;

            // Asserrt
            Assert.That(result.AddressId, Is.EqualTo(3));
        }

        [Test]
        public async Task GetLastAddressShouldReturnNullIfListIsEmpty()
        {
            // Arrange
            var controller = GenerateControllerWithNoAddresses();

            // Act
            var actionResult = await controller.GetLast() as ObjectResult;

            // Assert
            Assert.That(actionResult.Value, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(ValidationDivideCases))]
        public async Task AddToAddressBookShoudlReturnBadRequestIfProvidedIdExistsOrOneOfAddressPropsAreEmptyOrNull(Address invalidAddress)
        {
            // Arrange
            var controller = GenerateControllerWithAddresses();

            // Act
            var actionResult = await controller.AddToAddressBook(invalidAddress) as ObjectResult;

            // Assert
            Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        static Address[] ValidationDivideCases =
        {
            new Address {AddressId = 1, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "", City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = null, City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "", Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = null, Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = "", StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = null, StreetNumber = "StreetNum", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "", PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = "Street", StreetNumber = null, PostalCode = "PostalCode"},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = ""},
            new Address {AddressId = 0, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = null},
        };

        [Test]
        public async Task AddToAddressBookShoudlReturnOkIfAddeddSuccessfully()
        {
            // Arrange
            var controller = GenerateControllerWithNoAddresses();
            var address = new Address() { AddressId = 0, AddressName = "Name", City = "City", Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode" };

            // Act
            var actionResult = await controller.AddToAddressBook(address) as ObjectResult;

            // Assert
            Assert.That(actionResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
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
            _dataPersistence.SaveDataAsync(addressBook).Wait();
        }
    }
}
