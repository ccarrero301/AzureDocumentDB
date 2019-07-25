﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Documents;

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
                await commandCosmosDbRepository.AddDocumentAsync(personDocument, personDocument.FamilyName).ConfigureAwait(false);
            }

            return peopleListToTest;
        }

        internal static async Task DeleteDocumentListToTestAsync(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile,
            List<Person> peopleListToTest)
        {
            var commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Person>(cosmosDbEndpointUri, cosmosDbAccessKey, databaseName, collectionName, mappingProfile);

            foreach (var personDocument in peopleListToTest)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(personDocument.Id, personDocument.FamilyName).ConfigureAwait(false);
            }
        }

        internal static async Task DeleteDocumentByIdAndPartitionKeyToTestAsync(CommandCosmosDbRepository<Entities.Person, Person> commandCosmosDbRepository, List<(string, string)> documentsToDelete)
        {
            foreach (var (documentId, partitionKey) in documentsToDelete)
            {
                await commandCosmosDbRepository.DeleteDocumentAsync(documentId, partitionKey).ConfigureAwait(false);
            }

            documentsToDelete.Clear();
        }

        internal static async Task<Person> InsertDocumentAsync(string id, string firstName, string middleName, string familyName,
            CommandCosmosDbRepository<Entities.Person, Person> commandCosmosDbRepository, List<(string, string)> documentsToDelete, bool deleteInTearDown = true)
        {
            var personDocumentToAdd = CreateDocument(id, firstName, middleName, familyName);

            await commandCosmosDbRepository.AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            if (deleteInTearDown)
                documentsToDelete.Add((id, familyName));

            return personDocumentToAdd;
        }

        internal static Task<Entities.Person> GetDocumentByIdAndPartitionKey(string cosmosDbEndpointUri, string cosmosDbAccessKey, string databaseName, string collectionName, Profile mappingProfile,
            string id, string partitionKey)
        {
            var queryCosmosDbRepository = new QueryCosmosDbRepository<Entities.Person, Person>(cosmosDbEndpointUri, cosmosDbAccessKey, databaseName, collectionName, mappingProfile);

            return queryCosmosDbRepository.GetDocumentByIdAsync(id, partitionKey);
        }
    }
}