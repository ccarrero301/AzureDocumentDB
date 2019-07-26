using System.Collections.Generic;
using System.Threading.Tasks;
using Specification.Base;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        Task<TEntity> GetByIdAsync(string partitionKey, string documentId);

        Task<IEnumerable<TEntity>> GetBySpecificationAsync(string partitionKey, ExpressionSpecification<TDocument> documentSpecification, int pageNumber = 1, int pageSize = 100);
    }
}