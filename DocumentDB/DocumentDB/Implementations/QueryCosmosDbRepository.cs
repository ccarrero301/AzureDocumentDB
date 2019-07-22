using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Mappings;
using Microsoft.Azure.Documents;
using Specification.Contracts;

namespace DocumentDB.Implementations
{
    public class QueryCosmosDbRepository<TEntity> : IQueryDocumentDbRepository<TEntity>
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbAccessKey;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _databaseName;
        private readonly IMapper _mapper;

        public QueryCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName,
            string collectionName, Profile mappingProfile)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbAccessKey = cosmosDbAccessKey;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _mapper = MappingConfiguration.Configure(mappingProfile);
        }

        public async Task<TEntity> GetDocumentByIdAsync<TDocument>(string documentId, string partitionKey)
        {
            try
            {
                using (var documentClient =
                    CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
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

        public IEnumerable<TEntity> GetBySpecification<TDocument>(ISpecification<TDocument> documentSpecification,
            string partitionKey)
        {
            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var documentCollectionUri =
                    CosmosDbUtilities.CreateDocumentCollectionUri(_databaseName, _collectionName);

                var feedOptions = CosmosDbUtilities.SetFeedOptions(partitionKey);

                //using (var documentQuery = documentClient
                //    .CreateDocumentQuery<TDocument>(documentCollectionUri, feedOptions)
                //    .Where(document => documentSpecification.IsSatisfiedBy(document)).AsDocumentQuery<TDocument>())
                //{
                //    while (documentQuery.HasMoreResults)
                //    {
                //        foreach (var document in await documentQuery.ExecuteNextAsync<TDocument>()
                //            .ConfigureAwait(false))
                //        {
                //            documentList.Add(document);
                //        }
                //    }
                //}

                var documentList = documentClient.CreateDocumentQuery<TDocument>(documentCollectionUri, feedOptions)
                    .Where(documentSpecification.IsSatisfiedBy);

                return _mapper.Map<IEnumerable<TDocument>, IEnumerable<TEntity>>(documentList);
            }
        }
    }
}