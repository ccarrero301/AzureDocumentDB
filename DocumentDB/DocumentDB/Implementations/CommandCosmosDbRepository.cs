using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Entities;
using DocumentDB.Exceptions;

namespace DocumentDB.Implementations
{
    internal class CommandCosmosDbRepository<TEntity, TDocument> : ICommandDocumentDbRepository<TEntity, TDocument>
        where TDocument : Entity
    {
        private readonly string _collectionName;
        private readonly string _cosmosDbEndpointUri;
        private readonly string _cosmosDbPrimaryKey;
        private readonly string _databaseName;
        private readonly IMapper _mapper;

        public CommandCosmosDbRepository(string cosmosDbEndpointUri, string cosmosDbPrimaryKey, string databaseName,
            string collectionName, IMapper mapper)
        {
            _cosmosDbEndpointUri = cosmosDbEndpointUri;
            _cosmosDbPrimaryKey = cosmosDbPrimaryKey;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _mapper = mapper;
        }

        public async Task<TEntity> AddDocumentAsync(TDocument document, string partitionKey)
        {
            if (await DocumentExistsAsync(document.Id, partitionKey).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document already exists", document);

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbPrimaryKey))
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
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbPrimaryKey))
            {
                var documentUri = CosmosDbUtilities.CreateDocumentUri(_databaseName, _collectionName, document.Id);

                var documentUpdated = await documentClient.ReplaceDocumentAsync(documentUri, document).ConfigureAwait(false);

                document.Id = documentUpdated.Resource.Id;

                return _mapper.Map<TDocument, TEntity>(document);
            }
        }

        public async Task DeleteDocumentAsync(string documentId, string partitionKey)
        {
            if (!await DocumentExistsAsync(documentId, partitionKey).ConfigureAwait(false))
                throw new DocumentException<TDocument>($"Document with id {documentId} does not exist");

            using (var documentClient =
                CosmosDbUtilities.CreateDocumentClient(_cosmosDbEndpointUri, _cosmosDbPrimaryKey))
            {
                var documentUri = CosmosDbUtilities.CreateDocumentUri(_databaseName, _collectionName, documentId);

                await documentClient.DeleteDocumentAsync(documentUri).ConfigureAwait(false);
            }
        }

        private async Task<bool> DocumentExistsAsync(string documentId, string partitionKey)
        {
            var cosmosDbQueryRepository = new QueryCosmosDbRepository<TEntity>(_cosmosDbEndpointUri,
                _cosmosDbPrimaryKey, _databaseName, _collectionName, _mapper);

            var entity = await cosmosDbQueryRepository.GetDocumentByIdAsync<TDocument>(documentId, partitionKey)
                .ConfigureAwait(false);

            return !EqualityComparer<TEntity>.Default.Equals(entity, default);
        }
    }
}