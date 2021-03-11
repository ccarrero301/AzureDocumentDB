using AutoMapper;
using IntegrationTests.Documents;

namespace IntegrationTests.Mappings
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, Entities.Person>()
                .ForMember(
                    destination => destination.Id, 
                    configuration => configuration.MapFrom(source => source.DocumentId));
        }
    }
}