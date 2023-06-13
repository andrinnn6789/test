using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.PerformX.ibW.Dto.Web;

namespace IAG.PerformX.ibW.Sybase;

public class WebClient
{
    private readonly ISybaseConnection _connection;

    public WebClient(ISybaseConnection sybaseConnection)
    {
        _connection = sybaseConnection;
    }

    public List<Angebot> AddSubLinks(IQueryable<Angebot> angebote)
    {
        var subZusatz = _connection.GetQueryable<AngebotZusatz>()
            .Join(angebote, z => z.AngebotId, a => a.Id, (z, _) => z)
            .ToList();
        var subECommerce = _connection.GetQueryable<AngebotECommerce>()
            .Join(angebote, e => e.AngebotId, a => a.Id, (e, _) => e)
            .ToList();
        var angebotList = angebote.ToList();

        foreach (var angebot in angebotList)
        {
            angebot.Zusatz = subZusatz.Where(z => z.AngebotId == angebot.Id).ToList();
            if (angebot.Zusatz.Count > 0)
            {
                var maxSubChange = angebot.Zusatz.Max(s => s.LastChange);
                if (angebot.LastChange < maxSubChange)
                    angebot.LastChange = maxSubChange;
            }

            angebot.ECommerce = subECommerce.Where(e => e.AngebotId == angebot.Id).ToList();
            if (angebot.ECommerce.Count > 0)
            {
                var maxSubChange = angebot.ECommerce.Max(s => s.LastChange);
                if (angebot.LastChange < maxSubChange)
                    angebot.LastChange = maxSubChange;
            }
        }

        return angebotList;
    }
}