using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IAG.Infrastructure.Globalisation.TranslationExchanger;

public class TranslationViewToExcel
{
    private List<string> _languages;
    private List<string> _resources;
    private TranslationView[] _backendTranslations;
    private ExcelWorksheet _wsTranslations;

    public byte[] BuildExcel(TranslationView[] translations)
    {
        _backendTranslations = translations.OrderBy(t => t.ResourceName).ThenBy(t => t.CultureName).ToArray();
        var excelStructure = new TranslationExcelStructure();
        _languages = excelStructure.ExtractCultures(_backendTranslations);
        _resources = excelStructure.ExtractResources(_backendTranslations);

        var excelPkg = new ExcelPackage();
        _wsTranslations = excelPkg.Workbook.Worksheets.Add("Translations");
        AddContent();
        _wsTranslations.Cells[_wsTranslations.Dimension.Address].AutoFitColumns();
        return excelPkg.GetAsByteArray();
    }

    private void AddContent()
    {
        AddHeader();
        AddData();
    }

    private void AddHeader()
    {
        using (var rng = _wsTranslations.Cells["A1"])
        {
            rng.Value = "Resource / Language";
            SetBackgroundColor(rng, Color.DarkTurquoise);
        }

        for (int i = 0; i < _languages.Count; i++)
        {
            using (var rng = _wsTranslations.Cells[(char)(i + 66) + "1"])
            {
                rng.Value = _languages[i];
                SetBackgroundColor(rng, Color.DarkTurquoise);
            }
        }
    }

    private void AddData()
    {
        for (int r = 0; r < _resources.Count; r++)
        {
            using (var rng = _wsTranslations.Cells["A" + (r + 2)])
            {
                rng.Value = _resources[r];
                SetBackgroundColor(rng, Color.DarkTurquoise);
            }
            for (int i = 0; i < _languages.Count; i++)
            {
                var translation = _backendTranslations.FirstOrDefault(t => t.ResourceName == _resources[r] && t.CultureName == _languages[i]);
                {
                    if (translation == null)
                        continue;
                    using (var rng = _wsTranslations.Cells[(char)(i + 66) + (r + 2).ToString()])
                    {
                        rng.Value = translation.Translation;
                        rng.AddComment(translation.Id.ToString(), "me");
                    }
                }
            }
        }
    }

    private void SetBackgroundColor(ExcelRange rng, Color color)
    {
        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        rng.Style.Fill.BackgroundColor.SetColor(color);
    }
}