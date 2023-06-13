using System.Collections.Generic;
using System.Linq;

using IAG.VinX.Globalwine.ShopNext.BusinessLogic;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.BusinessLogic;

public class AddressTransformerTest
{

    [Fact]
    public void TransformTest()
    {
        var flatAddresses = new List<ContactWithAddressFlat>
        {
            new ()
            {
                BiId = 123
            },
            new ()
        };
        var contacts = new AddressTransformer().Transform(flatAddresses.AsQueryable());
        Assert.Equal(2, contacts.Count);
        Assert.Equal(2, contacts.First().Addresses.Count);
    }
}