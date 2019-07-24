using AutoMapper;

namespace DocumentDB.Mappings
{
    internal class MappingConfiguration
    {
        internal static IMapper Configure(Profile mappingProfile)
        {
            var mappingConfiguration = new MapperConfiguration(configuration => { configuration.AddProfile(mappingProfile); });

            var mapper = mappingConfiguration.CreateMapper();

            return mapper;
        }
    }
}