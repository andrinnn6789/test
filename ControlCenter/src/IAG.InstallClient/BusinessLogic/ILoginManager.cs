using System.Threading.Tasks;

namespace IAG.InstallClient.BusinessLogic;

public interface ILoginManager
{
    Task<string> DoLoginAsync(string username, string password);
}