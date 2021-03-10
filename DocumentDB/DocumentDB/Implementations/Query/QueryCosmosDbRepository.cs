using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DesignPatterns.DomainDrivenDesign.Specification.Base;
using DocumentDB.Contracts;
using DocumentDB.Implementations.Utils;
using DocumentDB.Mappings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DocumentDB.Implementations.Query
{
    public class QueryCosmosDbRepository<TEntity, TDocument> : IQueryDocumentDbRepository<TEntity, TDocument>
    {
        private CosmosDbConfiguration CosmosDbConfiguration { get; }
        private readonly IMapper _mapper;

        public QueryCosmosDbRepository(CosmosDbConfiguration cosmosDbConfiguration)
        {
            CosmosDbConfiguration = cosmosDbConfiguration;
            _mapper = MappingConfiguration.Configure(cosmosDbConfiguration.MappingProfile);
        }

        public async Task<CosmosDocumentResponse<TDocument, TEntity>> GetBySpecificationAsync(string partitionKey,
            ExpressionSpecification<TDocument> documentSpecification, int pageNumber = 1, int pageSize = 100)
        {
            var cosmosDocumentResponse = new CosmosDocumentResponse<TDocument, TEntity>();

            using var cosmosClient = new CosmosClient(CosmosDbConfiguration.Endpoint, CosmosDbConfiguration.AccessKey);
            
            var container = cosmosClient.GetContainer(CosmosDbConfiguration.DatabaseName, CosmosDbConfiguration.CollectionName);

            var queryRequestOptions = CosmosDbUtilities.SetQueryRequestOptions(partitionKey, pageSize);

            var documentQueryable = container.GetItemLinqQueryable<TDocument>(requestOptions: queryRequestOptions)
                .Where(documentSpecification.ToExpression()).Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var documentIterator = documentQueryable.ToFeedIterator();

            while (documentIterator.HasMoreResults)
            {
                var documentFeedResponse = await documentIterator.ReadNextAsync().ConfigureAwait(false);

                cosmosDocumentResponse = new CosmosDocumentResponse<TDocument, TEntity>(documentFeedResponse.StatusCode,
                    documentFeedResponse.RequestCharge, documentFeedResponse.Resource.ToList(), _mapper);
            }

            return cosmosDocumentResponse;
        }

        public async Task<CosmosDocumentResponse<TDocument, TEntity>> GetByIdAsync(string partitionKey, string documentId)
        {
            try
            {
                using var cosmosClient = new CosmosClient(CosmosDbConfiguration.Endpoint, CosmosDbConfiguration.AccessKey);
                
                var container = cosmosClient.GetContainer(CosmosDbConfiguration.DatabaseName, CosmosDbConfiguration.CollectionName);

                var documentResponse = await container.ReadItemAsync<TDocument>(partitionKey: new PartitionKey(partitionKey), id: documentId)
                    .ConfigureAwait(false);

                var documentList = new List<TDocument> {documentResponse.Resource};

                var cosmosDocumentResponse = new CosmosDocumentResponse<TDocument, TEntity>(documentResponse.StatusCode,
                    documentResponse.RequestCharge, documentList, _mapper);

                return cosmosDocumentResponse;
            }
            catch (CosmosException)
            {
                return new CosmosDocumentResponse<TDocument, TEntity>(HttpStatusCode.NotFound, 0, null, _mapper);
            }
        }
    }
}