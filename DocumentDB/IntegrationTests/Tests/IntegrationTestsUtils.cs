using DocumentDB.Implementations;
using IntegrationTests.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace IntegrationTests.Tests
{
    internal static class IntegrationTestsUtils
    {
        internal static Documents.Person CreateDocument(string id, string firstName, string middleName,
            string familyName) =>
            new Documents.Person
            {
                Id = id,
                FirstName = firstName,
                MiddleName = middleName,
                FamilyName = familyName
            };

        internal static async Task<List<Documents.Person>> AddDocumentListToTestAsync(CommandCosmosDbRepository<Person, Documents.Person> commandCosmosDbRepository)
        {
            var peopleListToTest = new List<Documents.Person>
            {
                CreateDocument("1", "Carlos", "Andres", "Carrero"),
                CreateDocument("2", "Luis", "Miguel", "Carrero"),
                CreateDocument("3", "Beatriz", "Elena", "Carrero")
            };

            foreach (var personDocument in peopleListToTest)
            {
                await commandCosmosDbRepository.AddDocumentAsync(personDocument, personDocument.FamilyName)
                    .ConfigureAwait(false);
            }

            return peopleListToTest;
        }

        internal static async Task DeleteDocumentListToTestAsync(CommandCosmosDbRepository<Person, Documents.Person> commandCosmosDbRepository, List<Documents.Person> peopleListToTest)
        {
            foreach (var personDocument in peopleListToTest)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(personDocument.Id, personDocument.FamilyName)
                    .ConfigureAwait(false);
            }
        }

        internal static async Task DeleteDocumentByIdAndPartitionKeyToTestAsync(CommandCosmosDbRepository<Person, Documents.Person> commandCosmosDbRepository, List<(string, string)> documentsToDelete)
        {
            foreach (var (documentId, partitionKey) in documentsToDelete)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(documentId, partitionKey)
                    .ConfigureAwait(false);
            }

            documentsToDelete.Clear();
        }

        internal static async Task<Documents.Person> InsertDocumentAsync(string id, string firstName, string middleName, string familyName, CommandCosmosDbRepository<Person, Documents.Person> commandCosmosDbRepository, List<(string, string)> documentsToDelete, bool deleteInTearDown = true)
        {
            var personDocumentToAdd = CreateDocument(id, firstName, middleName, familyName);

            await commandCosmosDbRepository.AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            if(deleteInTearDown)
                documentsToDelete.Add((id, familyName));

            return personDocumentToAdd;
        }

        internal static Task<Documents.Person> GetDocumentByIdAndPartitionKey(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile, string id, string partitionKey)
        {
            var queryCosmosDbRepository = new QueryCosmosDbRepository<Documents.Person>(cosmosDbEndpointUri,
                cosmosDbAccessKey, databaseName, collectionName, mappingProfile);

            return queryCosmosDbRepository.GetDocumentByIdAsync<Documents.Person>(id, partitionKey);
        }
    }
}
