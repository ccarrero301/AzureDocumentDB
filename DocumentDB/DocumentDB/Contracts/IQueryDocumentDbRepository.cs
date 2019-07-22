using System.Collections.Generic;
using System.Threading.Tasks;
using Specification.Contracts;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity>
    {
        Task<TEntity> GetDocumentByIdAsync<TDocument>(string documentId, string partitionKey);

        IEnumerable<TEntity> GetBySpecification<TDocument>(ISpecification<TDocument> documentSpecification,
            string partitionKey);
    }
}