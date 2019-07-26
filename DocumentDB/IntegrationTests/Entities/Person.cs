using DocumentDB.Contracts;

namespace IntegrationTests.Entities
{
    internal class Person : IEntity
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string FamilyName { get; set; }
        public string Id { get; set; }
    }
}