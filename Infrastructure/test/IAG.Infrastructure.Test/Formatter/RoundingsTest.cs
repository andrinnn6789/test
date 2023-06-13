using IAG.Infrastructure.Formatter;

using Xunit;

namespace IAG.Infrastructure.Test.Formatter;

public class RoundingsTest
{

    [Fact]
    public void SwissRoundings()
    {
        Assert.Equal(1.05m, Roundings.SwissCommercialRound(1.03m));
        Assert.Equal(1.05m, Roundings.SwissCommercialRound(1.07m));
        Assert.Equal(1.10m, Roundings.SwissCommercialRound(1.08m));
        Assert.Equal(1.00m, Roundings.SwissCommercialRound(1.02m));
    }

    [Fact]
    public void DecimalRoundings()
    {
        Assert.Equal(1.05m, Roundings.DecimalRounding(1.03m, 0.05m));
        Assert.Equal(1.05m, Roundings.DecimalRounding(1.07m, 0.05m));
        Assert.Equal(1.10m, Roundings.DecimalRounding(1.08m, 0.05m));
        Assert.Equal(1.00m, Roundings.DecimalRounding(1.02m, 0.05m));
        Assert.Equal(1.02m, Roundings.DecimalRounding(1.02m, 0.01m));
    }
}