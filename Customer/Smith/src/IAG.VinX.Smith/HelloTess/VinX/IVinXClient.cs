using System.Collections.Generic;

namespace IAG.VinX.Smith.HelloTess.VinX;

public interface IVinXClient<T>
{
    IEnumerable<T> Get();
}