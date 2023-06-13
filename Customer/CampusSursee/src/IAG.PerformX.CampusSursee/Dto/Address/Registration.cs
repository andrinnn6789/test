using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Address;

[TableCte(@"
       WITH Registration (
            Id, AddressId, BillingAddressId, KstCustomer, HrAddressId, EventId, 
            EventParentId, NumberOfAttendees, AdditionalAttendees, Remark, EmailForEventDocuments, 
            EmailForEventDocumentsCc, WebLinkForUserDocuments, StatusName, StatusId, RoleId, LastChange
        ) AS (
        SELECT 
            Ver_ID, Ver_AdresseID, Ver_RechnungsadresseID, Ver_KSTKunde, Ver_AdresseIDHRPerson, Ver_VertragDefID, 
            IsNull(VerDefParent.VerDef_ID, Ver_VertragDefID), Ver_Personen, Ver_NamenPersonen, Ver_Bemerkungen, Ver_EMailZiel1, 
            Ver_EMailZiel2, Ver_DokumenteWeb , Status_Bezeichnung, Status_Id, Ver_RolleId,
            Ver_ChangedOn        
        FROM Vertrag
        JOIN VertragStatus ON Status_Id = Ver_StatusId
        JOIN Adresse Adr ON Adr.Adr_Id = Ver_AdresseID
        LEFT OUTER JOIN VertragDefBeziehung ON EreigBez_VertragDefID = Ver_VertragDefID and EreigBez_BeziehungstypID IN (8)
        LEFT OUTER JOIN VertragDef VerDefParent ON VerDefParent.VerDef_Id = EreigBez_GegenVertragDefID
        WHERE " + ExportFilter + " AND " + Address.ExportFilter + @" 
        )        
        ")]
[UsedImplicitly]
public class Registration
{
    private const string ExportFilter = "Ver_BereichId = 2";

    public int Id { get; set; }
    public int AddressId { get; set; }
    public int BillingAddressId { get; set; }
    public string KstCustomer { get; set; }
    public int HrAddressId { get; set; }
    public int EventId { get; set; }
    public int EventParentId { get; set; }
    public int NumberOfAttendees { get; set; }
    public string AdditionalAttendees { get; set; }
    public string Remark { get; set; }
    public string EmailForEventDocuments { get; set; }
    public string EmailForEventDocumentsCc { get; set; }
    public string WebLinkForUserDocuments { get; set; }
    public string StatusName { get; set; }
    public int StatusId { get; set; }
    public int RoleId { get; set; }
    public DateTime LastChange { get; set; }
}