using AutoMapper;

namespace EmployeeMicroservice.Helpers
{
    public static class MapperHelper
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            var mapper = GetMapper();
            return mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var mapper = GetMapper();
            return mapper.Map(source, destination);
        }

        private static IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });
            return configuration.CreateMapper();
        }
    }
}