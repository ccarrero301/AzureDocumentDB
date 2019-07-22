using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Documents;
using IntegrationTests.Mappings;
using NUnit.Framework;

namespace IntegrationTests
{
    public class PeopleCollectionTests
    {
        private string _collectionName;
        private string _cosmosDbEndpointUri;
        private string _cosmosDbPrimaryKey;
        private string _databaseName;

        private IMapper _mapper;
        private QueryCosmosDbRepository<Person> _queryCosmosDbRepository;

        [SetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbPrimaryKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _mapper = MappingConfiguration.Configure();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Person>(_cosmosDbEndpointUri, _cosmosDbPrimaryKey,
                _databaseName, _collectionName, _mapper);
        }

        [Test]
        public async Task GetDocumentByIdAndPartitionKey()
        {
            var documentId = "1";
            var partitionKey = "Carrero";

            var personByIdAndPartitionKey = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Person>(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(personByIdAndPartitionKey != null);
        }
    }
}