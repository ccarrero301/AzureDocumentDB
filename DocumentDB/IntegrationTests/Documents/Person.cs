using DocumentDB.Contracts;
using Newtonsoft.Json;

namespace IntegrationTests.Documents
{
    internal class Person : IEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string DocumentId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string FamilyName { get; set; }

        [JsonIgnore]
        public string PartitionKey => FamilyName;
    }
}