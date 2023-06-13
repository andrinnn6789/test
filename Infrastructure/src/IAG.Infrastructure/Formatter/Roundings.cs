using System;

namespace IAG.Infrastructure.Formatter;

public static class Roundings
{
    /// <summary>
    /// Rounds a decimal using the Swiss Commercial Rounding System
    /// 0.92 will be rounded to 0.90
    /// 0.93 will be rounded to 0.95
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <param name="decimals">number of decimals, default = 2</param>
    /// <returns> 0 or rounded Decimal</returns>
    public static decimal SwissCommercialRound(decimal? value, int decimals = 2)
    {
        var factor = (decimal)Math.Pow(10, decimals - 1) * 2;
        return value.HasValue 
            ? Math.Round((decimal) (value * factor), MidpointRounding.AwayFromZero) / factor 
            : 0.00m;
    }

    /// <summary>
    /// Rounding with rounding-template
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <param name="roundingTemplate">Template to apply, e.g. 0.05, 0.01</param>
    /// <returns></returns>
    public static decimal DecimalRounding(decimal? value, decimal roundingTemplate)
    {
        return value.HasValue && roundingTemplate != 0
            ? Math.Round((decimal) (value /roundingTemplate), MidpointRounding.AwayFromZero) * roundingTemplate 
            : 0.00m;
    }
}