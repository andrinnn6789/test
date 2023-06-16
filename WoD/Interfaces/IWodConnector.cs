using System.Threading.Tasks;

using IAG.Common.WoD.Dto;
using IAG.Infrastructure.Globalisation.Localizer;

namespace IAG.Common.WoD.Interfaces;

public interface IWodConnector
{
    Task<byte[]> SubmitJob(byte[] zip, string jobType);

    Task<WodConnectionResult<LocalizableParameter>> CheckWodConnectionAsync();
}