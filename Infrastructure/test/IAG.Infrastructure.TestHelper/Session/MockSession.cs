using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace IAG.Infrastructure.TestHelper.Session;

public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStore = new();

    public void Clear()
    {
        _sessionStore.Clear();
    }

    public Task CommitAsync(CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _sessionStore.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        _sessionStore[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        return _sessionStore.TryGetValue(key, out value);
    }

    public string Id => "MockSessionStore";
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => _sessionStore.Keys;
}