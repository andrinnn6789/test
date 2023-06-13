using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

using FileHelpers;

using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.Dto;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.Dto;
using IAG.VinX.CDV.Wamas.StockReportImport.Dto;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public static class WamasSerializationHelper
{
    public static byte[] SerializeAsCsv(List<GenericWamasRecord> records, params Type[] recordTypes)
    {
        var engine = new MultiRecordEngine(recordTypes);
        engine.Encoding = Encoding.Unicode;

        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        engine.BeginWriteStream(streamWriter);

        var recordNumber = 1;
        foreach (var record in records)
        {
            var castedRecord = CastHelper.Cast(record.RecordValue, record.RecordType);
            ((IWamasRecord)castedRecord).SerialNumber = recordNumber;
            engine.WriteNext(castedRecord);
            recordNumber++;
        }

        engine.Close();
        var data = stream.ToArray();

        return data;
    }

    public static List<GenericWamasRecord> DeserializeFromCsv(byte[] data, Tuple<Type, string> recordMapping)
    {
        var recordType = recordMapping.Item1;
        var engine = new FileHelperEngine(recordType)
        {
            Encoding = Encoding.UTF8
        };
        var genericWamasRecords = new List<GenericWamasRecord>();

        using var stream = new MemoryStream(data);
        using var streamReader = new StreamReader(stream);
        var records = engine.ReadStream(streamReader).ToList();

        foreach (var record in records)
        {
            var wamasRecord = (IWamasRecord)record;
            var genericWamasRecord = new GenericWamasRecord(recordType, wamasRecord);
            genericWamasRecords.Add(genericWamasRecord);
        }

        return genericWamasRecords;
    }

    public static List<GenericWamasRecord> DeserializeFromCsv(byte[] data, params Tuple<Type, string>[] recordMappings)
    {
        var recordTypes = recordMappings.Select(mapping => mapping.Item1).ToArray();
        var engine = new MultiRecordEngine(recordTypes);
        engine.Encoding = Encoding.UTF8;
        engine.RecordSelector = CustomRecordSelector;
        var genericWamasRecords = new List<GenericWamasRecord>();

        using var stream = new MemoryStream(data);
        using var streamReader = new StreamReader(stream);
        var records = engine.ReadStream(streamReader).ToList();

        foreach (var record in records)
        {
            var wamasRecord = (IWamasRecord)record;
            var wamasRecordType = recordMappings
                .FirstOrDefault(mapping => mapping.Item2 == wamasRecord.DatasetType)?.Item1;

            var genericWamasRecord = new GenericWamasRecord(wamasRecordType, wamasRecord);
            genericWamasRecords.Add(genericWamasRecord);
        }

        engine.Close();

        return genericWamasRecords;
    }

    [ExcludeFromCodeCoverage]
    private static Type CustomRecordSelector(MultiRecordEngine engine, string recordLine)
    {
        var columns = recordLine.Split(";");
        var datasetType = columns[4];

        return datasetType switch
        {
            ResourceIds.WamasGoodsReceiptRecordType => typeof(GoodsReceipt),
            ResourceIds.WamasGoodsReceiptLineRecordType => typeof(GoodsReceiptLine),
            ResourceIds.WamasGoodsReceiptLineDifferenceRecordType => typeof(GoodsReceiptLineDifference),
            ResourceIds.WamasGoodsIssueRecordType => typeof(GoodsIssue),
            ResourceIds.WamasGoodsIssueLineRecordType => typeof(GoodsIssueLine),
            ResourceIds.WamasGoodsIssueLineDifferenceRecordType => typeof(GoodsIssueLineDifference),
            ResourceIds.WamasStockReportBeginRecordType => typeof(StockReportBegin),
            ResourceIds.WamasStockReportAcknowledgeRecordType => typeof(StockReportAcknowledge),
            ResourceIds.WamasStockReportEndRecordType => typeof(StockReportEnd),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}