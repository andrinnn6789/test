using System;
using System.Security.Claims;
using System.Text.Json;

using IAG.IdentityServer.Authentication;

using Xunit;

namespace IAG.IdentityServer.Test.Authentication;

public class ClaimJsonConverterTest
{
    [Fact]
    public void DeserializeTest()
    {
        var claim = new Claim("stringSimple", "TestString1");
        var claimWithValueType = new Claim("stringWithValueType", "TestString2", "ValueType2");
        var claimWithIssuer = new Claim("stringWithIssuer", "TestString3", "ValueType3", "Issuer");

        var claimJson = JsonSerializer.Serialize(claim);
        var claimWithValueTypeJson = JsonSerializer.Serialize(claimWithValueType);
        var claimWithIssuerJson = JsonSerializer.Serialize(claimWithIssuer);

        var options = new JsonSerializerOptions()
        {
            Converters = {new ClaimJsonConverter()}
        };
        var claimDeserialized = JsonSerializer.Deserialize<Claim>(claimJson, options);
        var claimWithValueTypeDeserialized = JsonSerializer.Deserialize<Claim>(claimWithValueTypeJson, options);
        var claimWithIssuerDeserialized = JsonSerializer.Deserialize<Claim>(claimWithIssuerJson, options);

        Assert.NotNull(claimDeserialized);
        Assert.Equal(claim.Type, claimDeserialized.Type);
        Assert.Equal(claim.Value, claimDeserialized.Value);
        Assert.Equal(claim.ValueType, claimDeserialized.ValueType);
        Assert.Equal(claim.Issuer, claimDeserialized.Issuer);
        Assert.Equal(claim.OriginalIssuer, claimDeserialized.OriginalIssuer);
        Assert.Equal(claim.Subject, claimDeserialized.Subject);
        Assert.Equal(claim.Properties, claimDeserialized.Properties);
        Assert.NotNull(claimWithValueTypeDeserialized);
        Assert.Equal(claimWithValueType.Type, claimWithValueTypeDeserialized.Type);
        Assert.Equal(claimWithValueType.Value, claimWithValueTypeDeserialized.Value);
        Assert.Equal(claimWithValueType.ValueType, claimWithValueTypeDeserialized.ValueType);
        Assert.Equal(claimWithValueType.Issuer, claimWithValueTypeDeserialized.Issuer);
        Assert.Equal(claimWithValueType.OriginalIssuer, claimWithValueTypeDeserialized.OriginalIssuer);
        Assert.Equal(claimWithValueType.Subject, claimWithValueTypeDeserialized.Subject);
        Assert.Equal(claimWithValueType.Properties, claimWithValueTypeDeserialized.Properties);
        Assert.NotNull(claimWithIssuerDeserialized);
        Assert.Equal(claimWithIssuer.Type, claimWithIssuerDeserialized.Type);
        Assert.Equal(claimWithIssuer.Value, claimWithIssuerDeserialized.Value);
        Assert.Equal(claimWithIssuer.ValueType, claimWithIssuerDeserialized.ValueType);
        Assert.Equal(claimWithIssuer.Issuer, claimWithIssuerDeserialized.Issuer);
        Assert.Equal(claimWithIssuer.OriginalIssuer, claimWithIssuerDeserialized.OriginalIssuer);
        Assert.Equal(claimWithIssuer.Subject, claimWithIssuerDeserialized.Subject);
        Assert.Equal(claimWithIssuer.Properties, claimWithIssuerDeserialized.Properties);
        Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Claim>("{}", options));
    }
}