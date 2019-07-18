namespace DocumentDB.Patterns.Specification.Contracts
{
    internal interface ISpecification<TEntity>
    {
        bool IsSatisfiedBy(TEntity entityToTest);

        ISpecification<TEntity> And(ISpecification<TEntity> specification);

        ISpecification<TEntity> Or(ISpecification<TEntity> specification);

        ISpecification<TEntity> Not();

        ISpecification<TEntity> All();
    }
}