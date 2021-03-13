namespace DocumentDB.Implementations.Utils
{
    public class CosmosDbConfiguration
    {
        public CosmosDbConfiguration(string endpoint, string accessKey, string databaseName, string collectionName)
        {
            Endpoint = endpoint;
            AccessKey = accessKey;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public string Endpoint { get; }

        public string AccessKey { get; }

        public string DatabaseName { get; }

        public string CollectionName { get; }
    }
}