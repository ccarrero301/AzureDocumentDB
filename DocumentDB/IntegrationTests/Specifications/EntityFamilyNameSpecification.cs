using System;
using System.Linq.Expressions;
using DesignPatterns.DomainDrivenDesign.Specification.Base;
using IntegrationTests.Entities;

namespace IntegrationTests.Specifications
{
    internal class EntityFamilyNameSpecification : ExpressionSpecification<Person>
    {
        private readonly string _familyName;

        public EntityFamilyNameSpecification(string familyName) => _familyName = familyName;

        public override Expression<Func<Person, bool>> ToExpression() => person => person.FamilyName == _familyName;
    }
}
