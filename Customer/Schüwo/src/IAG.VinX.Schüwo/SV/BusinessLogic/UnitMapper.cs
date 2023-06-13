using System;
using System.Globalization;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.Schüwo.Resource;
using IAG.VinX.Schüwo.SV.Dto;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class UnitMapper
{
    private readonly IMessageLogger _msgLogger;
    private readonly ResultCounts _resultCounts;

    public UnitMapper(IMessageLogger msgLogger, ResultCounts resultCounts)
    {
        _msgLogger = msgLogger;
        _resultCounts = resultCounts;
    }

    public string GetUnit2(ArticleSw articleSw)
    {
        var unit2 = MapUnit(articleSw.BulkPackagingTextShort);

        if (articleSw.FillingTextShort == null)
        {
            return unit2;
        }

        if (articleSw.IsTank)
            return $"{unit2}>{articleSw.UnitsPerBulkPackaging} L";

        // unit 1 and 2 must be different!
        var unit1 = MapUnit(articleSw.FillingTextShort);
        if (unit1 == unit2)
            unit2 = unit1 == "KST" ? "CT": "KST";
        return $"{unit2}>{articleSw.UnitsPerBulkPackaging} {GetUnit1(articleSw)}";
    }

    public string GetUnit1(ArticleSw articleSw)
    {
        var unit = MapUnit(articleSw.FillingTextShort);
        if (articleSw.FillingInCl == null)
            return unit;

        var fillingInLiters = ((decimal)(articleSw.FillingInCl / 100)).ToString("G20", CultureInfo.InvariantCulture);
        unit += $">{fillingInLiters} L";
        return unit;
    }

    public string MapUnit(string vxUnit)
    {
        var svUnit = vxUnit switch
        {
            null => "STK",
            "Glas" => "STK",
            "HK" => "STK",
            "Gal." => "FS",
            "Dose" => "DS",
            "Pers." => "STK",
            "KEG" => "FS",
            "GK" => "STK",
            "Box" => "KST",
            "Rol." => "STK",
            "Har." => "HAR",
            "Vini" => "HAR",
            "6-P." => "CT",
            "Pack" => "KST",
            "10-P." => "CT",
            "Pal." => "PAL",
            "Sta." => "STK",
            "Fla." => "FLA",
            "Kart" => "CT",
            "Fass" => "FS",
            "Tank" => "FS",
            "Set" => "STK",
            "K" => "CT",
            "Bid." => "FLA",
            "Krug" => "STK",
            "Stk." => "STK",
            "Kist" => "KST",
            _ => ((Func<string>) (() =>
                    {
                        _msgLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.SyncWarningMapUnitFormatMessage, vxUnit);
                        _resultCounts.WarningCount++;
                        return "STK";
                    }
                ))()
        };

        return svUnit;
    }
}