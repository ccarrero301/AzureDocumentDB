using System;
using System.Linq.Expressions;
using DesignPatterns.DomainDrivenDesign.Specification.Base;
using IntegrationTests.Documents;

namespace IntegrationTests.Specifications
{
    internal class FamilyNameSpecification : ExpressionSpecification<Person>
    {
        private readonly string _familyName;

        public FamilyNameSpecification(string familyName) => _familyName = familyName;

        public override Expression<Func<Person, bool>> ToExpression() => person => person.FamilyName == _familyName;
    }
}