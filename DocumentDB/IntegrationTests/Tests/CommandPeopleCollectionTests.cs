using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Exceptions;
using DocumentDB.Implementations.Command;
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
        private List<Documents.Person> _documentsToDelete;
        private Profile _mappingProfile;

        [OneTimeSetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbAccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";
            _documentsToDelete = new List<Documents.Person>();
            _mappingProfile = new MappingProfile();
            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri, _cosmosDbAccessKey,
                _databaseName, _collectionName, _mappingProfile);
        }

        [OneTimeTearDown]
        public Task TearDownAsync() =>
            IntegrationTestsUtils.DeleteDocumentByIdAndPartitionKeyToTestAsync(_commandCosmosDbRepository, _documentsToDelete);

        [Test]
        public async Task TryToAddExistentDocument()
        {
            var personDocumentAdded = await IntegrationTestsUtils
                .InsertDocumentAsync("1", "Beatriz1", "Elena", "Saldarriaga", _commandCosmosDbRepository, _documentsToDelete).ConfigureAwait(false);

            var documentException =
                Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.AddDocumentAsync(personDocumentAdded));

            Assert.IsTrue(documentException.Document != null);
        }

        [Test]
        public async Task AddDocument()
        {
            var personDocumentToAdd = IntegrationTestsUtils.CreateDocument("2", "Beatriz2", "Elena", "Saldarriaga");

            var cosmosDocumentResponse = await _commandCosmosDbRepository.AddDocumentAsync(personDocumentToAdd).ConfigureAwait(false);

            _documentsToDelete.Add(personDocumentToAdd);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.Created);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.Id == "2");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Saldarriaga");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Beatriz2");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Elena");
        }

        [Test]
        public void TryToUpdateNotExistentDocument()
        {
            var personDocumentToUpdate = IntegrationTestsUtils.CreateDocument("3", "Carlos1", "Carrero", "Johnson");

            var documentException =
                Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.UpdateDocumentAsync(personDocumentToUpdate));

            Assert.IsTrue(documentException.Document != null);
        }

        [Test]
        public async Task UpdateDocument()
        {
            var personDocumentAdded = await IntegrationTestsUtils
                .InsertDocumentAsync("4", "Chris", "Jerry", "Johnson", _commandCosmosDbRepository, _documentsToDelete).ConfigureAwait(false);

            var personDocumentToUpdate = IntegrationTestsUtils.CreateDocument(personDocumentAdded.Id, "Carlos2", "Carrero", "Johnson");

            var cosmosDocumentResponse = await _commandCosmosDbRepository.UpdateDocumentAsync(personDocumentToUpdate).ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Johnson");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos2");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Carrero");
        }

        [Test]
        public void TryToDeleteNotExistentDocument()
        {
            var personDocumentToDelete = IntegrationTestsUtils.CreateDocument("5", "Carlos3", "Carrero", "Johnson");

            var documentException =
                Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.DeleteDocumentAsync(personDocumentToDelete));

            Assert.IsTrue(documentException.Document == null);
        }

        [Test]
        public async Task DeleteDocument()
        {
            var personDocumentAdded = await IntegrationTestsUtils
                .InsertDocumentAsync("6", "Beatriz3", "Elena", "Johnson", _commandCosmosDbRepository, _documentsToDelete, false)
                .ConfigureAwait(false);

            var cosmosDocumentResponse = await _commandCosmosDbRepository.DeleteDocumentAsync(personDocumentAdded).ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault() == null);
        }
    }
}