using Moq;
using NUnit.Framework;
using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;
using PitneyAddressBook.Repository;
using System.Threading.Tasks;

namespace Tests
{
    public class AddressBookRepositoryTests
    {
        private AddressBookRepository _sut;         // Cant be asigned in constructor to ensure tests are correct
        private readonly Mock<IDataPersistence> _dataPersistenceMock = new Mock<IDataPersistence>();

        [Test]
        public async Task AddAsyncShouldInvokeSaveDataAsync()
        {
            // Arrange
            AddressBook book = new AddressBook();
            Address addressSaved = GenerateAddress(1, "City");

            InitializeSubjectForTests(book);
            book.addresses.Add(addressSaved);
            _dataPersistenceMock.Setup(r => r.SaveDataAsync(book))
                .Verifiable();

            // Act
            await _sut.AddAsync(addressSaved);
            var result = await _sut.GetLastAsync();

            // Assert
            _dataPersistenceMock.Verify(r => r.SaveDataAsync(book), Times.Once);
            AssertAddressesAreEqual(addressSaved, result);
        }

        [Test]
        public async Task GetLastAsyncShouldReturnAddress()
        {
            // Arrange
            Address wrongAddress = GenerateAddress(1, "City");

            Address returnedAddress = GenerateAddress(2, "City2");

            AddressBook addressBook = new AddressBook();
            addressBook.addresses.Add(wrongAddress);
            addressBook.addresses.Add(returnedAddress);

            InitializeSubjectForTests(addressBook);

            // Act
            var result = await _sut.GetLastAsync();

            // Assert
            AssertAddressesAreEqual(returnedAddress, result);
        }

        [Test]
        public async Task GetLastAsyncShouldReturnNull()
        {
            // Arrange
            AddressBook addressBook = new();
            InitializeSubjectForTests(addressBook);

            // Act
            var result = await _sut.GetLastAsync();

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task GetByCityAsyncShouldReturnListWith2Addresses()
        {
            // Arrange
            string city = "City";
            Address returnedAddress1 = GenerateAddress(1, city);
            Address returnedAddress2 = GenerateAddress(2, city);

            Address notReturnedAddress = GenerateAddress(3, "WrongAddress");

            AddressBook book = new AddressBook();
            book.addresses.Add(returnedAddress1);
            book.addresses.Add(returnedAddress2);
            book.addresses.Add(notReturnedAddress);
            InitializeSubjectForTests(book);

            // Act
            var result = await _sut.GetByCityAsync(city);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            AssertAddressesAreEqual(returnedAddress1, result[0]);
            AssertAddressesAreEqual(returnedAddress2, result[1]);
        }

        [Test]
        public async Task GetByCityAsyncShouldReturnEmptyList()
        {
            // Arrange
            string city = "City";
            Address wrongAddress1 = GenerateAddress(1, "WrongCity");
            Address wrongAddress2 = GenerateAddress(2, "WrongCity");
            AddressBook book = new();
            book.addresses.Add(wrongAddress1);
            book.addresses.Add(wrongAddress2);

            InitializeSubjectForTests(book);

            // Act
            var result = await _sut.GetByCityAsync(city);

            // Assert
            Assert.NotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void IdExistsShouldReturnTrueIfAddresContainingProvidedIdAlreadyExists()
        {
            // Arrange
            int existingId = 1;
            Address existingAddress = GenerateAddress(existingId, "City");

            AddressBook book = new AddressBook();
            book.addresses.Add(existingAddress);

            InitializeSubjectForTests(book);

            // Act
            var result = _sut.IdExists(existingId);

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void IdExistsShouldReturnFalseIfAddresContainingProvidedIdDoesntExist()
        {
            // Arrange
            int existingId = 1;
            int newId = 2;
            Address existingAddress = GenerateAddress(existingId, "City");
            AddressBook book = new AddressBook();
            book.addresses.Add(existingAddress);

            InitializeSubjectForTests(book);

            // Act
            var result = _sut.IdExists(newId);

            // Assert
            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCaseSource(nameof(ValidationDivideCases))]
        public void IsAddressValidShouldReturnFalseIfAtLeastOneOfAddressPropsIsNullOrEmpty(Address invalidAddress)
        {
            // Arrange
            AddressBook book = new AddressBook();
            InitializeSubjectForTests(book);

            // Act
            var result = _sut.IsAddressValid(invalidAddress);

            // Assert
            Assert.NotNull(result);
            Assert.IsFalse(result);
        }

        static Address[] ValidationDivideCases =
        {
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
        public void IsAddressValidShouldReturnTrueIfAddressIsValid()
        {
            // Arrange
            Address validAddress = GenerateAddress(1, "City");
            AddressBook book = new();
            InitializeSubjectForTests(book);

            // Act
            var result = _sut.IsAddressValid(validAddress);

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result);
        }

        public void InitializeSubjectForTests(AddressBook addressBook)
        {
            _dataPersistenceMock.Setup(r => r.ReadAllData())
                .Returns(addressBook);
            _sut = new AddressBookRepository(_dataPersistenceMock.Object);
        }

        public static void AssertAddressesAreEqual(Address orginal, Address returned)
        {
            Assert.NotNull(returned);
            Assert.AreEqual(orginal.AddressId, returned.AddressId);
            Assert.AreEqual(orginal.AddressName, returned.AddressName);
            Assert.AreEqual(orginal.City, returned.City);
            Assert.AreEqual(orginal.Street, returned.Street);
            Assert.AreEqual(orginal.StreetNumber, returned.StreetNumber);
            Assert.AreEqual(orginal.PostalCode, returned.PostalCode);
        }

        public static Address GenerateAddress(int id, string city)
        {
            Address result = new Address()
            {
                AddressId = id,
                AddressName = $"Name{id}",
                City = city,
                Street = $"Street{id}",
                StreetNumber = $"StreetNumber{id}",
                PostalCode = $"PostalCode{id}"
            };
            return result;
        }
    }
}
