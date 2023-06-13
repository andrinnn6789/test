using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ObjectMapper;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;

public class IssueToPendenzMapper : ObjectMapper<Issue, Pendenz>
{
    private readonly PendenzSyncSettings _pendenzSyncSettings;
    private readonly List<PendenzSyncDetailSettings> _vxPendenzSyncDetailSettings;
    private readonly List<VinXUser> _vxMitarbeiter;
    private readonly ISybaseConnection _sybaseConnection;
    private IMessageLogger MessageLogger { get; }
    private Issue _issue;
    private Pendenz _pendenz;

    public IssueToPendenzMapper(IMessageLogger msgLogger, ISybaseConnection sybaseConnection, PendenzSyncSettings pendenzSyncSettings, List<PendenzSyncDetailSettings> vxPendenzSyncDetailSettings)
    {
        MessageLogger = msgLogger;
        _sybaseConnection = sybaseConnection;
        _pendenzSyncSettings = pendenzSyncSettings;
        _vxPendenzSyncDetailSettings = vxPendenzSyncDetailSettings;

        _vxMitarbeiter = _sybaseConnection.GetQueryable<VinXUser>().ToList().Where(m => m.IsActive).ToList();
    }

    public JobResultEnum MapperResult { get; set; } = JobResultEnum.Success;

    protected override Pendenz MapToDestination(Issue source, Pendenz destination)
    {
        _issue = source;
        _pendenz = destination;

        StraightMapping();
        ConcatInfoField();
        AddressMapping();
        UserMapping();
        StatusMapping();
        KostenartMapping();
        SystemLinkMapping();
        BillingLinkMapping();

        return destination;
    }

    private static DateTime? SetDateTimeKind(DateTime? dateTime, DateTimeKind kind)
    {
        if (dateTime.HasValue)
        {
            return DateTime.SpecifyKind(dateTime.Value, kind);
        }

        return null;
    }

    private void StraightMapping()
    {
        _pendenz.JiraKey = _issue.Key;
        _pendenz.Bezeichnung = _issue.Fields.Summary;
        if (_issue.Fields.Aggregatetimeoriginalestimate != null) _pendenz.Aufwand = (decimal) _issue.Fields.Aggregatetimeoriginalestimate / 3600;
        if (_issue.Fields.Expenses != null) _pendenz.OffertBetrag = (decimal) _issue.Fields.Expenses;

        var vxErfassungsdatum = _issue.Fields.Created;
        _pendenz.Erfassungsdatum = SetDateTimeKind(vxErfassungsdatum, DateTimeKind.Utc);

        var vxEnde = _issue.Fields.Duedate;
        _pendenz.Ende = SetDateTimeKind(vxEnde, DateTimeKind.Utc);

        var vxDatum = _issue.Fields.Resolutiondate;
        _pendenz.Datum = SetDateTimeKind(vxDatum, DateTimeKind.Utc);
    }

    private void ConcatInfoField()
    {
        var project = _issue.Fields.Project.Name;
        var issueType = _issue.Fields.Issuetype.Name;
        var status = _issue.Fields.Status.Name;

        _pendenz.JiraInfo = $"Projekt: {project} | Vorgangstyp: {issueType} | Status: {status}";
    }

    private void AddressMapping()
    {
        var organizationKey = _issue.Fields.Organizations?.FirstOrDefault()?.Id;
        var projectKey = _issue.Fields.Project.Key;

        var adresseId = organizationKey != null
            ? _sybaseConnection.GetQueryable<Address>().Where(a => a.JiraOrganisation == organizationKey).ToList().FirstOrDefault()?.ID
            : _sybaseConnection.GetQueryable<Address>().Where(a => a.JiraBusinessProjekt == projectKey).ToList().FirstOrDefault()?.ID;

        if (_pendenz.AdresseID != 0 && adresseId == null)
            adresseId = _pendenz.AdresseID;

        _pendenz.AdresseID = adresseId ?? _pendenzSyncSettings.AdresseID;
    }

    private void UserMapping()
    {
        var assigneeKey = _issue.Fields.Assignee.Key;
        _pendenz.ErfasserID = _pendenzSyncSettings.SyncUserID;
        _pendenz.MitarbeiterID = GetMitarbeiterId(assigneeKey);
    }

    private void StatusMapping()
    {
        var statusCategory = _issue.Fields.Status.StatusCategory.Key;
        _pendenz.Status = GetStatusId(statusCategory);
    }

