using IAG.Infrastructure.Globalisation.Localizer;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Localizer;

public class LocalizableParamterTest
{
    [Fact]
    public void Clone()
    {
        var localizableParameter = new LocalizableParameter("test", 
            1, 
            new LocalizableParameter("sub", 
                2,
                new LocalizableParameter("sub-sub", 
                    3)));
        var clone = localizableParameter.Clone();
        clone.ResourceId = "clone";
        (clone.Params[1] as LocalizableParameter)!.Params[1] = "hello sub-sub";
        clone.Params[1] = "hello sub";
        Assert.Equal("test", localizableParameter.ResourceId);
        var subParam = localizableParameter.Params[1] as LocalizableParameter;
        Assert.Equal("sub", subParam!.ResourceId);
        Assert.Equal("sub-sub", (subParam!.Params[1] as LocalizableParameter)!.ResourceId);
    }
}