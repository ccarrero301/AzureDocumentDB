namespace DocumentDB.Contracts
{
    public interface IEntity
    {
        string Id { get; set; }

        string PartitionKey { get; }
    }
}