using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace IAG.VinX.Zweifel.MySign.Dto;

public enum DiffEnum
{
    New,
    Equal,
    DiffShopId,
    Diff,
    IsB2B
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false, ElementName = "kunden")]
public class Customers
{
    [XmlElement("adresse")]
    public List<Customer> Items { get; } = new();
}

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Customer
{
    public string adresstyp { get; set; } = "H";
    public string adrnbr { get; set; } = string.Empty;
    public string debinbr { get; set; } = string.Empty;
    public string whgid { get; set; } = "CHF";
    public string anrede { get; set; } = string.Empty;
    public string namefirma { get; set; } = string.Empty;
    public string vorname { get; set; } = string.Empty;
    public string adresse1 { get; set; } = string.Empty;
    public string adresse2 { get; set; } = string.Empty;
    public string adresse3 { get; set; } = string.Empty;
    public string postfach { get; set; } = string.Empty;
    public string landcode { get; set; } = string.Empty;
    public string plz { get; set; } = string.Empty;
    public string ort { get; set; } = string.Empty;
    public string telefon1 { get; set; } = string.Empty;
    public string telefon2 { get; set; } = string.Empty;
    public string telefax { get; set; } = string.Empty;
    public string mobil { get; set; } = string.Empty;
    public string email1 { get; set; } = string.Empty;
    public string email2 { get; set; } = string.Empty;
    public string email3 { get; set; } = string.Empty;
    public ushort sprachcode { get; set; } = 2055;
    public decimal kreditlimite { get; set; }
    public string werbungjanein { get; set; } = "J";
    public decimal saldo { get; set; }
    public string preisgruppe { get; set; } = "P01";
    public string rabattgruppe { get; set; } = "R01";
    public byte flag { get; set; }
    public uint idshop { get; set; }
    public uint idshopadr { get; set; }
    public byte zahlkond { get; set; } = 3;
    public string sperrcode { get; set; } = string.Empty;
    public string b2b { get; set; }

    public DiffEnum Equals(Customer other)
    {
        if (other == null)
            return DiffEnum.New;
        if (other.b2b == "b2b" || b2b == "b2b")
            return DiffEnum.IsB2B;
        var equalWithoutShopId =
            adresstyp == other.adresstyp &&
            adrnbr == other.adrnbr &&
            debinbr == other.debinbr &&
            //anrede == other.anrede &&   // gem. Peter nicht notwendig
            namefirma == other.namefirma &&
            vorname == other.vorname &&
            adresse1 == other.adresse1 &&
            adresse2 == other.adresse2 &&
            adresse3 == other.adresse3 &&
            postfach == other.postfach &&
            plz == other.plz &&
            ort == other.ort &&
            telefon1 == other.telefon1 &&
            telefon2 == other.telefon2 &&
            mobil == other.mobil &&
            email1 == other.email1 &&
            sperrcode == other.sperrcode;
        if (!equalWithoutShopId)
            return DiffEnum.Diff;
        var equalShopId = idshop == other.idshop;
        return equalShopId ? DiffEnum.Equal: DiffEnum.DiffShopId;
    }
}