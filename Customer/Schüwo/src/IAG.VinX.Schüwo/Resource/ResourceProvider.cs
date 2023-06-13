using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.Schüwo.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // jobs
        AddTemplate(ResourceIds.UploadBaseDataJobName, "de", "SV Upload Stammdaten");
        AddTemplate(ResourceIds.UploadImagesJobName, "de", "SV Upload Bilder");
        AddTemplate(ResourceIds.UploadOrderDataJobName, "de", "SV Upload Archiv Bestelldaten");
        AddTemplate(ResourceIds.DownloadOrderJobName, "de", "SV Download Bestellungen");

        // SV Sync Jobs
        AddTemplate(ResourceIds.SyncWarningMapUnitFormatMessage, "de", "Die VinX-Einheit '{0}' konnte keiner Einheit der SV-Plattform zugeordnet werden. Es wurde der Standard 'STK' eingetragen.");
        AddTemplate(ResourceIds.SyncWarningUnsupportedImageExtensionFormatMessage, "de", "Die Dateiendung der Bild-Datei '{0}' wird von der SV-Plattform nicht unterstützt. Die Datei wird übersprungen.");
        AddTemplate(ResourceIds.SyncWarningInvalidImageFileNameFormatMessage, "de", "Die Bild-Datei '{0}' enthält ungültige Zeichen. Es werden nur Zahlen (Artikelnummer) im Namen unterstützt. Die Datei wird übersprungen.");
        AddTemplate(ResourceIds.SyncWarningSyncImageFormatMessage, "de", "Fehler beim Synchronisieren der Bild-Datei '{0}': {1}");
        AddTemplate(ResourceIds.SyncWarningDeleteImageFormatMessage, "de", "Fehler beim Löschen der Bild-Datei '{0}' auf dem FTP: {1}");
        AddTemplate(ResourceIds.SyncErrorUnknownArticle, "de", "Fehler beim Auftragsimport: Artikel {0} ist unbekannt");
    }
}