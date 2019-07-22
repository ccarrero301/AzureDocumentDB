using DocumentDB.Entities;
using Newtonsoft.Json;

namespace IntegrationTests.Documents
{
    internal class Person : Entity
    {
        [JsonProperty(PropertyName = "id")]
        public override string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "middleName")]
        public string MiddleName { get; set; }

        [JsonProperty(PropertyName = "familyName")]
        public string FamilyName { get; set; }
    }
}