using System.Collections.Generic;

namespace IAG.Infrastructure.ProcessEngine.Configuration;

public interface IJobConfigProvider
{
    IEnumerable<IJobConfig> GetAll();
}