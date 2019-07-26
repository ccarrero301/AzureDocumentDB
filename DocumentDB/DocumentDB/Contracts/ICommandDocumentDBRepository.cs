using System.Threading.Tasks;

namespace DocumentDB.Contracts
{
    internal interface ICommandDocumentDbRepository<TEntity, in TDocument> where TDocument : IEntity
    {
        Task<TEntity> AddDocumentAsync(TDocument document);

        Task<TEntity> UpdateDocumentAsync(TDocument document);

        Task<TEntity> DeleteDocumentAsync(TDocument document);
    }
}