using System.Threading.Tasks;
using DocumentDB.Implementations.Utils;
using Specification.Base;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        Task<CosmosDocumentResponse<TDocument, TEntity>> GetByIdAsync(string partitionKey, string documentId);

        Task<CosmosDocumentResponse<TDocument, TEntity>> GetBySpecificationAsync(string partitionKey,
            ExpressionSpecification<TDocument> documentSpecification, int pageNumber = 1, int pageSize = 100);
    }
}