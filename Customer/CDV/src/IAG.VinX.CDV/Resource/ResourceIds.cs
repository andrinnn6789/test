namespace IAG.VinX.CDV.Resource;

public static class ResourceIds
{
    private const string ResourcePrefix = "CDV.Job.";
    
    // Wamas
    private const string WamasPrefix = ResourcePrefix + "Wamas.";
        
    internal const string WamasPartnerExportJobName = WamasPrefix + "PartnerExportJob";
    internal const string WamasArticleExportJobName = WamasPrefix + "ArticleExportJob";
    internal const string WamasPurchaseOrderExportJobName = WamasPrefix + "PurchaseOrderExportJob";
    internal const string WamasGoodsReceiptImportJobName = WamasPrefix + "GoodsReceiptImportJob";
    internal const string WamasPickListExportJobName = WamasPrefix + "PickListExportJob";
    internal const string WamasGoodsIssueImportJobName = WamasPrefix + "GoodsIssueImportJob";
    internal const string WamasStockAdjustmentImportJobName = WamasPrefix + "StockAdjustmentImportJob";
    internal const string WamasStockReportImportJobName = WamasPrefix + "StockReportImportJob";

    internal const string WamasLogError = WamasPrefix + "Fehler beim Schreiben des Fehlerprotokolls an VinX, {0}";
    internal const string WamasPartnerExportError = WamasPrefix + "Fehler beim Exportieren von Partnern & Adressen zu WAMAS, {0}";
    internal const string WamasArticleExportError = WamasPrefix + "Fehler beim Exportieren von Artikeln zu WAMAS, {0}";
    internal const string WamasPurchaseOrderExportError = WamasPrefix + "Fehler beim Exportieren von Bestellungen zu WAMAS, {0}";
    internal const string WamasPurchaseOrderConfirmError = WamasPrefix + "Fehler beim Bestaetigen des Logistikstatus {0} nach Export von Bestellung (Beleg-ID {1}) zu WAMAS, {2}";
    internal const string WamasGoodsReceiptsImportError = WamasPrefix + "Fehler beim Importieren von Wareneingaengen aus WAMAS, {0}";
    internal const string WamasGoodsReceiptsImportFileError = WamasPrefix + "Fehler beim Importieren der Wareneingangs-Datei {0} aus WAMAS, {1}";
    internal const string WamasGoodsReceiptImportError = WamasPrefix + "Fehler beim Importieren von Wareneinaengen zu VinX-Bestellung (Beleg-ID {0}) aus der Wareneingangs-Datei {1} aus WAMAS, {2}";
    internal const string WamasPickListExportError = WamasPrefix + "Fehler beim Exportieren von Ruestscheinen zu WAMAS, {0}";
    internal const string WamasPickListConfirmError = WamasPrefix + "Fehler beim Bestaetigen des Logistikstatus {0} nach Export von Ruestschein (Beleg-ID {1}) zu WAMAS, {2}";
    internal const string WamasGoodsIssuesImportError = WamasPrefix + "Fehler beim Importieren von Warenausgaengen aus WAMAS, {0}";
    internal const string WamasGoodsIssuesImportFileError = WamasPrefix + "Fehler beim Importieren der Warenausgangs-Datei {0} aus WAMAS, {1}";
    internal const string WamasGoodsIssueImportError = WamasPrefix + "Fehler beim Importieren von Warenausgaengen zu VinX-Ruestschein (Beleg-ID {0}) aus der Warenausgangs-Datei {1} aus WAMAS, {2}";
    internal const string WamasStockAdjustmentsImportError = WamasPrefix + "Fehler beim Importieren von Bestandskorrekturen aus WAMAS, {0}";
    internal const string WamasStockAdjustmentsImportFileError = WamasPrefix + "Fehler beim Importieren der Bestandskorrektur-Datei {0} aus WAMAS, {1}";
    internal const string WamasStockAdjustmentImportError = WamasPrefix + "Fehler beim Importieren der Bestandskorrektur zu Artikel ({0}) vom {1} aus der Bestandskorrektur-Datei {2} aus WAMAS, {3}";
    internal const string WamasStockReportsImportError = WamasPrefix + "Fehler beim Importieren von Bestandsberichten aus WAMAS, {0}";
    internal const string WamasStockReportsImportFileError = WamasPrefix + "Fehler beim Importieren der Bestandsbericht-Datei {0} aus WAMAS, {1}";
    internal const string WamasStockReportImportError = WamasPrefix + "Fehler beim Importieren des Bestandsberichts aus WAMAS-Request ({0}) vom {1} aus der Bestandsbericht-Datei {2} aus WAMAS, {3}";
        
