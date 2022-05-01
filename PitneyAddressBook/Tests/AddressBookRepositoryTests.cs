using Moq;
using NUnit.Framework;
using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AddressBookRepositoryTests
    {
        private AddressBookRepository _sut;
        private readonly Mock<IDataPersistence> _dataPersistenceMock = new Mock<IDataPersistence>();

        [Test]
        public async Task GetLastAsyncShouldReturnAddress()
        {
            // Arrange
            Address wrongAddress = new Address()
            {
                AddressId = 1,
                AddressName = "Name",
                City = "City",
                Street = "Street",
                StreetNumber = "StreetNum",
                PostalCode = "PostalCode"
            };
            Address returnedAddress = new Address()
            {
                AddressId = 2,
                AddressName = "Name2",
                City = "City2",
                Street = "Street2",
                StreetNumber = "StreetNum2",
                PostalCode = "PostalCode2"
            };

            AddressBook addressBook = new AddressBook();
            addressBook.addresses.Add(wrongAddress);
            addressBook.addresses.Add(returnedAddress);

            _dataPersistenceMock.Setup(r => r.ReadAllData())
                    .Returns(addressBook);
            _sut = new AddressBookRepository(_dataPersistenceMock.Object);

            // Act
            var result = await _sut.GetLastAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.AddressId, returnedAddress.AddressId);
            Assert.AreEqual(result.AddressName, returnedAddress.AddressName);
            Assert.AreEqual(result.City, returnedAddress.City);
            Assert.AreEqual(result.Street, returnedAddress.Street);
            Assert.AreEqual(result.StreetNumber, returnedAddress.StreetNumber);
            Assert.AreEqual(result.PostalCode, returnedAddress.PostalCode);

        }
    }
}
