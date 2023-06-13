using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IAG.IdentityServer.Authentication;

/// <summary>
/// JSON converter for Claims. This is necessary because the class <code>Claim</code> has no default constructor
/// without no arguments.
/// This converter just supports deserialization. For serialization no special converter is necessary.
/// </summary>
public class ClaimJsonConverter : JsonConverter<Claim>
{
    private class ClaimLight
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string Issuer { get; set; }
    }

    public override Claim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var claim = JsonSerializer.Deserialize<ClaimLight>(ref reader, options);
        if (claim?.Type == null || claim.Value == null)
        {
            throw new InvalidOperationException("Failed to deserialize Claim");
        }

        return new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer);
    }

    [ExcludeFromCodeCoverage]
    public override void Write(Utf8JsonWriter writer, Claim value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}