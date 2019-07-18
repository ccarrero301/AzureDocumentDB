using System.Threading.Tasks;
using DocumentDB.Entities;

namespace DocumentDB.Contracts
{
    internal interface ICommandDocumentDbRepository<TEntity, in TDocument> where TDocument : Entity
    {
        Task<TEntity> AddDocumentAsync(TDocument document, string partitionKey);

        Task<TEntity> UpdateDocumentAsync(TDocument document, string partitionKey);

        Task DeleteDocumentAsync(string documentId, string partitionKey);
    }
}