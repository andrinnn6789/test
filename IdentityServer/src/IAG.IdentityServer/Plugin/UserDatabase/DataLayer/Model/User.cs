using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

[ExcludeFromCodeCoverage]
[UsedImplicitly]
public class User : BaseEntityWithTenant, ITenantUniqueNamedEntity
{
    private string _culture;
    public string Name { get; set; }
    public string Salt { get; set; }
    public string Password { get; set; }
    public bool ChangePasswordAfterLogin { get; set; }
    public string EMail { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Culture
    {
        get => _culture;
        set
        {
            if (value != null)
            {
                var unused = new CultureInfo(value);  // check valid culture
            }

            _culture = value;
        }
    }
}