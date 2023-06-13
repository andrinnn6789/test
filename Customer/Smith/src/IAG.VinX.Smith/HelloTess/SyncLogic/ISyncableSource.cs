namespace IAG.VinX.Smith.HelloTess.SyncLogic;

public interface ISyncableSource : IKeyable
{
    string Id { get; set; }
        
    string Name { get; set; }
}