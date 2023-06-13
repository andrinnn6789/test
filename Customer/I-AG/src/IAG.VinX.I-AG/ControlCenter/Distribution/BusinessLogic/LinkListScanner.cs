using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class LinkListScanner : ILinkListScanner
{
    public async Task<IEnumerable<LinkData>> ScanAsync(string linkListPath)
    {
        try
        {
            var results = new List<LinkData>();
            var dirInfo = new DirectoryInfo(linkListPath);
            foreach(var linkListFile in dirInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly))
            {
                await using var linkListFileStream = linkListFile.OpenRead();
                var linkList = await JsonSerializer.DeserializeAsync<LinkList>(linkListFileStream);

                results.AddRange(linkList.Links ?? Enumerable.Empty<LinkData>());
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ScanConfigurationsError, ex, linkListPath);
        }
    }
}