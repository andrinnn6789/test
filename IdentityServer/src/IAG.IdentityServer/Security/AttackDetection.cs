using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration.DataLayer.State;
using IAG.IdentityServer.Configuration.Model.Config;
using IAG.Infrastructure.IdentityServer;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.IdentityServer.Security;

[UsedImplicitly]
public class AttackDetection : IAttackDetection
{
    private readonly IdentityStateDbContext _context;
    private readonly TimeSpan _observationPeriod;
    private readonly int _maxFailedRequests;

    public AttackDetection(IdentityStateDbContext context, IAttackDetectionConfig config)
    {
        _context = context;
        _observationPeriod = config.ObservationPeriod;
        _maxFailedRequests = config.MaxFailedRequests;
    }

    public async Task<bool> CheckRequest(string realm, string user, string request = null)
    {
        try
        {
            var observationStart = DateTime.UtcNow.Subtract(_observationPeriod);

            var lastRequests = _context.FailedRequestEntries.Where(x => x.Realm == realm && x.User == user && x.Timestamp >= observationStart);
            if (!string.IsNullOrEmpty(request))
            {
                lastRequests = lastRequests.Where(x => x.Request == request);
            }

            return await lastRequests.CountAsync() < _maxFailedRequests;
        }
        catch (Exception)
        {
            return true;
        }
    }

    public async Task AddFailedRequest(string realm, string user, string request)
    {
        try
        {
            var entry = new FailedRequestDb()
            {
                Id = Guid.NewGuid(),
                Realm = realm,
                User = user,
                Timestamp = DateTime.UtcNow,
                Request = request
            };

            _context.FailedRequestEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task ClearFailedRequests(string realm, string user, string request = null)
    {
        try
        {
            var requests = _context.FailedRequestEntries.Where(x => x.Realm == realm && x.User == user);
            if (!string.IsNullOrEmpty(request))
            {
                requests = requests.Where(x => x.Request == request);
            }

            _context.RemoveRange(requests);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}