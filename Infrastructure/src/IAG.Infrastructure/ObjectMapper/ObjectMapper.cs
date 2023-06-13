namespace IAG.Infrastructure.ObjectMapper;

public abstract class ObjectMapper<TSource, TDestination> : IObjectMapper<TSource, TDestination>
    where TDestination : class, new() 
{
    public TDestination NewDestination(TSource source)
    {
        var newTo = new TDestination();
        return MapToDestination(source, newTo);
    }

    public TDestination UpdateDestination(TDestination destination, TSource source)
    {
        return MapToDestination(source, destination);
    }

    protected abstract TDestination MapToDestination(TSource source, TDestination destination);
}