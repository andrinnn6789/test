using System.Collections.Generic;

namespace IAG.Infrastructure.BarcodeScanner;

public class BarcodeScanOptions
{
    public IEnumerable<BarcodeType> PossibleBarcodeTypes { get; set; }
    public bool ContainsBarcodeOnly { get; set; }
}