using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Exceptions;
using DocumentDB.Mappings;

namespace DocumentDB.Implementations
{
    public class CommandCosmosDbRepository<TEntity, TDocument> : ICommandDocumentDbRepository<TEntity, TDocument>
        where TDocument : IEntity
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbAccessKey;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _databaseName;
        private readonly IMapper _mapper;
        private readonly Profile _mappingProfile;

        public CommandCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName,
            string collectionName, Profile mappingProfile)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbAccessKey = cosmosDbAccessKey;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _mappingProfile = mappingProfile;
            _mapper = MappingConfiguration.Configure(_mappingProfile);
        }

        public async Task<TEntity> AddDocumentAsync(TDocument document, string partitionKey)
        {
            if (await DocumentExistsAsync(document.Id, partitionKey).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document already exists", document);

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var documentCollectionUri =
                    CosmosDbUtilities.CreateDocumentCollectionUri(_databaseName, _collectionName);

                var documentCreated = await documentClient.CreateDocumentAsync(documentCollectionUri, document)
                    .ConfigureAwait(false);

                document.Id = documentCreated.Resource.Id;

                return _mapper.Map<TDocument, TEntity>(document);
            }
        }

        public async Task<TEntity> UpdateDocumentAsync(TDocument document, string partitionKey)
        {
            if (!await DocumentExistsAsync(document.Id, partitionKey).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document does not exist", document);

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var documentUri = CosmosDbUtilities.CreateDocumentUri(_databaseName, _collectionName, document.Id);

                var requestOptions = CosmosDbUtilities.SetRequestOptions(partitionKey);

                var documentUpdated =
                    await documentClient.ReplaceDocumentAsync(documentUri, document, requestOptions)
                        .ConfigureAwait(false);

                document.Id = documentUpdated.Resource.Id;

                return _mapper.Map<TDocument, TEntity>(document);
            }
        }

        public async Task DeleteDocumentAsync(string documentId, string partitionKey)
        {
            if (!await DocumentExistsAsync(documentId, partitionKey).ConfigureAwait(false))
                throw new DocumentException<TDocument>($"Document with id {documentId} does not exist");

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var documentUri = CosmosDbUtilities.CreateDocumentUri(_databaseName, _collectionName, documentId);

                var requestOptions = CosmosDbUtilities.SetRequestOptions(partitionKey);

                await documentClient.DeleteDocumentAsync(documentUri, requestOptions).ConfigureAwait(false);
            }
        }

        private async Task<bool> DocumentExistsAsync(string documentId, string partitionKey)
        {
            var cosmosDbQueryRepository = new QueryCosmosDbRepository<TEntity>(_cosmosDbEndpointUri,
                _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);

            var entity = await cosmosDbQueryRepository.GetDocumentByIdAsync<TDocument>(documentId, partitionKey)
                .ConfigureAwait(false);

            return !EqualityComparer<TEntity>.Default.Equals(entity, default);
        }
    }
}