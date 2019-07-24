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
        private string _cosmosDbAccessKey;
        private string _cosmosDbEndpointUri;
        private string _databaseName;

        private Profile _mappingProfile;
        private List<Documents.Person> _peopleListToTest;
        private QueryCosmosDbRepository<Person> _queryCosmosDbRepository;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbAccessKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _mappingProfile = new MappingProfile();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Person>(_cosmosDbEndpointUri, _cosmosDbAccessKey,
                _databaseName, _collectionName, _mappingProfile);

            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);

            _peopleListToTest = await IntegrationTestsUtils.AddDocumentListToTestAsync(_commandCosmosDbRepository);
        }

        [OneTimeTearDown]
        public Task TearDownAsync() => IntegrationTestsUtils.DeleteDocumentListToTestAsync(_commandCosmosDbRepository, _peopleListToTest);

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
            Assert.IsTrue(personByIdAndPartitionKey.FamilyName == "Carrero");
            Assert.IsTrue(personByIdAndPartitionKey.FirstName == "Carlos");
            Assert.IsTrue(personByIdAndPartitionKey.MiddleName == "Andres");
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
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().MiddleName == "Andres");
        }

        [Test]
        public async Task GetPaginatedResultsByExpressionSpecification()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey)
                .ConfigureAwait(false);

            Assert.IsTrue(continuationToken == null);
            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 3);
        }

        [Test]
        public async Task GetPageOneResultsByExpressionSpecification()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 1, 1)
                .ConfigureAwait(false);

            Assert.IsTrue(continuationToken != null);
            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FirstName == "Carlos");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().MiddleName == "Andres");
        }

        [Test]
        public async Task GetPageTwoResultsByExpressionSpecification()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 1, 1)
                .ConfigureAwait(false);

            (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 2, 1, continuationToken)
                .ConfigureAwait(false);

            Assert.IsTrue(continuationToken != null);
            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FirstName == "Luis");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().MiddleName == "Miguel");
        }

        [Test]
        public async Task GetPageThreeResultsByExpressionSpecification()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 1, 1)
                .ConfigureAwait(false);

            (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 2, 1, continuationToken)
                .ConfigureAwait(false);

            (continuationToken, documentsBySpecificationList) = await _queryCosmosDbRepository
                .GetPaginatedResultsBySpecificationAsync(familyNameSpecification, partitionKey, 3, 1, continuationToken)
                .ConfigureAwait(false);

            Assert.IsTrue(continuationToken == null);
            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FamilyName == "Carrero");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().FirstName == "Beatriz");
            Assert.IsTrue(documentsBySpecificationList.FirstOrDefault().MiddleName == "Elena");
        }
    }
}