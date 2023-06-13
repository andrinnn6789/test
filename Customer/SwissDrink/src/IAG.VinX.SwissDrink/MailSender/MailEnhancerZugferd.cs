using System.IO;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.Dto;
using IAG.Common.EBill.BusinessLogic;
using IAG.Common.MailSender;
using IAG.VinX.SwissDrink.Dto;

namespace IAG.VinX.SwissDrink.MailSender;

public class MailEnhancerZugferd: IMailEnhancer
{
    private readonly IZugferdEmbedder _zugferdEmbedder;
    private readonly ISybaseConnection _sybaseConnection;

    public MailEnhancerZugferd(IZugferdEmbedder zugferdEmbedder, ISybaseConnection sybaseConnection)
    {
        _sybaseConnection = sybaseConnection;
        _zugferdEmbedder = zugferdEmbedder;
    }

    public void Enhance(MemoryStream mailContent, int archiveLinkId)
    {
        var archiveLink = _sybaseConnection.GetQueryable<ArchiveLink>()
            .FirstOrDefault(a => a.Id == archiveLinkId && a.Tablename == "Beleg");
        if (archiveLink == null)
            return;

        var opData = _sybaseConnection.GetQueryable<OpData>().FirstOrDefault(o => o.ReceiptId == archiveLink.ForeignId);
        if (opData == null || !opData.HasZugferd)
            return;

        _zugferdEmbedder.EmbedZugferd(mailContent, archiveLink, opData.Id);
    }
}