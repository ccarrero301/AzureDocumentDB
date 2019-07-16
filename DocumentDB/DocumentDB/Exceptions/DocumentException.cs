using System;
using DocumentDB.Entities;

namespace DocumentDB.Exceptions
{
    internal class DocumentException<TDocument> : Exception where TDocument : Entity
    {
        public DocumentException(string message) : base(message)
        {
        }

        public DocumentException(string message, TDocument document) : base(
            $"Error : {message}, Document with Id : {document.Id}") => Document = document;

        public TDocument Document { get; }
    }
}