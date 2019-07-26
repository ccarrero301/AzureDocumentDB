using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Documents;
using IntegrationTests.Mappings;
using IntegrationTests.Specifications;
using NUnit.Framework;

namespace IntegrationTests.Tests
{
    public class QueryPeopleCollectionTests
    {
        private string _collectionName;
        private string _cosmosDbAccessKey;
        private string _cosmosDbEndpointUri;
        private string _databaseName;
        private Profile _mappingProfile;
        private List<Person> _peopleListToTest;
        private QueryCosmosDbRepository<Entities.Person, Person> _queryCosmosDbRepository;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbAccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _mappingProfile = new MappingProfile();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Entities.Person, Person>(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);

            _peopleListToTest = await IntegrationTestsUtils.AddDocumentListToTestAsync(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public Task TearDownAsync() =>
            IntegrationTestsUtils.DeleteDocumentListToTestAsync(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile, _peopleListToTest);

        [Test]
        public async Task GetNotExistentDocumentByIdAndPartitionKey()
        {
            const string documentId = "-1";
            const string partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository.GetByIdAsync(partitionKey, documentId).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey == null);
        }

        [Test]
        public async Task GetDocumentByIdAndPartitionKey()
        {
            const string documentId = "1";
            const string partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository.GetByIdAsync(partitionKey, documentId).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey != null);
            Assert.IsTrue(personByIdAndPartitionKey.FamilyName == "Carrero");
            Assert.IsTrue(personByIdAndPartitionKey.FirstName == "Carlos");
            Assert.IsTrue(personByIdAndPartitionKey.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecification()
        {
            var carlosFirstNameSpecification = new FirstNameSpecification("Carlos");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, carlosFirstNameSpecification).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, familyNameSpecification, 1, 1).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count == 1);
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeTwo()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, familyNameSpecification, 1, 2).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count == 2);
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.MiddleName == "Andres");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.FirstName == "Luis");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.MiddleName == "Miguel");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeThree()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, familyNameSpecification, 1, 3).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count == 3);
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.MiddleName == "Andres");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.FirstName == "Luis");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(1)?.MiddleName == "Miguel");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(2)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(2)?.FirstName == "Miguel");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(2)?.MiddleName == "Antonio");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageTwoPageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, familyNameSpecification, 2, 1).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count == 1);
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FirstName == "Luis");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.MiddleName == "Miguel");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageThreePageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = (await _queryCosmosDbRepository.GetBySpecificationAsync(partitionKey, familyNameSpecification, 3, 1).ConfigureAwait(false)).ToList();

            Assert.IsTrue(documentsBySpecificationList.Count == 1);
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.FirstName == "Miguel");
            Assert.IsTrue(documentsBySpecificationList.ElementAt(0)?.MiddleName == "Antonio");
        }
    }
}