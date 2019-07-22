using System;
using System.Linq.Expressions;
using IntegrationTests.Documents;
using Specification.Base;

namespace IntegrationTests.Specifications
{
    internal class FirstNameSpecification : ExpressionSpecification<Person>
    {
        private readonly string _firstName;

        public FirstNameSpecification(string firstName) => _firstName = firstName;

        public override Expression<Func<Person, bool>> ToExpression() => person => person.FirstName == _firstName;
    }
}