namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

public class UploadImagesJobConfig : SvBaseJobConfig<UploadImagesJob>
{
    public string ArticleImageSourcePath { get; set; } = "E:\\Bilder\\Artikel";

    public string ArticleImageArchivePath { get; set; } = "E:\\Bilder\\Artikel\\Inaktive Artikel";
}