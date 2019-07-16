using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Utilities;
using Microsoft.Azure.Documents;

namespace DocumentDB.Implementations
{
    internal class QueryCosmosDbRepository<TEntity> : IQueryDocumentDbRepository<TEntity>
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _cosmosDbPrimaryKey;
        private readonly string _databaseName;
        private readonly IMapper _mapper;

        public QueryCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbPrimaryKey, string databaseName,
            string collectionName, IMapper mapper)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbPrimaryKey = cosmosDbPrimaryKey;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _mapper = mapper;
        }

        public async Task<TEntity> GetDocumentByIdAsync<TDocument>(string documentId, string partitionKey)
        {
            try
            {
                using (var documentClient =
                    CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbPrimaryKey))
                {
                    var documentUri = CosmosDbUtilities.CreateDocumentUri(_databaseName, _collectionName, documentId);
                    var requestOptions = CosmosDbUtilities.GetRequestOptions(partitionKey);

                    var document = await documentClient.ReadDocumentAsync<TDocument>(documentUri, requestOptions)
                        .ConfigureAwait(false);

                    return _mapper.Map<TDocument, TEntity>(document.Document);
                }
            }
            catch (DocumentClientException)
            {
                return default;
            }
        }
    }
}