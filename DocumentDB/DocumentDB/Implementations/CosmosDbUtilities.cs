using Microsoft.Azure.Cosmos;

namespace DocumentDB.Implementations
{
    internal static class CosmosDbUtilities
    {
        internal static QueryRequestOptions SetQueryRequestOptions(string partitionKey, int maxItemCount = 100) =>
            new QueryRequestOptions
            {
                PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey(partitionKey),
                MaxItemCount = maxItemCount
            };
    }
}