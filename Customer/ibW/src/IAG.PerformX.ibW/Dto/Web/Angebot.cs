using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.ibW.Dto.Web;

[UsedImplicitly]
[Table("REST3Angebot")]
public class Angebot
{
    public string Nummer { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Variante { get; set; }
    public int AnzahlTeilnehmer { get; set; }
    public int MinTeilnehmer { get; set; }
    public int MaxTeilnehmer { get; set; }
    public int MaxWarteliste { get; set; }
    public string SuchfelderInternet { get; set; }
    [JsonIgnore]
    [Column("Publizieren")]
    public AtlasBoolean PublizierenAtlas { get; set; }
    [NotMapped]
    public bool Publizieren
    {
        get => PublizierenAtlas;
        set => PublizierenAtlas = value;
    }
    public int AnmeldeFormularId { get; set; }
    public DateTime? PublizierenAb { get; set; }
    public DateTime? PublizierenBis { get; set; }
    [Column("Angebot")]
    [JsonProperty("Angebot")]
    public string AngebotName { get; set; }
    public string AnmeldeFormularName { get; set; }
    public string Kursort { get; set; }
    public string Status { get; set; }
    public string Standort { get; set; }
    public DateTime? Beginn { get; set; }
    public DateTime? Ende { get; set; }
    public string Kosten { get; set; }
    public int Sonntag { get; set; }
    public int Montag { get; set; }
    public int Dienstag { get; set; }
    public int Mittwoch { get; set; }
    public int Donnerstag { get; set; }
    public int Freitag { get; set; }
    public int Samstag { get; set; }
    public int DiverseTage { get; set; }
    public int DiverseOderMehrereTage { get; set; }
    public string Weiterbildung { get; set; }
    public string WeiterbildungText { get; set; }
    public bool StartdatumAufAnfrage { get; set; }
    public DateTime LastChange { get; set; }

    [NotMapped]
    public List<AngebotZusatz> Zusatz { get; set; }
    [NotMapped]
    public List<AngebotECommerce> ECommerce { get; set; }
}