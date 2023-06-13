namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

public class UploadImagesJobResult : SvBaseJobResult
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int ArticleImagesCount { get; set; }

    public int ImagesMovedCount { get; set; }
}