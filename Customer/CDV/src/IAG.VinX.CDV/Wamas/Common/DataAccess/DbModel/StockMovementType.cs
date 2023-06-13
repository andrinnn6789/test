using System.ComponentModel;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

public enum StockMovementType : short
{
    [Description("Waren Eingang")] GoodsIncoming = 2,
    [Description("Waren Ausgang")] GoodsOutgoing = -2,
    [Description("Keller Eingang")] CellarIncoming = 3,
    [Description("Keller Ausgang")] CellarOutgoing = -3,
    [Description("Füllung Eingang")] FillingIncoming = 5,
    [Description("Füllung Ausgang")] FillingOutgoing = -5,
    [Description("Schwund Eingang")] LossIncoming = 7,
    [Description("Schwund Ausgang")] LossOutgoing = -7,
    [Description("Inventar Eingang")] InventoryIncoming = 8,
    [Description("Inventar Ausgang")] InventoryOutgoing = -8,
    [Description("Einlagerung")] Picking = 9,
    [Description("Auslagerung")] PutAway = -9,
    [Description("Produktion Eingang")] ProductionIncoming = 10,
    [Description("Produktion Ausgang")] ProductionOutgoing = -10
}