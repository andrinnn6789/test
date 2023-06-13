using System.Collections.Generic;

namespace IAG.VinX.Schüwo.SV.Dto.Interface;

public interface IOrder<out T> where T : IOrderPos
{
    public IEnumerable<T> PosData { get; }
}