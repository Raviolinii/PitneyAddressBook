using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using PitneyAddressBook.DataPersistence;
using PitneyAddressBook.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class DataPersistenceTests
    {
        private readonly DataPersistence _sut;
        private readonly string _path = "TestingJson/AddressBook.json";

        public DataPersistenceTests()
        {
            var testConfig = new Dictionary<string, string>()
            {
                {"PersistentData:AddressBook", _path}
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(testConfig).Build();

            _sut = new DataPersistence(configuration);
        }

        [Test]
        public void ReadAllDataShouldReturnNewAddressBookIfDeserializedObjectIsNull()
        {
            // Arrange
            AddressBook serializedAddressBook = new();
            string jsonToSave = JsonConvert.SerializeObject(serializedAddressBook, Formatting.Indented);

            using (StreamWriter file = new(_path))
            {
                file.Write(jsonToSave);
            }

            // Act
            var result = (AddressBook)_sut.ReadAllData();

            // Assert
            Assert.NotNull(result);
            Assert.Zero(result.addresses.Count);
        }

        [Test]
        public void ReadAllDataShoultReturnAddressBookWithOneAddress()
        {
            // Arrange
            AddressBook serializedAddressBook = new();
            Address serializedAddress = AddressBookRepositoryTests.GenerateAddress(1, "City");
            serializedAddressBook.addresses.Add(serializedAddress);
            string jsonToSave = JsonConvert.SerializeObject(serializedAddressBook, Formatting.Indented);
            
            using (StreamWriter file = new(_path))
            {
                file.Write(jsonToSave);
            }

            // Act
            var result = _sut.ReadAllData();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.addresses.Count);
            AddressBookRepositoryTests.AssertAddressesAreEqual(serializedAddress, result.addresses.First());
        }

        [Test]
        public async Task SaveDataAsyncShouldSaveWholeAddressBook()
        {
            // Arrange
            AddressBook addressBook = new();
            Address address1 = AddressBookRepositoryTests.GenerateAddress(1, "City1");
            Address address2 = AddressBookRepositoryTests.GenerateAddress(2, "City2");
            addressBook.addresses.Add(address1);
            addressBook.addresses.Add(address2);

            // Act
            await _sut.SaveDataAsync(addressBook);

            var json = File.ReadAllText(_path);
            var result = JsonConvert.DeserializeObject<AddressBook>(json);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addressBook.addresses.Count, result.addresses.Count);
            for (int i = 0; i < addressBook.addresses.Count; i++)
            {
                AddressBookRepositoryTests.AssertAddressesAreEqual(addressBook.addresses[i], result.addresses[i]);
            }
        }
    }
}