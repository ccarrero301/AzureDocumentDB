using System.Threading.Tasks;
using DesignPatterns.DomainDrivenDesign.Specification.Base;
using DocumentDB.Implementations.Utils;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity, TDocument>
    {
        Task<CosmosDocumentResponse<TDocument, TEntity>> GetByIdAsync(string partitionKey, string documentId);

        Task<CosmosDocumentResponse<TDocument, TEntity>> GetBySpecificationAsync(string partitionKey,
            ExpressionSpecification<TDocument> documentSpecification, int pageNumber = 1, int pageSize = 100);
    }
}