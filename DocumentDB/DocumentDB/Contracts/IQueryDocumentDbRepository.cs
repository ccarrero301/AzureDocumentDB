using System.Collections.Generic;
using System.Threading.Tasks;
using Specification.Base;
using Specification.Contracts;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        Task<TEntity> GetDocumentByIdAsync(string documentId, string partitionKey);

        IEnumerable<TEntity> GetBySpecification(ISpecification<TDocument> documentSpecification, string partitionKey);

        Task<(string continuationToken, IEnumerable<TEntity>)> GetPaginatedResultsBySpecificationAsync(ExpressionSpecification<TDocument> documentSpecification, string partitionKey,
            int pageNumber = 1, int pageSize = 100, string continuationToken = null);
    }
}