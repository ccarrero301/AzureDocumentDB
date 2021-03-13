using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Contracts;
using DocumentDB.Exceptions;
using DocumentDB.Implementations.Query;
using DocumentDB.Implementations.Utils;
using Microsoft.Azure.Cosmos;

namespace DocumentDB.Implementations.Command
{
    public class CommandCosmosDbRepository<TEntity, TDocument> : ICommandDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        private CosmosDbConfiguration CosmosDbConfiguration { get; }
        private readonly IMapper _mapper;

        public CommandCosmosDbRepository(CosmosDbConfiguration cosmosDbConfiguration, IMapper mapper)
        {
            CosmosDbConfiguration = cosmosDbConfiguration;
            _mapper = mapper;
        }

        public async Task<CosmosDocumentResponse<TDocument, TEntity>> AddDocumentAsync(TDocument document)
        {
            if (await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document already exists", document);

            using var cosmosClient = new CosmosClient(CosmosDbConfiguration.Endpoint, CosmosDbConfiguration.AccessKey);
            
            var container = cosmosClient.GetContainer(CosmosDbConfiguration.DatabaseName, CosmosDbConfiguration.CollectionName);

            var documentResponse = await container.CreateItemAsync(document, new PartitionKey(document.PartitionKey)).ConfigureAwait(false);

            var documentList = new List<TDocument> {documentResponse.Resource};

            return new CosmosDocumentResponse<TDocument, TEntity>(documentResponse.StatusCode, documentResponse.RequestCharge, documentList,
                _mapper);
        }

        public async Task<CosmosDocumentResponse<TDocument, TEntity>> UpdateDocumentAsync(TDocument document)
        {
            if (!await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>("Document does not exist", document);

            using var cosmosClient = new CosmosClient(CosmosDbConfiguration.Endpoint, CosmosDbConfiguration.AccessKey);
            
            var container = cosmosClient.GetContainer(CosmosDbConfiguration.DatabaseName, CosmosDbConfiguration.CollectionName);

            var documentResponse = await container
                .ReplaceItemAsync(partitionKey: new PartitionKey(document.PartitionKey), id: document.DocumentId, item: document).ConfigureAwait(false);

            var documentList = new List<TDocument> {documentResponse.Resource};

            return new CosmosDocumentResponse<TDocument, TEntity>(documentResponse.StatusCode, documentResponse.RequestCharge, documentList,
                _mapper);
        }

        public async Task<CosmosDocumentResponse<TDocument, TEntity>> DeleteDocumentAsync(TDocument document)
        {
            if (!await DocumentExistsAsync(document).ConfigureAwait(false))
                throw new DocumentException<TDocument>($"Document with id {document.DocumentId} does not exist");

            using var cosmosClient = new CosmosClient(CosmosDbConfiguration.Endpoint, CosmosDbConfiguration.AccessKey);
            
            var container = cosmosClient.GetContainer(CosmosDbConfiguration.DatabaseName, CosmosDbConfiguration.CollectionName);

            var documentResponse = await container
                .DeleteItemAsync<TDocument>(partitionKey: new PartitionKey(document.PartitionKey), id: document.DocumentId).ConfigureAwait(false);

            var documentList = new List<TDocument> {documentResponse.Resource};

            return new CosmosDocumentResponse<TDocument, TEntity>(documentResponse.StatusCode, documentResponse.RequestCharge, documentList,
                _mapper);
        }

        private async Task<bool> DocumentExistsAsync(TDocument document)
        {
            var cosmosDbQueryRepository = new QueryCosmosDbRepository<TEntity, TDocument>(CosmosDbConfiguration, _mapper);

            var cosmosDocumentResponse = await cosmosDbQueryRepository.GetByIdAsync(document.PartitionKey, document.DocumentId).ConfigureAwait(false);

            var entity = cosmosDocumentResponse.Entities.FirstOrDefault();

            return !EqualityComparer<TEntity>.Default.Equals(entity, default);
        }
    }
}