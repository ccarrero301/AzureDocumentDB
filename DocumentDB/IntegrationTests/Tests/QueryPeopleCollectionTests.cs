using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Entities;
using IntegrationTests.Mappings;
using IntegrationTests.Specifications;
using NUnit.Framework;

namespace IntegrationTests.Tests
{
    public class QueryPeopleCollectionTests
    {
        private string _collectionName;
        private CommandCosmosDbRepository<Person, Documents.Person> _commandCosmosDbRepository;
        private string _cosmosDbEndpointUri;
        private string _cosmosDbPrimaryKey;
        private string _databaseName;

        private Profile _mappingProfile;
        private List<Documents.Person> _peopleListToTest;
        private QueryCosmosDbRepository<Person> _queryCosmosDbRepository;

        [SetUp]
        public Task SetupAsync()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbPrimaryKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _peopleListToTest = new List<Documents.Person>();

            _mappingProfile = new MappingProfile();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Person>(_cosmosDbEndpointUri, _cosmosDbPrimaryKey,
                _databaseName, _collectionName, _mappingProfile);

            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbPrimaryKey, _databaseName, _collectionName, _mappingProfile);

            return AddDocumentsToTestAsync();
        }

        [TearDown]
        public Task TearDownAsync() => DeleteDocumentsToTestAsync();

        [Test]
        public async Task GetNotExistentDocumentByIdAndPartitionKey()
        {
            const string documentId = "-1";
            const string partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Person>(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey == null);
        }

        [Test]
        public async Task GetDocumentByIdAndPartitionKey()
        {
            const string documentId = "1";
            const string partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Person>(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey != null);
            Assert.IsTrue(string.CompareOrdinal(personByIdAndPartitionKey.FamilyName, "Carrero") == 0);
            Assert.IsTrue(string.CompareOrdinal(personByIdAndPartitionKey.FirstName, "Carlos") == 0);
            Assert.IsTrue(string.CompareOrdinal(personByIdAndPartitionKey.MiddleName, "Andres") == 0);
        }

        [Test]
        public void GetDocumentsBySpecification()
        {
            var carlosFirstNameSpecification = new FirstNameSpecification("Carlos");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = _queryCosmosDbRepository
                .GetBySpecification(carlosFirstNameSpecification, partitionKey).ToList();

            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FamilyName, "Carrero") ==
                          0);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FirstName, "Carlos") ==
                          0);
            Assert.IsTrue(
                string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().MiddleName, "Andres") == 0);
        }

        [Test]
        public async Task GetPaginatedResultsByExpressionSpecification()
        {
            var carlosFirstNameSpecification = new FirstNameSpecification("Carlos");
            const string partitionKey = "Carrero";

            var (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(carlosFirstNameSpecification, partitionKey)
                .ConfigureAwait(false);

            Assert.IsTrue(continuationToken == null);
            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FamilyName, "Carrero") ==
                          0);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FirstName, "Carlos") ==
                          0);
            Assert.IsTrue(
                string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().MiddleName, "Andres") == 0);
        }

        private static Documents.Person CreateDocument(string id, string firstName, string middleName,
            string familyName) =>
            new Documents.Person
            {
                Id = id,
                FirstName = firstName,
                MiddleName = middleName,
                FamilyName = familyName
            };

        private async Task AddDocumentsToTestAsync()
        {
            _peopleListToTest.Add(CreateDocument("1", "Carlos", "Andres", "Carrero"));
            _peopleListToTest.Add(CreateDocument("2", "Luis", "Miguel", "Carrero"));
            _peopleListToTest.Add(CreateDocument("3", "Beatriz", "Elena", "Carrero"));

            foreach (var personDocument in _peopleListToTest)
            {
                await _commandCosmosDbRepository.AddDocumentAsync(personDocument, personDocument.FamilyName)
                    .ConfigureAwait(false);
            }
        }

        private async Task DeleteDocumentsToTestAsync()
        {
            foreach (var personDocument in _peopleListToTest)
            {
                await _commandCosmosDbRepository.DeleteDocumentAsync(personDocument.Id, personDocument.FamilyName)
                    .ConfigureAwait(false);
            }
        }
    }
}