using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Mappings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Specification.Base;

namespace DocumentDB.Implementations
{
    public class QueryCosmosDbRepository<TEntity, TDocument> : IQueryDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbAccessKey;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _databaseName;
        private readonly IMapper _mapper;

        public QueryCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbAccessKey = cosmosDbAccessKey;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _mapper = MappingConfiguration.Configure(mappingProfile);
        }

        public async Task<IEnumerable<TEntity>> GetBySpecificationAsync(string partitionKey, ExpressionSpecification<TDocument> documentSpecification, int pageNumber = 1, int pageSize = 100)
        {
            var entityList = new List<TEntity>();

            using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                var queryRequestOptions = CosmosDbUtilities.SetQueryRequestOptions(partitionKey, pageSize);

                var documentQueryable = container.GetItemLinqQueryable<TDocument>(requestOptions: queryRequestOptions).AsQueryable();

                var filteredDocumentQueryable = documentQueryable.Where(documentSpecification.ToExpression()).Skip((pageNumber - 1) * pageSize).Take(pageSize);

                var documentIterator = filteredDocumentQueryable.ToFeedIterator();

                while (documentIterator.HasMoreResults)
                {
                    foreach (var document in await documentIterator.ReadNextAsync().ConfigureAwait(false))
                    {
                        var entity = _mapper.Map<TDocument, TEntity>(document);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        public async Task<TEntity> GetByIdAsync(string partitionKey, string documentId)
        {
            TEntity entity;

            try
            {
                using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
                {
                    var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                    var entityResponse = await container.ReadItemAsync<TDocument>(partitionKey: new PartitionKey(partitionKey), id: documentId).ConfigureAwait(false);

                    entity = _mapper.Map<TDocument, TEntity>(entityResponse.Resource);
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                    return default;

                return default;
            }

            return entity;
        }
    }
}