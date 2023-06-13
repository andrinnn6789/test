namespace IAG.Infrastructure.ObjectMapper;

public interface IObjectMapper<in TSource, TDestination>
    where TDestination : class, new()
{
    TDestination NewDestination(TSource source);

    TDestination UpdateDestination(TDestination destination, TSource source);
}