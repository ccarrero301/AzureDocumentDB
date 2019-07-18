using DocumentDB.Patterns.Specification.Base;

namespace DocumentDB.Patterns.Specification.Implementations
{
    internal sealed class AllSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        public override bool IsSatisfiedBy(TEntity entityToTest) => true;
    }
}