using System.Collections.Generic;
using System.Linq;

using IAG.VinX.Smith.BossExport.Dto;

namespace IAG.VinX.Smith.BossExport.BusinessLogic;

public class NewIdFinder
{
    public List<ArticleBoss> GetNewArticles(List<ArticleBoss> articles, List<int> exportedIds, bool updateList, bool all)
    {
        var newArticles = (all ? articles : articles.Where(a => !exportedIds.Contains(a.Id))).ToList();
        if (updateList)
        {
            if (all)
                exportedIds.Clear();
            exportedIds.AddRange(newArticles.Select(a => a.Id));
        }

        return newArticles;
    }
}