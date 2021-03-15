using System.Threading.Tasks;
using DesignPatterns.DomainDrivenDesign.Specification.Base;
using DocumentDB.Implementations.Utils;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity, TDocument>
    {
        Task<CosmosDocumentResponse<TDocument, TEntity>> GetByIdAsync(string documentId, string partitionKey = "");

        Task<CosmosDocumentResponse<TDocument, TEntity>> GetBySpecificationAsync(ExpressionSpecification<TDocument> documentSpecification,
            string partitionKey = "", int pageNumber = 1, int pageSize = 100);
    }
}