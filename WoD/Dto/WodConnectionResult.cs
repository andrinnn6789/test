namespace IAG.Common.WoD.Dto;

public class WodConnectionResult<T>
{
    public bool Success { get; set; }

    public T Info { get; set; }
}