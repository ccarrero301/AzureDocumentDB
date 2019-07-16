using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDB.Utilities
{
    internal static class CosmosDbUtilities
    {
        internal static Uri CreateDocumentCollectionUri(string databaseName, string collectionName) =>
            UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

        internal static DocumentClient CreateDocumentClient(string cosmosDbEndpointUri, string cosmosDbPrimaryKey) =>
            new DocumentClient(new Uri(cosmosDbEndpointUri), cosmosDbPrimaryKey);

        internal static Uri CreateDocumentUri(string databaseName, string collectionName, string documentId) =>
            UriFactory.CreateDocumentUri(databaseName, collectionName, documentId);

        internal static RequestOptions GetRequestOptions(string partitionKey) => new RequestOptions
        {
            PartitionKey = new PartitionKey(partitionKey)
        };
    }
}