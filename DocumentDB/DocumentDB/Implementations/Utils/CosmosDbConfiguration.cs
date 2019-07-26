using AutoMapper;

namespace DocumentDB.Implementations.Utils
{
    public class CosmosDbConfiguration
    {
        public CosmosDbConfiguration(string endpoint, string accessKey, string databaseName, string collectionName, Profile mappingProfile)
        {
            Endpoint = endpoint;
            AccessKey = accessKey;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            MappingProfile = mappingProfile;
        }

        public string Endpoint { get; }

        public string AccessKey { get; }

        public string DatabaseName { get; }

        public string CollectionName { get; }

        public Profile MappingProfile { get; }
    }
}