    private void KostenartMapping()
    {
        var projectKey = _issue.Fields.Project.Key;
        var projectCat = _issue.Fields.Project.ProjectCategory.Name;

        var projectKeySettings = _vxPendenzSyncDetailSettings.FirstOrDefault(ds => ds.ProjectSettingType == 10 && ds.Name == projectKey);
        var projectCatSettings = _vxPendenzSyncDetailSettings.FirstOrDefault(ds => ds.ProjectSettingType == 20 && ds.Name == projectCat);

        int kostenartId;
        int kostenstelleId;
        if (projectKeySettings != null)
        {
            kostenartId = projectKeySettings.KostenartID;
            kostenstelleId = projectKeySettings.KostenstelleID;
        }
        else if (projectCatSettings != null)
        {
            kostenartId = projectCatSettings.KostenartID;
            kostenstelleId = projectCatSettings.KostenstelleID;
        }
        else
        {
            kostenartId = _pendenzSyncSettings.KostenartID;
            kostenstelleId = _pendenzSyncSettings.KostenstelleID;
        }

        if (_pendenz.KostenartID.HasValue && _pendenz.KostenartID != kostenartId ||
            _pendenz.KostenstelleID.HasValue && _pendenz.KostenstelleID != kostenstelleId)
        {
            return; // don't overwrite!
        }

        _pendenz.KostenartID = kostenartId;
        _pendenz.KostenstelleID = kostenstelleId;
    }

    private void SystemLinkMapping()
    {
        var systemLinkIssueKey = _issue.Fields.Issuetype.Subtask ? _issue.Fields.Parent.Key : _issue.Fields.EpicLink;
        if (systemLinkIssueKey == null)
        {
            _pendenz.PendenzID = null;
            return;
        }

        var pendenzLinkId = GetPendenzId(systemLinkIssueKey);
        if (pendenzLinkId == null) return;
        if (_pendenz.PendenzID != null && !CheckOverwriteSystemLink((int)_pendenz.PendenzID)) return;

        _pendenz.PendenzID = pendenzLinkId;
    }

    private void BillingLinkMapping()
    {
        var billingLinks =
            _issue.Fields.IssueLinks.Where(il => il.Type.Name == "Verrechnung" && il.InwardIssue != null).ToList();

        switch (billingLinks.Count)
        {
            case 0:
                _pendenz.PendenzVerrechnungID = null;
                return;
            case 1:
                var pendenzLinkId = GetPendenzId(billingLinks.First().InwardIssue.Key);
                if (pendenzLinkId != null)
                    _pendenz.PendenzVerrechnungID = pendenzLinkId;
                    
                break;
            default:
                MessageLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.MultipleBillingLinksErrorFormatMessage, _issue.Key);
                MapperResult = JobResultEnum.PartialSuccess;
                return;
        }
    }

    private int? GetPendenzId(string jiraKeyLink)
    {
        var vxPendenzen = _sybaseConnection.GetQueryable<Pendenz>().Where(p => p.JiraKey == jiraKeyLink).ToList();

        switch (vxPendenzen.Count)
        {
            case 0:
                MessageLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.NoPendenzForLinkFoundErrorFormatMessage, _issue.Key, jiraKeyLink);
                MapperResult = JobResultEnum.PartialSuccess;
                return null;
            case 1:
                return vxPendenzen.First().ID;
            default:
                MessageLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.MultiplePendenzenForLinkErrorFormatMessage, _issue.Key, jiraKeyLink);
                MapperResult = JobResultEnum.PartialSuccess;
                return null;
        }
    }

    private bool CheckOverwriteSystemLink(int pendenzId)
    {
        var vxPendenzen = _sybaseConnection.GetQueryable<Pendenz>().Where(p => p.ID == pendenzId).ToList();

        // do not overwrite links with non-jira Pendenz
        return vxPendenzen.FirstOrDefault()?.JiraKey != null;
    }

    private int GetMitarbeiterId(string uref)
    {
        var mitarbeiter = _vxMitarbeiter.FirstOrDefault(m => m.URef?.ToLower() == uref.ToLower());

        return mitarbeiter?.ID ?? _pendenzSyncSettings.SyncUserID;
    }

    private int GetStatusId(string statusCategory)
    {
        return statusCategory.ToLowerInvariant() switch
        {
            "new" => _pendenzSyncSettings.PendenzstatusOpenID,
            "indeterminate" => _pendenzSyncSettings.PendenzstatusProgressID,
            "done" => _pendenzSyncSettings.PendenzstatusDoneID,
            _ => _pendenzSyncSettings.PendenzstatusOpenID
        };
    }
}