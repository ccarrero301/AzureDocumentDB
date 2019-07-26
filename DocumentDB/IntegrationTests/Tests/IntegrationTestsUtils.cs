using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations.Command;
using DocumentDB.Implementations.Query;
using IntegrationTests.Documents;
using IntegrationTests.Specifications;

namespace IntegrationTests.Tests
{
    internal static class IntegrationTestsUtils
    {
        internal static Person CreateDocument(string id, string firstName, string middleName, string familyName) =>
            new Person
            {
                Id = id,
                FirstName = firstName,
                MiddleName = middleName,
                FamilyName = familyName
            };

        internal static async Task<List<Person>> AddDocumentListToTestAsync(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile)
        {
            var commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Person>(cosmosDbEndpointUri, cosmosDbAccessKey, databaseName, collectionName, mappingProfile);

            var peopleListToTest = new List<Person>
            {
                CreateDocument("1", "Carlos", "Andres", "Carrero"),
                CreateDocument("2", "Luis", "Miguel", "Carrero"),
                CreateDocument("3", "Miguel", "Antonio", "Carrero"),
                CreateDocument("4", "Beatriz", "Elena", "Saldarriaga")
            };

            foreach (var personDocument in peopleListToTest)
            {
                await commandCosmosDbRepository.AddDocumentAsync(personDocument).ConfigureAwait(false);
            }

            return peopleListToTest;
        }

        internal static async Task DeleteDocumentListToTestAsync(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile,
            List<Person> peopleListToTest)
        {
            var commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Person>(cosmosDbEndpointUri, cosmosDbAccessKey, databaseName, collectionName, mappingProfile);

            foreach (var personDocument in peopleListToTest)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(personDocument).ConfigureAwait(false);
            }
        }

        internal static async Task DeleteDocumentByIdAndPartitionKeyToTestAsync(CommandCosmosDbRepository<Entities.Person, Person> commandCosmosDbRepository, List<Person> documentsToDelete)
        {
            foreach (var personDocument in documentsToDelete)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(personDocument).ConfigureAwait(false);
            }

            documentsToDelete.Clear();
        }

        internal static async Task<Person> InsertDocumentAsync(string id, string firstName, string middleName, string familyName,
            CommandCosmosDbRepository<Entities.Person, Person> commandCosmosDbRepository, List<Person> documentsToDelete, bool deleteInTearDown = true)
        {
            var personDocumentToAdd = CreateDocument(id, firstName, middleName, familyName);

            await commandCosmosDbRepository.AddDocumentAsync(personDocumentToAdd).ConfigureAwait(false);

            if (deleteInTearDown)
                documentsToDelete.Add(personDocumentToAdd);

            return personDocumentToAdd;
        }
    }
}