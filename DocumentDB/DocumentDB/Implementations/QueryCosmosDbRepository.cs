using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Patterns.Specification.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

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
                    var requestOptions = CosmosDbUtilities.SetRequestOptions(partitionKey);

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

        public async Task<IEnumerable<TEntity>> GetBySpecificationAsync<TDocument>(
            ISpecification<TDocument> documentSpecification,
            string partitionKey)
        {
            var documentList = new List<TDocument>();

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbPrimaryKey))
            {
                var documentCollectionUri =
                    CosmosDbUtilities.CreateDocumentCollectionUri(_databaseName, _collectionName);

                var feedOptions = new FeedOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                };

                using (var documentQuery = documentClient
                    .CreateDocumentQuery<TDocument>(documentCollectionUri, feedOptions)
                    .Where(document => documentSpecification.IsSatisfiedBy(document)).AsDocumentQuery())
                {
                    while (documentQuery.HasMoreResults)
                    {
                        foreach (var document in await documentQuery.ExecuteNextAsync<TDocument>()
                            .ConfigureAwait(false))
                        {
                            documentList.Add(document);
                        }
                    }
                }

                return _mapper.Map<IEnumerable<TDocument>, IEnumerable<TEntity>>(documentList);
            }
        }
    }
}