using System;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations;
using IntegrationTests.Entities;
using IntegrationTests.Mappings;
using NUnit.Framework;

namespace IntegrationTests
{
    public class CommandPeopleCollectionTests
    {
        private string _collectionName;
        private CommandCosmosDbRepository<Person, Documents.Person> _commandCosmosDbRepository;
        private string _cosmosDbEndpointUri;
        private string _cosmosDbPrimaryKey;
        private string _databaseName;

        private Profile _mappingProfile;
        private QueryCosmosDbRepository<Documents.Person> _queryCosmosDbRepository;

        [SetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbPrimaryKey =
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _mappingProfile = new MappingProfile();

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbPrimaryKey, _databaseName, _collectionName, _mappingProfile);

            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri,
                _cosmosDbPrimaryKey, _databaseName, _collectionName, _mappingProfile);
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

            Assert.IsTrue(personEntityAdded != null);
            Assert.IsTrue(personEntityFound != null);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.Id, documentId) == 0);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.FamilyName, "Saldarriaga") == 0);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.FirstName, "Beatriz") == 0);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.MiddleName, "Elena") == 0);

            await _commandCosmosDbRepository.DeleteDocumentAsync(documentId, personEntityAdded.FamilyName)
                .ConfigureAwait(false);
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

            Assert.IsTrue(personEntityUpdated != null);
            Assert.IsTrue(personEntityFound != null);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.FamilyName, "Johnson") == 0);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.FirstName, "Carlos") == 0);
            Assert.IsTrue(string.CompareOrdinal(personEntityFound.MiddleName, "Carrero") == 0);

            await _commandCosmosDbRepository.DeleteDocumentAsync(documentId, personEntityAdded.FamilyName)
                .ConfigureAwait(false);
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