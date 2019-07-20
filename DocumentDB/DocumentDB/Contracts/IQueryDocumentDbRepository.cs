using System.Collections.Generic;
using System.Threading.Tasks;
using Specification.Contracts;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity>
    {
        Task<TEntity> GetDocumentByIdAsync<TDocument>(string documentId, string partitionKey);

        Task<IEnumerable<TEntity>> GetBySpecificationAsync<TDocument>(ISpecification<TDocument> documentSpecification,
            string partitionKey);
    }
}