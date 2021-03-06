using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDB.Implementations.Query;
using DocumentDB.Implementations.Utils;
using IntegrationTests.Documents;
using IntegrationTests.Mappings;
using IntegrationTests.Specifications;
using NUnit.Framework;

namespace IntegrationTests.Tests
{
    public class QueryPeopleCollectionTests
    {
        private CosmosDbConfiguration _cosmosDbConfiguration;
        private List<Person> _peopleListToTest;
        private QueryCosmosDbRepository<Entities.Person, Person> _queryCosmosDbRepository;
        private QueryCosmosDbRepository<Entities.Person, Entities.Person> _querySameEntityAndDocumentCosmosDbRepository;
        private IMapper _mapper;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _cosmosDbConfiguration = new CosmosDbConfiguration("https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "People", "PeopleCollection");

            _mapper = MappingConfiguration.Configure(new MappingProfile());

            _queryCosmosDbRepository = new QueryCosmosDbRepository<Entities.Person, Person>(_cosmosDbConfiguration, _mapper);
            
            _querySameEntityAndDocumentCosmosDbRepository = new QueryCosmosDbRepository<Entities.Person, Entities.Person>(_cosmosDbConfiguration, _mapper);
            
            _peopleListToTest = await IntegrationTestsUtils.AddDocumentListToTestAsync(_cosmosDbConfiguration, _mapper).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public Task TearDownAsync() => IntegrationTestsUtils.DeleteDocumentListToTestAsync(_cosmosDbConfiguration, _peopleListToTest, _mapper);

        [Test]
        public async Task GetNotExistentDocumentByIdAndPartitionKey()
        {
            const string documentId = "-1";
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetByIdAsync(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.NotFound);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge == 0);
            Assert.IsTrue(!cosmosDocumentResponse.Entities.Any());
        }

        [Test]
        public async Task GetDocumentByIdAndPartitionKey()
        {
            const string documentId = "1";
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetByIdAsync(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault() != null);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecification()
        {
            var carlosFirstNameSpecification = new FirstNameSpecification("Carlos");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(carlosFirstNameSpecification, partitionKey)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 1);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(familyNameSpecification, partitionKey, 1, 1)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 1);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeTwo()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(familyNameSpecification, partitionKey, 1, 2)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 2);
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.MiddleName == "Andres");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.FirstName == "Luis");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.MiddleName == "Miguel");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageOnePageSizeThree()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(familyNameSpecification, partitionKey, 1, 3)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 3);
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.MiddleName == "Andres");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.FirstName == "Luis");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(1)?.MiddleName == "Miguel");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(2)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(2)?.FirstName == "Miguel");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(2)?.MiddleName == "Antonio");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageTwoPageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(familyNameSpecification, partitionKey, 2, 1)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 1);
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FirstName == "Luis");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.MiddleName == "Miguel");
        }

        [Test]
        public async Task GetDocumentsBySpecificationPageThreePageSizeOne()
        {
            var familyNameSpecification = new FamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _queryCosmosDbRepository.GetBySpecificationAsync(familyNameSpecification, partitionKey, 3, 1)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.Count() == 1);
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.FirstName == "Miguel");
            Assert.IsTrue(cosmosDocumentResponse.Entities.ElementAt(0)?.MiddleName == "Antonio");
        }

        [Test]
        public async Task GetDocumentByIdWhenDocumentAndEntityAreOfTheSameType()
        {
            const string documentId = "1";
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _querySameEntityAndDocumentCosmosDbRepository.GetByIdAsync(documentId, partitionKey).ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault() != null);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Andres");
        }

        [Test]
        public async Task GetDocumentsBySpecificationWhenDocumentAndEntityAreOfTheSameTypePageOnePageSizeOne()
        {
            var entityFamilyNameSpecification = new EntityFamilyNameSpecification("Carrero");
            const string partitionKey = "Carrero";

            var cosmosDocumentResponse = await _querySameEntityAndDocumentCosmosDbRepository.GetBySpecificationAsync(entityFamilyNameSpecification, partitionKey, 1)
                .ConfigureAwait(false);

            Assert.IsTrue(cosmosDocumentResponse.HttpStatusCode == HttpStatusCode.OK);
            Assert.IsTrue(cosmosDocumentResponse.RequestCharge > 0);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault() != null);
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FamilyName == "Carrero");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.FirstName == "Carlos");
            Assert.IsTrue(cosmosDocumentResponse.Entities.FirstOrDefault()?.MiddleName == "Andres");
        }
    }
}