using Microsoft.Azure.Cosmos;

namespace DocumentDB.Implementations.Utils
{
    internal static class CosmosDbUtilities
    {
        internal static QueryRequestOptions SetQueryRequestOptions(string partitionKey, int maxItemCount = 100) =>
            new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey),
                MaxItemCount = maxItemCount
            };
    }
}