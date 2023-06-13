using System.Collections.Generic;
using System.Globalization;

using IAG.VinX.Smith.BossExport.Dto;

using OfficeOpenXml;

namespace IAG.VinX.Smith.BossExport.BusinessLogic;

public class ExcelWriter
{
    private ExcelWorksheet _wsTranslations;

    public byte[] GetExcel(IEnumerable<ArticleBoss> articles)
    {
        var excelPkg = new ExcelPackage();
        _wsTranslations = excelPkg.Workbook.Worksheets.Add("Articles");
        AddHeader();
        var rowNum = 2;
        foreach (var article in articles)
        {
            AddRow(rowNum++, article);
        }
        _wsTranslations.Cells[_wsTranslations.Dimension.Address].AutoFitColumns();
        return excelPkg.GetAsByteArray();
    }

    private void AddHeader()
    {
        AddHeaderCell("A1", "Haupt-EAN");
        AddHeaderCell("B1", "Artikeltyp");
        AddHeaderCell("C1", "Bezeichnung");
        AddHeaderCell("D1", "Kurzbezeichnung (Kassenbon)");
        AddHeaderCell("E1", "Regalbezeichnung1");
        AddHeaderCell("F1", "Regalbezeichnung2");
        AddHeaderCell("G1", "Packungsgrösse");
        AddHeaderCell("H1", "Nettoinhalt");
        AddHeaderCell("I1", "Referenzpreismenge");
        AddHeaderCell("J1", "Inhaltsmengeneinheit");
        AddHeaderCell("K1", "Vorgez. Recycling Gebühr");
        AddHeaderCell("L1", "nSTA Gewichtsartikel");
        AddHeaderCell("M1", "Mehrwertsteuer");
        AddHeaderCell("N1", "Verknüpfte Barcode (Pfand)");
        AddHeaderCell("O1", "Promotionen");
        AddHeaderCell("P1", "Produktionsartikel");
        AddHeaderCell("Q1", "Tagesartikel");
        AddHeaderCell("R1", "Einkaufspreis");
        AddHeaderCell("S1", "Verkaufspreis");
        AddHeaderCell("T1", "Alterslimite");
        AddHeaderCell("U1", "BOSS-Struktur");
        AddHeaderCell("V1", "Zusatz-EAN 1");
        AddHeaderCell("W1", "Zusatz-EAN 2");
        AddHeaderCell("X1", "Zusatz-EAN 3");
        AddHeaderCell("Y1", "Zusatz-EAN 4");
        AddHeaderCell("Z1", "Zusatz-EAN 5");
    }

    private void AddRow(int rowNum, ArticleBoss article)
    {
        AddDataCell($"A{rowNum}", article.Ean.ToString());
        AddDataCell($"B{rowNum}", "Stück");
        AddDataCell($"C{rowNum}", article.Description);
        AddDataCell($"D{rowNum}", article.DescriptionShort);
        AddDataCell($"M{rowNum}", article.VatRate.ToString("0.0", CultureInfo.InvariantCulture));
        AddDataCell($"N{rowNum}", article.EanPackage == 0 ? string.Empty : article.EanPackage.ToString());
        AddDataCell($"O{rowNum}", "Ja");
        AddDataCell($"P{rowNum}", "Nein");
        AddDataCell($"Q{rowNum}", "Nein");
        AddDataCell($"S{rowNum}", article.PriceSell.ToString("#,##0.00", CultureInfo.InvariantCulture));
        AddDataCell($"T{rowNum}", article.AgeMin.ToString());
        AddDataCell($"U{rowNum}", article.Structure);
        AddDataCell($"V{rowNum}", article.EanContainer == 0 ? string.Empty : article.EanContainer.ToString());
    }

    private void AddHeaderCell(string cell, string name)
    {
        using var rng = _wsTranslations.Cells[cell];
        rng.Value = name;
        rng.Style.Font.Bold = true;
    }

    private void AddDataCell(string cell, string name)
    {
        using var rng = _wsTranslations.Cells[cell];
        rng.Value = name;
    }
}