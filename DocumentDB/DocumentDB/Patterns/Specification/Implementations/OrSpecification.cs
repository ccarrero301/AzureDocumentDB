using DocumentDB.Patterns.Specification.Base;
using DocumentDB.Patterns.Specification.Contracts;

namespace DocumentDB.Patterns.Specification.Implementations
{
    internal sealed class OrSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        private readonly ISpecification<TEntity> _leftSpecification;
        private readonly ISpecification<TEntity> _rightSpecification;

        public OrSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
        {
            _leftSpecification = left;
            _rightSpecification = right;
        }

        public override bool IsSatisfiedBy(TEntity entityToTest) =>
            _leftSpecification.IsSatisfiedBy(entityToTest) || _rightSpecification.IsSatisfiedBy(entityToTest);
    }
}