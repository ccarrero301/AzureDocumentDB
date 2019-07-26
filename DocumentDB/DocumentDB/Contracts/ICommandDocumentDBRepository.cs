using System.Threading.Tasks;
using DocumentDB.Implementations.Utils;

namespace DocumentDB.Contracts
{
    internal interface ICommandDocumentDbRepository<TEntity, TDocument> where TDocument : IEntity
    {
        Task<CosmosDocumentResponse<TDocument, TEntity>> AddDocumentAsync(TDocument document);

        Task<CosmosDocumentResponse<TDocument, TEntity>> UpdateDocumentAsync(TDocument document);

        Task<CosmosDocumentResponse<TDocument, TEntity>> DeleteDocumentAsync(TDocument document);
    }
}