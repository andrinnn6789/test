namespace IAG.Infrastructure.Startup;

public interface IServiceLifetime
{
    void OnStart();
    void OnStop();
}