using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Rest.Atlas.Dto;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Resource<T> : BaseResource
    where T : new()
{
    private List<T> _parsedData;

    public Resource()
    {
    }

    public Resource(List<T> data)
    {
        _parsedData = data;
    }

    [UsedImplicitly]
    public Meta Meta { get; set; }

    [JsonProperty("data")]
    public List<List<object>> RawData { get; set; }

    [JsonIgnore]
    public List<T> Data
    {
        get
        {
            if (_parsedData == null)
            {
                var mapper = new AtlasMapper<T>(Meta.Properties);
                _parsedData = new List<T>();

                foreach (List<object> data in RawData)
                {
                    _parsedData.Add(mapper.GetObject(data));
                }
            }

            return _parsedData;
        }
    }
}