using System;
using DocumentDB.Contracts;

namespace DocumentDB.Exceptions
{
    public class DocumentException<TDocument> : Exception where TDocument : IEntity
    {
        public DocumentException(string message) : base(message) { }

        public DocumentException(string message, TDocument document) : base($"Error : {message}, Document with Id : {document.Id}") =>
            Document = document;

        public TDocument Document { get; }
    }
}