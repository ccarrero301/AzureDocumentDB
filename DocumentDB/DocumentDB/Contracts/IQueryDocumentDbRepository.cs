using System.Threading.Tasks;

namespace DocumentDB.Contracts
{
    internal interface IQueryDocumentDbRepository<TEntity>
    {
        Task<TEntity> GetDocumentByIdAsync<TDocument>(string documentId, string partitionKey);
    }
}