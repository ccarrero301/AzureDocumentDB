using DocumentDB.Contracts;
using Newtonsoft.Json;

namespace IntegrationTests.Documents
{
    internal class Person : IEntity
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "middleName")]
        public string MiddleName { get; set; }

        [JsonProperty(PropertyName = "familyName")]
        public string FamilyName { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }
    }
}