using System.Collections.Generic;
using System.Net;
using AutoMapper;

namespace DocumentDB.Implementations.Utils
{
    public class CosmosDocumentResponse<TDocument, TEntity>
    {
        internal CosmosDocumentResponse()
        {
            HttpStatusCode = HttpStatusCode.NotAcceptable;
            RequestCharge = 0;
            Documents = null;
            Entities = null;
        }

        internal CosmosDocumentResponse(HttpStatusCode httpStatusCode, double requestCharge, IEnumerable<TDocument> documents, IMapper mapper)
        {
            HttpStatusCode = httpStatusCode;
            RequestCharge = requestCharge;
            Documents = documents;
            Entities = mapper.Map<IEnumerable<TDocument>, IEnumerable<TEntity>>(Documents);
        }

        public HttpStatusCode HttpStatusCode { get; }

        public double RequestCharge { get; }

        internal IEnumerable<TDocument> Documents { get; }

        public IEnumerable<TEntity> Entities { get; }
    }
}