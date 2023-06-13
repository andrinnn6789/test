using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Resource;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Globalisation.Validator;

public static class Iso3166Validator
{
    private static readonly List<IsoData> IsoDatas;

    static Iso3166Validator()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Iso3166Validator).Namespace + ".Iso3166.json");
        using var reader = new StreamReader(stream!);
        IsoDatas = JsonSerializer.Deserialize<List<IsoData>>(reader.ReadToEnd(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static void ValidateIso2Country(string isoCode)
    {
        if (string.IsNullOrWhiteSpace(isoCode))
            return;
        if (isoCode.Length != 2)
            throw new LocalizableException(ResourceIds.InvalidIsoCode, isoCode);

        isoCode = isoCode.ToUpper();
        if (IsoDatas.Any(regionInfo => regionInfo.Alpha2 == isoCode))
        {
            return;
        }

        throw new LocalizableException(ResourceIds.InvalidIsoCode, isoCode);
    }

    // ReSharper disable UnusedMember.Local
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    private class IsoData
    {
        public string Name { get; set; }

        [JsonPropertyName("alpha-2")]
        public string Alpha2 { get; [UsedImplicitly] set; }

        [JsonPropertyName("alpha-3")]
        public string Alpha3 { get; set; }

        [JsonPropertyName("country-code")]
        public string Countrycode { get; set; }

        [JsonPropertyName("iso_3166-2")]
        public string Iso31662 { get; set; }

        public string Region { get; set; }

        [JsonPropertyName("sub-region")]
        public string Subregion { get; set; }

        [JsonPropertyName("intermediate-region")]
        public string Intermediateregion { get; set; }

        [JsonPropertyName("region-code")]
        public string Regioncode { get; set; }

        [JsonPropertyName("sub-region-code")]
        public string Subregioncode { get; set; }

        [JsonPropertyName("intermediate-region-code")]
        public string Intermediateregioncode { get; set; }
    }
}