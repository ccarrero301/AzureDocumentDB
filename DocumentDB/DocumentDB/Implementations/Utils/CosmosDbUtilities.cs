using Microsoft.Azure.Cosmos;

namespace DocumentDB.Implementations.Utils
{
    internal static class CosmosDbUtilities
    {
        internal static QueryRequestOptions SetQueryRequestOptions(string partitionKey = "", int maxItemCount = 100)
        {
            return !string.IsNullOrEmpty(partitionKey) ? 
                new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey), MaxItemCount = maxItemCount } : 
                new QueryRequestOptions { MaxItemCount = maxItemCount };
        }
    }
}