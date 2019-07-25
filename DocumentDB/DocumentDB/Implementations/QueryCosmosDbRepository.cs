using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Mappings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Specification.Base;
using Specification.Contracts;

namespace DocumentDB.Implementations
{
    public class QueryCosmosDbRepository<TEntity, TDocument> : IQueryDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity where TEntity : IEntity
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

        public async Task<TEntity> GetDocumentByIdAsync(string documentId, string partitionKey)
        {
            using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var container = cosmosClient.GetContainer(_databaseName, _collectionName);
                var queryRequestOptions = CosmosDbUtilities.SetQueryRequestOptions(partitionKey);

                var documentQueryable = container.GetItemLinqQueryable<TDocument>();
                var iterator = documentQueryable.Where(entity => entity.Id == documentId).ToFeedIterator<TDocument>();
                
                while (iterator.HasMoreResults)
                {
                    foreach (var document in await iterator.ReadNextAsync())
                    {
                        //return document;
                        return _mapper.Map<TDocument, TEntity>(document);
                    }
                }
            }

            return default;
        }

        //public IEnumerable<TEntity> GetBySpecification(ISpecification<TDocument> documentSpecification, string partitionKey)
        //{
        //    using (var documentClient = CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
        //    {
        //        var documentCollectionUri = CosmosDbUtilities.CreateDocumentCollectionUri(_databaseName, _collectionName);

        //        var feedOptions = CosmosDbUtilities.SetFeedOptions(partitionKey);

        //        var documentList = documentClient.CreateDocumentQuery<TDocument>(documentCollectionUri, feedOptions).Where(documentSpecification.IsSatisfiedBy);

        //        return _mapper.Map<IEnumerable<TDocument>, IEnumerable<TEntity>>(documentList);
        //    }
        //}

        //public async Task<(string continuationToken, IEnumerable<TEntity>)> GetPaginatedResultsBySpecificationAsync(ExpressionSpecification<TDocument> documentSpecification, string partitionKey,
        //    int pageNumber = 1, int pageSize = 100, string continuationToken = null)
        //{
        //    var documentList = new List<TDocument>();

        //    using (var documentClient = CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
        //    {
        //        var documentCollectionUri = CosmosDbUtilities.CreateDocumentCollectionUri(_databaseName, _collectionName);

        //        var feedOptions = CosmosDbUtilities.SetFeedOptions(partitionKey, pageSize, false, continuationToken);

        //        using (var documentQuery = documentClient.CreateDocumentQuery<TDocument>(documentCollectionUri, feedOptions).Where(documentSpecification.ToExpression()).AsDocumentQuery())
        //        {
        //            var feedResponse = await documentQuery.ExecuteNextAsync<TDocument>().ConfigureAwait(false);

        //            foreach (var document in feedResponse)
        //            {
        //                documentList.Add(document);
        //            }

        //            var updatedContinuationToken = feedResponse.ResponseContinuation;

        //            return (updatedContinuationToken, _mapper.Map<IEnumerable<TDocument>, IEnumerable<TEntity>>(documentList));
        //        }
        //    }
        //}
    }
}