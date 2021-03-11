namespace DocumentDB.Contracts
{
    public interface IEntity
    {
        string DocumentId { get; set; }

        string PartitionKey { get; }
    }
}