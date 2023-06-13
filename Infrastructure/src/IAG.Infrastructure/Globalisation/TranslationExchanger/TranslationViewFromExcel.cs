using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OfficeOpenXml;

namespace IAG.Infrastructure.Globalisation.TranslationExchanger;

public class TranslationViewFromExcel
{
    private List<TranslationView> _translations;
    private ExcelWorksheet _wsTranslations;

    public IList<TranslationView> GetTranslations(byte[] fromExcel)
    {
        using (var stream = new MemoryStream(fromExcel))
        {
            var excelPkg = new ExcelPackage();
            excelPkg.Load(stream);
            _wsTranslations = excelPkg.Workbook.Worksheets.First(w => w.Name ==  "Translations");
            _translations = new List<TranslationView>();
            GetTranslations(ExtractLanguages());
        }

        return _translations;
    }

    private void GetTranslations(Dictionary<int, string> languages)
    {
        var start = _wsTranslations.Dimension.Start;
        var end = _wsTranslations.Dimension.End;
        for (var row = start.Row + 1; row <= end.Row; row++)
        { 
            for (var col = start.Column + 1; col <= end.Column; col++)
            {
                var cell = _wsTranslations.Cells[row, col];
                var cellValue = cell.Text;
                var cellPos = col;
                if (string.IsNullOrWhiteSpace(cellValue))
                    continue;
                var id = string.Empty;
                if (cell.Comment != null)
                    id = cell.Comment.Text;
                _translations.Add(new TranslationView
                {
                    Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid() : Guid.Parse(id),
                    ResourceName = _wsTranslations.Cells[row, 1].Text,
                    CultureName = languages[cellPos],
                    Translation = cellValue
                });
            }
        }
    }

    private Dictionary<int, string> ExtractLanguages()
    {
        var languages = new Dictionary<int, string>();
        var start = _wsTranslations.Dimension.Start;
        var end = _wsTranslations.Dimension.End;
        for (var col = start.Column + 1; col <= end.Column; col++)
        {
            var language = _wsTranslations.Cells[start.Row, col].Text;
            languages.Add(col, language);
        }

        return languages;
    }
}