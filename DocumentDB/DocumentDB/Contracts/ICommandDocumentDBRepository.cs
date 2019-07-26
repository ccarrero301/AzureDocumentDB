using System.Threading.Tasks;

namespace DocumentDB.Contracts
{
    internal interface ICommandDocumentDbRepository<TEntity, in TDocument> where TDocument : IEntity
    {
        Task<TEntity> AddDocumentAsync(TDocument document, string partitionKey);

        Task<TEntity> UpdateDocumentAsync(TDocument document, string partitionKey);

        Task<TEntity> DeleteDocumentAsync(string documentId, string partitionKey);
    }
}