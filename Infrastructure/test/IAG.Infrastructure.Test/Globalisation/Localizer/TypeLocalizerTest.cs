using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Localizer;

public class TypeLocalizerTest
{
    [Fact]
    public void TypelocalizingTest()
    {
        var localizer = new TypeLocalizer(new DbStringLocalizer(new SortedList<string, string>()));
        localizer.Localize("x",
            new List<MessageStructure>
            {
                new(MessageTypeEnum.Information, "Test 1"),
                new(
                    MessageTypeEnum.Description,
                    "Test with params",
                    new LocalizableParameter(
                        "abs",
                        1))
            }
        );
    }
}