    internal const string WamasPartnerRecordType = "PARTNER00008";
    internal const string WamasPartnerAddressRecordType = "PARTNERADDR00007";
    internal const string WamasArticleRecordType = "ITEM00011";
    internal const string WamasArticleAliasRecordType = "ITEMALIAS00006";
    internal const string WamasArticleDescriptionRecordType = "ITEMDESC00005";
    internal const string WamasArticleQuantityUnitRecordType = "ITEMQTYUNIT00007";
    internal const string WamasPurchaseOrderRecordType = "IBD00008";
    internal const string WamasPurchaseOrderReferenceRecordType = "IBDEXTREF00006";
    internal const string WamasPurchaseOrderLineRecordType = "IBDL00006";
    internal const string WamasPurchaseOrderPartnerRecordType = "IBDPARTNER00008";
    internal const string WamasGoodsReceiptRecordType = "IBDACK00010";
    internal const string WamasGoodsReceiptLineRecordType = "IBDLACK00006";
    internal const string WamasGoodsReceiptLineDifferenceRecordType = "IBDLDIFFCODEACK00005";
    internal const string WamasPickListRecordType = "OBO00008";
    internal const string WamasPickListReferenceRecordType = "OBOEXTREF00006";
    internal const string WamasPickListLineRecordType = "OBOL00006";
    internal const string WamasPickListPartnerRecordType = "OBOPARTNER00008";
    internal const string WamasPickListTextRecordType = "OBOTEXT00001";
    internal const string WamasGoodsIssueRecordType = "OBOACK00010";
    internal const string WamasGoodsIssueLineRecordType = "OBOLACK00006";
    internal const string WamasGoodsIssueLineDifferenceRecordType = "OBOLDIFFCODEACK00005";
    internal const string WamasStockAdjustmentRecordType = "STOCKADJ00007";
    internal const string WamasStockReportBeginRecordType = "STOCKBEGIN00008";
    internal const string WamasStockReportAcknowledgeRecordType = "STOCKACK00008";
    internal const string WamasStockReportEndRecordType = "STOCKEND00008";
        
    internal const string WamasPartnerExportErrorTitle = "WAMAS: Adress-Export";
    internal const string WamasArticleExportErrorTitle = "WAMAS: Artikel-Export";
    internal const string WamasPurchaseOrderExportErrorTitle = "WAMAS: Bestell-Export";
    internal const string WamasGoodsReceiptImportErrorTitle = "WAMAS: Wareneingang-Import";
    internal const string WamasPickListExportErrorTitle = "WAMAS: Ruestschein-Export";
    internal const string WamasGoodsIssueImportErrorTitle = "WAMAS: Warenausgang-Import";
    internal const string WamasStockAdjustmentImportErrorTitle = "WAMAS: Bestandskorr.-Import";
    internal const string WamasStockReportImportErrorTitle = "WAMAS: Bestandsbericht-Import";
    
    // Gastivo
    private const string GastivoPrefix = ResourcePrefix + "Gastivo.";
        
    internal const string GastivoCustomerExportJobName = GastivoPrefix + "CustomerExportJob";
    internal const string GastivoArticleExportJobName = GastivoPrefix + "ArticleExportJob";
    internal const string GastivoPriceExportJobName = GastivoPrefix + "PricesExportJob";
    internal const string GastivoOrderImportJobName = GastivoPrefix + "OrderImportJob";
    
    internal const string GastivoLogError = GastivoPrefix + "Fehler beim Schreiben des Fehlerprotokolls an VinX, {0}";
    
    internal const string GastivoCustomerExportErrorTitle = "Gastivo: Adress-Export";
    internal const string GastivoArticleExportErrorTitle = "Gastivo: Artikel-Export";
    internal const string GastivoPriceExportErrorTitle = "Gastivo: Preis-Export";
    internal const string GastivoOrderImportErrorTitle = "Gastivo: Bestellungs-Import";
    
    internal const string GastivoCustomerExportError = GastivoPrefix + "Fehler beim Exportieren von Kundenadressen zu Gastivo, {0}";
    internal const string GastivoArticleExportError = GastivoPrefix + "Fehler beim Exportieren von Artikeln zu Gastivo, {0}";
    internal const string GastivoPriceExportError = GastivoPrefix + "Fehler beim Exportieren von Preisen zu Gastivo, {0}";
    
    internal const string GastivoOrderImportError = GastivoPrefix + "Fehler beim Importieren der Bestellungs-Datei {0} aus Gastivo, {1}";
    internal const string GastivoOrdersImportError = GastivoPrefix + "Fehler beim Importieren von Bestellungen aus Gastivo, {0}";
}