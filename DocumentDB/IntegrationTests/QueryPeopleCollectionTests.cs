using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Documents;
using IntegrationTests.Mappings;
using IntegrationTests.Specifications;
using NUnit.Framework;

namespace IntegrationTests
{
    public class QueryPeopleCollectionTests
    {
        private string _collectionName;
        private string _cosmosDbEndpointUri;
        private string _cosmosDbPrimaryKey;
        private string _databaseName;

        private Profile _mappingProfile;
        private QueryCosmosDbRepository<Person> _queryCosmosDbRepository;

        [SetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbPrimaryKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _mappingProfile = new MappingProfile();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Person>(_cosmosDbEndpointUri, _cosmosDbPrimaryKey,
                _databaseName, _collectionName, _mappingProfile);
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
        public async Task GetNotExistentDocumentByIdAndPartitionKey()
        {
            const string documentId = "-1";
            const string partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Person>(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey == null);
        }

        [Test]
        public void GetDocumentBySpecification()
        {
            var carlosFirstNameSpecification = new FirstNameSpecification("Carlos");
            const string partitionKey = "Carrero";

            var documentsBySpecificationList = _queryCosmosDbRepository.GetBySpecification(carlosFirstNameSpecification, partitionKey).ToList();

            Assert.IsTrue(documentsBySpecificationList.Any());
            Assert.IsTrue(documentsBySpecificationList.Count() == 1);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FamilyName, "Carrero") ==
                          0);
            Assert.IsTrue(string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().FirstName, "Carlos") ==
                          0);
            Assert.IsTrue(
                string.CompareOrdinal(documentsBySpecificationList.FirstOrDefault().MiddleName, "Andres") == 0);
        }
    }
}