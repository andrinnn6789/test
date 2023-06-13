using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Logging;
using IAG.VinX.Schüwo.SV.Dto;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class OrderImporter
{
    private readonly ISybaseConnection _connection;
    private readonly ResultCounts _resultCounts;
    private readonly int? _providerId;
    private readonly Dictionary<int, ArticleSw> _vxArticles;
    private readonly UnitMapper _unitMapper;
    private readonly IMessageLogger _msgLogger;

    public OrderImporter(ISybaseConnection connection, IMessageLogger msgLogger, ResultCounts resultCounts, int? providerId)
    {
        _msgLogger = msgLogger;
        _connection = connection;
        _resultCounts = resultCounts;
        _providerId = providerId;
        var vxArticles = _connection.GetQueryable<ArticleSw>().ToList();
        _vxArticles = new Dictionary<int, ArticleSw>();
        vxArticles.ForEach(art => _vxArticles.Add(art.ArtNr, art));
        _unitMapper = new UnitMapper(msgLogger, resultCounts);
    }

    public void Import(Stream orderStream)
    {
        orderStream.Position = 0;
        var orderLines = new StreamReader(orderStream).ReadToEnd().Split("\n");
        var externalId = string.Empty;
        var delDate = DateTime.Today;
        var adrNumber = 0;
        var comment = string.Empty;
        var posLines = new List<string>();
        var startPosLines = false;
        List<string> posHeader = null;
        foreach (var orderLineRaw in orderLines)
        {
            var orderLine = orderLineRaw.TrimEnd('\r');
            var lineData = orderLine.Split(';');
            var lineId = lineData[0];
            if (lineData.Length == 1)
                continue;
            switch (lineId)
            {
                case "_gpoid":
                    externalId = orderLine.Split(';')[1];
                    break;
                case "_cid":
                    adrNumber = Convert.ToInt32(orderLine.Split(';')[1]);
                    break;
                case "delivery_date":
                    delDate = Convert.ToDateTime(orderLine.Split(';')[1]);
                    break;
                case "comment":
                    comment = orderLine.Split(';')[1];
                    break;
                case "lnbrr":
                    startPosLines = true;
                    posHeader = new List<string>(lineData);
                    break;
                default:
                    if (startPosLines)
                        posLines.Add(orderLine);
                    break;
            }
        }

        _connection.BeginTransaction();
        using var cmd = _connection.CreateCommand(string.Empty);
        try
        {
            var orderId = InsertOrderHead(cmd, adrNumber, externalId, delDate, comment);
            InsertOrderPos(cmd, orderId, posHeader, posLines);
            _connection.Commit();
            _resultCounts.SuccessCount++;
        }
        catch (Exception ex)
        {
            _connection.Rollback();
            _msgLogger.AddMessage(ex);
            _resultCounts.WarningCount++;
            var orderText = $"Der Auftrag konnte nicht eingelesen werden {Environment.NewLine}({ex.Message})";
            InsertOrderHead(cmd, null, externalId, delDate, orderText);
        }
    }

    private int InsertOrderHead(ISybaseCommand cmd, int? adrNumber, string externalId, DateTime deliveryDate, string comment)
    {
        var sql = @"INSERT INTO OnlineBestellung (
                    Best_Datum, Best_ExterneID, Best_AdrID, Best_Lieferdatum, Best_Hinweis, Best_ProviderID)
                    SELECT ?, ?, Adr_Id, ?, ?, ? FROM Adresse WHERE Adr_Adressnummer = ?;
                    SELECT @@identity;";
        cmd.CommandText = sql;
        cmd.AddParameter(DateTime.Now);
        cmd.AddParameter(externalId);
        cmd.AddParameter(deliveryDate);
        cmd.AddParameter(comment);
        cmd.AddParameter(_providerId);
        cmd.AddParameter(adrNumber);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private void InsertOrderPos(ISybaseCommand cmd, int orderId, List<string> posHeader, List<string> posLines)
    {
        cmd.CommandText = @"
                        INSERT INTO OnlineBestellPosition (
                            BestPos_BestID, BestPos_ArtikelID, BestPos_Bezeichnung, BestPos_Anzahl
                            )
                        SELECT ?, Art_Id, Art_Bezeichnung, ?
                        FROM Artikel 
                        WHERE Art_ArtikelNummer = ?";

        foreach (var posLine in posLines)
        {
            cmd.Parameters.Clear();
            var lineData = posLine.Split(';');
            var unit = lineData[posHeader.IndexOf("unit")];
            var quantity = Convert.ToInt32(lineData[posHeader.IndexOf("quant")]);
            var artNr = Convert.ToInt32(lineData[posHeader.IndexOf("anbr")]);
            quantity = UnitFactor(artNr, unit) * quantity;

            cmd.AddParameter(orderId);
            cmd.AddParameter(quantity);
            cmd.AddParameter(artNr);
            cmd.ExecuteNonQuery();
        }
    }

    private int UnitFactor(int artNr, string unit)
    {
        if (!_vxArticles.ContainsKey(artNr)) 
            throw new LocalizableException(Resource.ResourceIds.SyncErrorUnknownArticle, artNr);

        var art = _vxArticles[artNr];
        if (_unitMapper.MapUnit(art.FillingTextShort) != unit || art.IsTank)
            return art.UnitsPerBulkPackaging;
        return 1;
    }
}