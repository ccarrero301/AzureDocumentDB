using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Exceptions;
using DocumentDB.Mappings;
using Microsoft.Azure.Cosmos;

namespace DocumentDB.Implementations
{
    public class CommandCosmosDbRepository<TEntity, TDocument> : ICommandDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbAccessKey;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _databaseName;
        private readonly IMapper _mapper;
        private readonly Profile _mappingProfile;

        public CommandCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbAccessKey = cosmosDbAccessKey;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _mappingProfile = mappingProfile;
            _mapper = MappingConfiguration.Configure(_mappingProfile);
        }

        public async Task<TEntity> AddDocumentAsync(TDocument document)
        {
            if (await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document already exists", document);

            using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                var documentResponse = await container.CreateItemAsync(document, new PartitionKey(document.PartitionKey)).ConfigureAwait(false);

                var documentCreated = documentResponse.Resource;

                return _mapper.Map<TDocument, TEntity>(documentCreated);
            }
        }

        public async Task<TEntity> UpdateDocumentAsync(TDocument document)
        {
            if (!await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document does not exist", document);

            using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                var documentResponse = await container.ReplaceItemAsync(partitionKey: new PartitionKey(document.PartitionKey), id: document.Id, item: document).ConfigureAwait(false);

                var documentUpdated = documentResponse.Resource;

                return _mapper.Map<TDocument, TEntity>(documentUpdated);
            }
        }

        public async Task<TEntity> DeleteDocumentAsync(TDocument document)
        {
            if (!await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>($"Document with id {document.Id} does not exist");

            using (var cosmosClient = new CosmosClient(_cosmosDbEndpointUri, _cosmosDbAccessKey))
            {
                var container = cosmosClient.GetContainer(_databaseName, _collectionName);

                var documentResponse = await container.DeleteItemAsync<TDocument>(partitionKey: new PartitionKey(document.PartitionKey), id: document.Id).ConfigureAwait(false);

                var documentDeleted = documentResponse.Resource;

                return _mapper.Map<TDocument, TEntity>(documentDeleted);
            }
        }

        private async Task<bool> DocumentExistsAsync(TDocument document)
        {
            var cosmosDbQueryRepository = new QueryCosmosDbRepository<TEntity, TDocument>(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);

            var entity = await cosmosDbQueryRepository.GetByIdAsync(document.PartitionKey, document.Id).ConfigureAwait(false);

            return !EqualityComparer<TEntity>.Default.Equals(entity, default);
        }
    }
}