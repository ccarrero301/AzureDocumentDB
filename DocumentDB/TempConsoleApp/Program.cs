using System.Threading.Tasks;
using IntegrationTests.Documents;
using Microsoft.Azure.Cosmos;

namespace TempConsoleApp
{
    internal class Program
    {
        private static readonly string _cosmosDbEndpointUri = "https://localhost:8081";
        private static readonly string _cosmosDbAccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static readonly string _databaseName = "People";
        private static readonly string _collectionName = "PeopleCollection";

        public static async Task Main(string[] args)
        {
            await GetByIdAsync<Person>("Carrero", "2").ConfigureAwait(false);
        }

        public static async Task GetByIdAsync<TDocument>(string partitionKey, string documentId)
        {
            try
            {
                using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
                {
                    var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                    var entityResponse = await container.ReadItemAsync<TDocument>(partitionKey: new PartitionKey(partitionKey), id: documentId).ConfigureAwait(false);
                }
            }
            catch (CosmosException cosmosException) { }
        }
    }
}