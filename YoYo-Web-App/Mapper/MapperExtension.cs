using AutoMapper;

namespace YoYo_Web_App.Mapper
{
    public static class MapperExtension
    {
        public static TDestination Map<TSource, TDestination>(this TDestination destination, TSource source, IMapper mapper) => mapper.Map(source, destination);
    }
}
