using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using FileHelpers;

using IAG.Infrastructure.Rest;

using Newtonsoft.Json;

namespace IAG.VinX.CDV.Gastivo.Common.BusinessLogic;

public static class GastivoSerializationHelper
{
    public static byte[] SerializeAsCsv(IEnumerable<object> records, Type recordType)
    {
        var engine = new FileHelperEngine(recordType)
        {
            Encoding = Encoding.UTF8
        };

        engine.HeaderText = engine.GetFileHeader();
        var data = engine.WriteString(records);
        return Encoding.UTF8.GetBytes(data);
    }

    public static byte[] SerializeAsJson(object data)
    {
        var jsonSerializerSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new JsonPropertyAnnotationContractResolver()
        };
                
        var dataAsJson = JsonConvert.SerializeObject(data, jsonSerializerSetting);
        return Encoding.Unicode.GetBytes(dataAsJson);
    }

    public static T DeserializeFromXml<T>(byte[] data)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stream = new MemoryStream(data);
        using var streamReader = new StreamReader(stream);
        return (T)serializer.Deserialize(stream);
    }
}