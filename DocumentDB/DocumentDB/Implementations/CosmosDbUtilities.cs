using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDB.Implementations
{
    internal static class CosmosDbUtilities
    {
        internal static Uri CreateDocumentCollectionUri(string databaseName, string collectionName) => UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

        internal static DocumentClient CreateDocumentClient(string cosmosDbEndpointUri, string cosmosDbAccessKey) => new DocumentClient(new Uri(cosmosDbEndpointUri), cosmosDbAccessKey);

        internal static Uri CreateDocumentUri(string databaseName, string collectionName, string documentId) => UriFactory.CreateDocumentUri(databaseName, collectionName, documentId);

        internal static RequestOptions SetRequestOptions(string partitionKey) =>
            new RequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            };

        internal static FeedOptions SetFeedOptions(string partitionKey, int maxItemCount = 100, bool enableCrossPartitionQuery = false, string continuationToken = null) =>
            new FeedOptions
            {
                PartitionKey = new PartitionKey(partitionKey),
                MaxItemCount = maxItemCount,
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                RequestContinuation = continuationToken
            };
    }
}