using System;
using System.IO;

using IAG.VinX.Zweifel.S1M.CoreServer;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;

namespace IAG.VinX.Zweifel.S1M.BusinessLogic;

public class S1MMediaWriter : IS1MMediaWriter
{
    private readonly S1MPluginConfig _config;

    public S1MMediaWriter(S1MPluginConfig config)
    {
        _config = config;
    }

    public bool WriteMedia(UploadMediaRequestModel reqModel)
    {
        var folderPath = Path.Combine(_config.S1MMediaFolderPath, reqModel.DocumentNumber.ToString());
        Directory.CreateDirectory(folderPath);
        foreach (var media in reqModel.Mediae) File.WriteAllBytes(Path.Combine(folderPath, media.Filename), Convert.FromBase64String(media.Content));

        return true;
    }
}