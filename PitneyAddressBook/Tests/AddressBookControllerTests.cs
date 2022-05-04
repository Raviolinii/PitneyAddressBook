using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PitneyAddressBook.Controllers;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AddressBookControllerTests
    {
        private readonly AddressBookController _sut;
        private readonly Mock<IAddressBookRepository> _addressBookRepositoryMock = new Mock<IAddressBookRepository>();

        public AddressBookControllerTests()
        {
            _sut = new AddressBookController(_addressBookRepositoryMock.Object, new NullLogger<AddressBookController>());
        }

        [Test]
        public async Task GetLastAddressShouldReturnNullIfListIsEmpty()
        {
            Address? returnedAddress = null;
            // Arrange
            _addressBookRepositoryMock.Setup(r => r.GetLastAsync())
                .ReturnsAsync(returnedAddress);

            // Act
            var result = await _sut.GetLast() as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.Null(result.Value);
        }

        [Test]
        public async Task GetLastAddressShouldReturnLastAddressIfAddressExists()
        {
            // Arrange
            Address? returnedAddress = GenerateSingleAddress();
            _addressBookRepositoryMock.Setup(r => r.GetLastAsync())
                .ReturnsAsync(returnedAddress);

            // Act
            var result = await _sut.GetLast() as ObjectResult;
            Address? resultAddress = (Address?)result.Value;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);


            AddressBookRepositoryTests.AssertAddressesAreEqual(resultAddress, returnedAddress);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task GetByCityWhenNoAddressWasGivenOrNotFoundShouldReturnEmptyList(string? value)
        {
            // Arrange
            List<Address> wrongResult = GenerateListWithTwoAddresses();

            _addressBookRepositoryMock.Setup(r => r.GetByCityAsync(value))
                .ReturnsAsync(wrongResult);

            // Act
            var result = await _sut.GetByCity(value) as ObjectResult;
            List<Address> resultList = (List<Address>)result.Value;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            Assert.NotNull(resultList);
            Assert.IsEmpty(resultList);
        }

        [Test]
        public async Task GetByCityShouldReturnListWithAddresses()
        {
            // Arrange
            List<Address> returnedList = GenerateListWithTwoAddresses();
            _addressBookRepositoryMock.Setup(r => r.GetByCityAsync("City"))
                .ReturnsAsync(returnedList);

            // Act
            var result = await _sut.GetByCity("City") as ObjectResult;
            var resultList = (List<Address>)result.Value;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            Assert.NotNull(resultList);
            Assert.AreEqual(2, resultList.Count);


            for (int i = 0; i < 2; i++)
            {
                AddressBookRepositoryTests.AssertAddressesAreEqual(returnedList[i], resultList[i]);
            }
        }

        [Test]
        public async Task AddToAddressBookShoudlReturnBadRequestIfProvidedIdExists()
        {
            // Arrange
            Address toAdd = GenerateSingleAddress();
            _addressBookRepositoryMock.Setup(r => r.IdExists(toAdd.AddressId))
                .Returns(true);
            _addressBookRepositoryMock.Setup(r => r.IsAddressValid(toAdd))
                .Returns(true);


            // Act
            var result = await _sut.AddToAddressBook(toAdd) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task AddToAddressBookShoudlReturnBadRequestIfOneOfAddressPropsAreEmptyOrNull()
        {
            // Arrange
            Address toAdd = GenerateSingleAddress();
            _addressBookRepositoryMock.Setup(r => r.IdExists(toAdd.AddressId))
                .Returns(false);
            _addressBookRepositoryMock.Setup(r => r.IsAddressValid(toAdd))
                .Returns(false);

            // Act
            var result = await _sut.AddToAddressBook(toAdd) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task AddToAddressBookShoudlReturnOkIfAddeddSuccessfully()
        {
            // Arrange
            Address toAdd = GenerateSingleAddress();
            _addressBookRepositoryMock.Setup(r => r.IdExists(toAdd.AddressId))
                .Returns(false);
            _addressBookRepositoryMock.Setup(r => r.IsAddressValid(toAdd))
                .Returns(true);

            // Act
            var result = await _sut.AddToAddressBook(toAdd) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }


        List<Address> GenerateListWithTwoAddresses()
        {
            List<Address> result = new List<Address>()
            {
                new Address(){AddressId = 1, AddressName = "Name", City = "City",
                    Street = "Street", StreetNumber = "StreetNum", PostalCode = "PostalCode"},

                new Address(){AddressId = 2, AddressName = "Name2", City = "City",
                    Street = "Street2", StreetNumber = "StreetNum2", PostalCode = "PostalCode2"},
            };
            return result;
        }

        Address GenerateSingleAddress()
        {
            var result = new Address()
            {
                AddressId = 1,
                AddressName = "Name",
                City = "City",
                Street = "Street",
                StreetNumber = "StreetNum",
                PostalCode = "PostalCode"
            };

            return result;
        }
    }
}
