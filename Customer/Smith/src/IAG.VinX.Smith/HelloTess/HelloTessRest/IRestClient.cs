using System.Collections.Generic;
using System.Threading.Tasks;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest;

public interface IRestClient<T>
{
    Task<IEnumerable<T>> Get();

    Task Post(T body);
}