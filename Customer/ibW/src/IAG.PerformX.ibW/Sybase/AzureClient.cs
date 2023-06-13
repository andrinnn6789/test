using System.Data;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.PerformX.ibW.Dto.Azure;

namespace IAG.PerformX.ibW.Sybase;

public class AzureClient
{
    private readonly ISybaseConnection _connection;

    public AzureClient(ISybaseConnection sybaseConnection)
    {
        _connection = sybaseConnection;
    }

    public void SetCloudLogin(int id, PersonChangeParam changeParam)
    {
        if (string.IsNullOrWhiteSpace(changeParam.CloudEMail) && string.IsNullOrWhiteSpace(changeParam.CloudLogin))
            throw new NoNullAllowedException("CloudEmail and CloudLogin must not both be empty");

        var address = GetAddress(id);
        if(!string.IsNullOrWhiteSpace(changeParam.CloudLogin))
            address.CloudLogin = changeParam.CloudLogin;
        if (!string.IsNullOrWhiteSpace(changeParam.CloudEMail))
            address.CloudEMail = changeParam.CloudEMail;
        _connection.Update(address);
    }

    public void ResetCloudLogin(int id)
    {
        var address = GetAddress(id);
        address.CloudLogin = null;
        address.CloudEMail = null;
        _connection.Update(address);
    }

    private PersonChange GetAddress(int id)
    {
        var address = _connection.GetQueryable<PersonChange>().FirstOrDefault(a => a.Id == id);
        if (address == null)
            throw new NotFoundException(id.ToString());
        return address;
    }
}