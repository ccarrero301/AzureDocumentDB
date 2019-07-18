using DocumentDB.Patterns.Specification.Contracts;
using DocumentDB.Patterns.Specification.Implementations;

namespace DocumentDB.Patterns.Specification.Base
{
    internal abstract class CompositeSpecification<TEntity> : ISpecification<TEntity>
    {
        public abstract bool IsSatisfiedBy(TEntity entityToTest);


        public ISpecification<TEntity> And(ISpecification<TEntity> specification) =>
            new AndSpecification<TEntity>(this, specification);

        public ISpecification<TEntity> Or(ISpecification<TEntity> specification) =>
            new OrSpecification<TEntity>(this, specification);

        public ISpecification<TEntity> Not() => new NotSpecification<TEntity>(this);

        public ISpecification<TEntity> All() => new AllSpecification<TEntity>();

        public static CompositeSpecification<TEntity> operator &(CompositeSpecification<TEntity> specificationLeft,
            CompositeSpecification<TEntity> specificationRight) =>
            new AndSpecification<TEntity>(specificationLeft, specificationRight);

        public static CompositeSpecification<TEntity> operator |(CompositeSpecification<TEntity> specificationLeft,
            CompositeSpecification<TEntity> specificationRight) =>
            new OrSpecification<TEntity>(specificationLeft, specificationRight);

        public static CompositeSpecification<TEntity> operator !(CompositeSpecification<TEntity> specification) =>
            new NotSpecification<TEntity>(specification);
    }
}