using Newtonsoft.Json;

namespace IAG.VinX.Smith.HelloTess.SyncLogic;

public interface IKeyable
{
    [JsonIgnore]
    string Key { get; }
}