using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Exceptions;
using DocumentDB.Implementations;
using IntegrationTests.Entities;
using IntegrationTests.Mappings;
using Microsoft.Azure.Cosmos;
using NUnit.Framework;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;

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

        [OneTimeSetUp]
        public void Setup()
        {
            _cosmosDbEndpointUri = "https://localhost:8081";
            _cosmosDbAccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _databaseName = "People";
            _collectionName = "PeopleCollection";

            _documentsToDelete = new List<(string, string)>();

            _mappingProfile = new MappingProfile();

            _commandCosmosDbRepository = new CommandCosmosDbRepository<Person, Documents.Person>(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile);
        }

        [OneTimeTearDown]
        public Task TearDownAsync() => IntegrationTestsUtils.DeleteDocumentByIdAndPartitionKeyToTestAsync(_commandCosmosDbRepository, _documentsToDelete);

        [Test]
        public async Task TryToAddExistentDocument()
        {
            var personDocumentAdded = await IntegrationTestsUtils.InsertDocumentAsync("1", "Beatriz1", "Elena", "Saldarriaga", _commandCosmosDbRepository, _documentsToDelete).ConfigureAwait(false);

            var documentException = Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.AddDocumentAsync(personDocumentAdded, personDocumentAdded.FamilyName));

            Assert.IsTrue(documentException.Document != null);
        }

        [Test]
        public async Task AddDocument()
        {
            var personDocumentAdded = await IntegrationTestsUtils.InsertDocumentAsync("2", "Beatriz2", "Elena", "Saldarriaga", _commandCosmosDbRepository, _documentsToDelete).ConfigureAwait(false);

            var personEntityFound = await IntegrationTestsUtils
                .GetDocumentByIdAndPartitionKey(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile, personDocumentAdded.Id, personDocumentAdded.FamilyName)
                .ConfigureAwait(false);

            Assert.IsTrue(personEntityFound != null);
            Assert.IsTrue(personEntityFound.Id == personDocumentAdded.Id);
            Assert.IsTrue(personEntityFound.FamilyName == "Saldarriaga");
            Assert.IsTrue(personEntityFound.FirstName == "Beatriz2");
            Assert.IsTrue(personEntityFound.MiddleName == "Elena");
        }

        //[Test]
        //public void TryToUpdateNotExistentDocument()
        //{
        //    var personDocumentToUpdate = IntegrationTestsUtils.CreateDocument("3", "Carlos1", "Carrero", "Johnson");

        //    var documentException =
        //        Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.UpdateDocumentAsync(personDocumentToUpdate, personDocumentToUpdate.FamilyName));

        //    Assert.IsTrue(documentException.Document != null);
        //}

        //[Test]
        //public async Task UpdateDocument()
        //{
        //    var personDocumentAdded = await IntegrationTestsUtils.InsertDocumentAsync("4", "Chris", "Jerry", "Johnson", _commandCosmosDbRepository, _documentsToDelete).ConfigureAwait(false);

        //    var personDocumentToUpdate = IntegrationTestsUtils.CreateDocument(personDocumentAdded.Id, "Carlos2", "Carrero", "Johnson");

        //    var personEntityUpdated = await _commandCosmosDbRepository.UpdateDocumentAsync(personDocumentToUpdate, personDocumentAdded.FamilyName).ConfigureAwait(false);

        //    var personEntityFound = await IntegrationTestsUtils
        //        .GetDocumentByIdAndPartitionKey(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile, personEntityUpdated.Id, personEntityUpdated.FamilyName)
        //        .ConfigureAwait(false);

        //    Assert.IsTrue(personEntityUpdated != null);
        //    Assert.IsTrue(personEntityFound != null);
        //    Assert.IsTrue(personEntityFound.FamilyName == "Johnson");
        //    Assert.IsTrue(personEntityFound.FirstName == "Carlos2");
        //    Assert.IsTrue(personEntityFound.MiddleName == "Carrero");
        //}

        //[Test]
        //public void TryToDeleteNotExistentDocument()
        //{
        //    var personDocumentToDelete = IntegrationTestsUtils.CreateDocument("5", "Carlos3", "Carrero", "Johnson");

        //    var documentException =
        //        Assert.ThrowsAsync<DocumentException<Documents.Person>>(() => _commandCosmosDbRepository.DeleteDocumentAsync(personDocumentToDelete.Id, personDocumentToDelete.FamilyName));

        //    Assert.IsTrue(documentException.Document == null);
        //}

        //[Test]
        //public async Task DeleteDocument()
        //{
        //    var personDocumentAdded = await IntegrationTestsUtils.InsertDocumentAsync("6", "Beatriz3", "Elena", "Johnson", _commandCosmosDbRepository, _documentsToDelete, false).ConfigureAwait(false);

        //    await _commandCosmosDbRepository.DeleteDocumentAsync(personDocumentAdded.Id, personDocumentAdded.FamilyName).ConfigureAwait(false);

        //    var personEntityDeleted = await IntegrationTestsUtils
        //        .GetDocumentByIdAndPartitionKey(_cosmosDbEndpointUri, _cosmosDbAccessKey, _databaseName, _collectionName, _mappingProfile, personDocumentAdded.Id, personDocumentAdded.FamilyName)
        //        .ConfigureAwait(false);

        //    Assert.IsTrue(personEntityDeleted == null);
        //}

    }
}