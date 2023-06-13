using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace IAG.InstallClient.Controllers;

public class LinkListController : BaseController
{
    private readonly ILinkListManager _linkListManager;

    public LinkListController(ILinkListManager linkListManager, ICustomerManager customerManager, IStringLocalizer<LinkListController> stringLocalizer) : base(customerManager, stringLocalizer)
    {
        _linkListManager = linkListManager;
    }

    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public async Task<IActionResult> Index()
    {
        var model = new LinkListViewModel();
        if (Customer != null)
        {
            try
            {
                model.Links = (await _linkListManager.GetLinksAsync(Customer.Id)).ToList();
            }
            catch (Exception ex)
            {
                model.Links = new List<LinkInfo>();
                model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
            }
        }

        return ViewCustomerChecked(model);
    }
}