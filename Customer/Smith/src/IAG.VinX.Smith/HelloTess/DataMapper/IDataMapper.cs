namespace IAG.VinX.Smith.HelloTess.DataMapper;

public interface IDataMapper<TSource, TTarget>
{
    bool CheckUpdate(TSource source, TTarget target);

    TTarget CreateTarget(TSource source);

    bool CheckDelete(TTarget target);
}