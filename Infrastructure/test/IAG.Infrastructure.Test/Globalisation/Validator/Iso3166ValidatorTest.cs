using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Validator;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Validator;

public class Iso3166ValidatorTest
{
    [Fact]
    public void ValidateOkTest()
    {
        Iso3166Validator.ValidateIso2Country("CH");
        Iso3166Validator.ValidateIso2Country("ch");
        Iso3166Validator.ValidateIso2Country("");
    }

    [Fact]
    public void ValidateFailTest()
    {
        Assert.Throws<LocalizableException>(() => Iso3166Validator.ValidateIso2Country("Di"));
        Assert.Throws<LocalizableException>(() => Iso3166Validator.ValidateIso2Country("xxxx"));
    }
}