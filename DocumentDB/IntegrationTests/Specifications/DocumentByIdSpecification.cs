using System;
using System.Linq.Expressions;
using IntegrationTests.Documents;
using Specification.Base;

namespace IntegrationTests.Specifications
{
    internal class DocumentByIdSpecification : ExpressionSpecification<Person>
    {
        private readonly string _id;

        public DocumentByIdSpecification(string id) => _id = id;

        public override Expression<Func<Person, bool>> ToExpression() => person => person.Id == _id;
    }
}