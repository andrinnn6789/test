using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // MainJob
        AddTemplate(ResourceIds.JobErrorMainSync, "de", "SyncJob für helloTess {0} war nicht erfolgreich.");

        // CommonJob
        AddTemplate(ResourceIds.SyncErrorGetSource, "de", "Fehler beim Abrufen der {0} von Atlas2 REST-Server: {1}");
        AddTemplate(ResourceIds.SyncErrorGetTarget, "de", "Fehler beim Abrufen der {0} von HelloTess: {1}");
        AddTemplate(ResourceIds.SyncErrorUpdateFormatMessage, "de", "Aktualisieren von {0} '{1}' fehlgeschlagen: {2}");
        AddTemplate(ResourceIds.SyncErrorInsertFormatMessage, "de", "Einfügen von {0} '{1}' fehlgeschlagen: {2}");
        AddTemplate(ResourceIds.SyncErrorDeleteFormatMessage, "de", "Inaktivstellen von {0} '{1}' fehlgeschlagen: {2}");

        AddTemplate(ResourceIds.ArticleGroupSingular, "de", "Artikelkategorie");
        AddTemplate(ResourceIds.ArticleGroupPlural, "de", "Artikelkategorien");
        AddTemplate(ResourceIds.ArticleSingular, "de", "Artikel");
        AddTemplate(ResourceIds.ArticlePlural, "de", "Artikel");

        // jobs
        AddTemplate(ResourceIds.ExtractorJobName, "de", "Export Verkaufsmappe");
        AddTemplate(ResourceIds.HelloTessArticleCommonSyncJobName, "de", "helloTess interner Hilfsjob (Export Artikel)");
        AddTemplate(ResourceIds.HelloTessMainSyncJobName, "de", "helloTess Abgleich aller Kassen");
        AddTemplate(ResourceIds.BossExportName, "de", "BOSS/Migros einmaliger Export Artikel");
    }
}