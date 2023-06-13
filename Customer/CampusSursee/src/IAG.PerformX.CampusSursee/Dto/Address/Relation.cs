using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Dto.Address;

public static class Consts
{
    public const string RelationSql = @"
            WITH Relation (
                Id, AddressIdInbound, AddressIdOutbound, NameInbound, NameOutbound, 
                RelationTypeId, IsHr, IsBillingAddress, IsContactAddress,  
                LastChange
            ) AS (
            SELECT
                AdrIn.Adressbeziehung_ID, AdrIn.Adressbeziehung_AdresseID, AdrIn.Adressbeziehung_GegenAdresseID, BezIn.BezTyp_Bezeichnung, BezOut.BezTyp_Bezeichnung, 
                BezIn.BezTyp_ID, 
                CASE AdrIn.Adressbeziehung_HRPerson WHEN 0 THEN 0 ELSE 1 END, 
                CASE AdrIn.Adressbeziehung_IstRechnungsadresse WHEN 0 THEN 0 ELSE 1 END, 
                CASE AdrIn.Adressbeziehung_IstKorrespondenzadresse WHEN 0 THEN 0 ELSE 1 END, 
                GREATER(AdrIn.Adressbeziehung_ChangedOn, GREATER(BezIn.BezTyp_ChangedOn, BezOut.BezTyp_ChangedOn))
            FROM Beziehungstyp BezIn 
            JOIN Beziehungstyp BezOut ON BezOut.BezTyp_Id = BezIn.BezTyp_GegentypID
            JOIN Adressbeziehung AdrIn ON BezIn.BezTyp_Id = AdrIn.Adressbeziehung_BeziehungstypID
            JOIN Adresse ON Adr_Id = AdrIn.Adressbeziehung_AdresseID
            WHERE Adr_AufnahmeartID IN (2, 8) AND AdrIn.Adressbeziehung_BeziehungstypID = 1 AND Adressbeziehung_Aktiv = -1
            )";
}

[TableCte(Consts.RelationSql)]
[UsedImplicitly]
public class Relation
{
    public int Id { get; set; }
    public int AddressIdInbound { get; set; }
    public int AddressIdOutbound { get; set; }
    public string NameInbound { get; set; }
    public string NameOutbound { get; set; }
    public int RelationTypeId { get; set; }
    public bool IsHr { get; set; }
    public bool IsBillingAddress { get; set; }
    public bool IsContactAddress { get; set; }
    public DateTime LastChange { get; set; }
}