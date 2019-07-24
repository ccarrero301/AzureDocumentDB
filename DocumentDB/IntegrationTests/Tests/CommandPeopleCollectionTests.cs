using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Exceptions;
using DocumentDB.Implementations;
using IntegrationTests.Entities;
using IntegrationTests.Mappings;
using NUnit.Framework;

namespace IntegrationTests.Tests
{
    public class CommandPeopleCollectionTests
    {
        private string _collectionName;
        private CommandCosmosDbRepository<Person, Documents.Person> _commandCosmosDbRepository;
        private string _cosmosDbAccessKey;
        private string _cosmosDbEndpointUri;
        private string _databaseName;

        private List<(string, string)> _documentsToDelete;
        private Profile _mappingProfile;
        private QueryCosmosDbRepository<Documents.Person> _queryCosmosDbRepository;

        [OneTimeSetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbAccessKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _documentsToDelete = new List<(string, string)>();

            _mappingProfile = new MappingProfile();

            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            foreach (var (documentId, partitionKey) in _documentsToDelete)
            {
                await _commandCosmosDbRepository.DeleteDocumentAsync(documentId, partitionKey)
                    .ConfigureAwait(false);
            }
        }

        [Test]
        public async Task TryToAddExistentDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToAdd = new Documents.Person
            {
                Id = documentId,
                FirstName = "Beatriz",
                MiddleName = "Elena",
                FamilyName = "Saldarriaga"
            };

            var personEntityAdded = await _commandCosmosDbRepository
                .AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            _documentsToDelete.Add((documentId, personEntityAdded.FamilyName));

            var documentException = Assert.ThrowsAsync<DocumentException<Documents.Person>>(() =>
                _commandCosmosDbRepository.AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName));

            Assert.IsTrue(documentException.Document != null);
        }

        [Test]
        public async Task AddDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToAdd = new Documents.Person
            {
                Id = documentId,
                FirstName = "Beatriz",
                MiddleName = "Elena",
                FamilyName = "Saldarriaga"
            };

            var personEntityAdded = await _commandCosmosDbRepository
                .AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            var personEntityFound = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Documents.Person>(documentId, personEntityAdded.FamilyName).ConfigureAwait(false);

            _documentsToDelete.Add((documentId, personEntityAdded.FamilyName));

            Assert.IsTrue(personEntityAdded != null);
            Assert.IsTrue(personEntityFound != null);
            Assert.IsTrue(personEntityFound.Id == documentId);
            Assert.IsTrue(personEntityFound.FamilyName == "Saldarriaga");
            Assert.IsTrue(personEntityFound.FirstName == "Beatriz");
            Assert.IsTrue(personEntityFound.MiddleName == "Elena");
        }

        [Test]
        public void TryToUpdateNotExistentDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToUpdate = new Documents.Person
            {
                Id = documentId,
                FirstName = "Carlos",
                MiddleName = "Carrero",
                FamilyName = "Johnson"
            };

            var documentException = Assert.ThrowsAsync<DocumentException<Documents.Person>>(() =>
                _commandCosmosDbRepository.UpdateDocumentAsync(personDocumentToUpdate,
                    personDocumentToUpdate.FamilyName));

            Assert.IsTrue(documentException.Document != null);
        }

        [Test]
        public async Task UpdateDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToAdd = new Documents.Person
            {
                Id = documentId,
                FirstName = "Chris",
                MiddleName = "Jerry",
                FamilyName = "Johnson"
            };

            var personEntityAdded = await _commandCosmosDbRepository
                .AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            var personDocumentToUpdate = new Documents.Person
            {
                Id = documentId,
                FirstName = "Carlos",
                MiddleName = "Carrero",
                FamilyName = "Johnson"
            };

            var personEntityUpdated = await _commandCosmosDbRepository
                .UpdateDocumentAsync(personDocumentToUpdate, personEntityAdded.FamilyName).ConfigureAwait(false);

            var personEntityFound = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Documents.Person>(personEntityUpdated.Id, personEntityUpdated.FamilyName)
                .ConfigureAwait(false);

            _documentsToDelete.Add((documentId, personEntityAdded.FamilyName));

            Assert.IsTrue(personEntityUpdated != null);
            Assert.IsTrue(personEntityFound != null);
            Assert.IsTrue(personEntityFound.FamilyName == "Johnson");
            Assert.IsTrue(personEntityFound.FirstName == "Carlos");
            Assert.IsTrue(personEntityFound.MiddleName == "Carrero");
        }

        [Test]
        public void TryToDeleteNotExistentDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToDelete = new Documents.Person
            {
                Id = documentId,
                FirstName = "Carlos",
                MiddleName = "Carrero",
                FamilyName = "Johnson"
            };

            var documentException = Assert.ThrowsAsync<DocumentException<Documents.Person>>(() =>
                _commandCosmosDbRepository.DeleteDocumentAsync(documentId, personDocumentToDelete.FamilyName));

            Assert.IsTrue(documentException.Document == null);
        }

        [Test]
        public async Task DeleteDocument()
        {
            var documentId = Guid.NewGuid().ToString();

            var personDocumentToAdd = new Documents.Person
            {
                Id = documentId,
                FirstName = "Beatriz",
                MiddleName = "Elena",
                FamilyName = "Saldarriaga"
            };

            var personEntityAdded = await _commandCosmosDbRepository
                .AddDocumentAsync(personDocumentToAdd, personDocumentToAdd.FamilyName).ConfigureAwait(false);

            await _commandCosmosDbRepository.DeleteDocumentAsync(documentId, personEntityAdded.FamilyName)
                .ConfigureAwait(false);

            var personEntityDeleted = await _queryCosmosDbRepository
                .GetDocumentByIdAsync<Documents.Person>(documentId, personEntityAdded.FamilyName).ConfigureAwait(false);

            Assert.IsTrue(personEntityDeleted == null);
        }
    }
}