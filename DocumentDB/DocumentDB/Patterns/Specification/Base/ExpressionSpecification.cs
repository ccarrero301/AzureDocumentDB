using System;
using System.Linq.Expressions;

namespace DocumentDB.Patterns.Specification.Base
{
    internal abstract class ExpressionSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        public abstract Expression<Func<TEntity, bool>> ToExpression();

        public override bool IsSatisfiedBy(TEntity entityToTest)
        {
            var predicate = ToExpression().Compile();
            return predicate(entityToTest);
        }
    }
}