using System;
using System.Data.Odbc;
using System.Globalization;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Formatter;
using IAG.VinX.Zweifel.MySign.Dto;

namespace IAG.VinX.Zweifel.MySign;

public class VinXConnector : BaseSybaseRepository
{
    private const string ArticleCteFilter =
        @"  Art (LArt_Id, Bez, rownum, Qty) AS 
                (
                SELECT Art_Id, Art_Bezeichnung, 1, Lagbest_Bestand - Lagbest_Reserviert - Lagbest_Rueckstellung
                                       FROM Artikel
                                       JOIN Lagerbestand ON Lagbest_LagerId in (1000) AND Lagbest_ArtikelId = Art_Id AND IsNull(Lagbest_Ort, '') = ''
                                       JOIN Zyklus on Art_ZyklusId = Zyk_ID AND Zyk_ID NOT IN (112)
                                       LEFT OUTER JOIN Produzent on Art_ProdId = Prod_ID
                                       WHERE Art_Artikeltyp = 2 OR IsNull(Prod_ExportShop, 0) = -1 AND Lagbest_MandantID = 1
                ),
                Art_Set (LArt_Id, Bez, rownum, Qty) AS 
                (
                SELECT Art_Id, Art_Bezeichnung, ROW_NUMBER() over (partition by art_id order by Lagbest_Bestand - Lagbest_Reserviert - Lagbest_Rueckstellung), Lagbest_Bestand - Lagbest_Reserviert - Lagbest_Rueckstellung
                                       FROM Vinx.Artikel
                                       JOIN Verkaufsset VSet ON VSet_SetId = Art_Id
                                       JOIN Lagerbestand ON Lagbest_LagerId in (1000) AND Lagbest_ArtikelId = VSet_ArtikelId AND IsNull(Lagbest_Ort, '') = ''
                                       WHERE Art_Artikeltyp = 50 AND Lagbest_MandantID = 1
                                       AND NOT EXISTS (SELECT 1 
                                                        FROM Artikel 
                                                        JOIN Zyklus on Art_ZyklusId = Zyk_ID AND Zyk_ID IN (112) 
                                                        JOIN Verkaufsset ON VSet_SetId = Art_Id
                                                        WHERE VSet_ID = VSet.VSet_Id)
                ),
                Art_All (AArt_Id, Bez, Qty) AS
                (
                SELECT LArt_Id, Bez, Qty FROM Art WHERE Qty > 0
                UNION 
                SELECT LArt_Id, Bez, Qty FROM Art_Set WHERE rownum = 1 and Qty > 0
                ) ";

    #region public

    public VinXConnector(ISybaseConnection sybaseConnection) : base(sybaseConnection)
    {
    }

    public Articles GetArticles()
    {
        var articles = new Articles();
        var sql = "WITH " + ArticleCteFilter + @",
                    SORTENANTEIL (ArtZus_WeinInfoID, ArtZus_Position, Bezeichnung) AS 
                        (
                        SELECT ArtZus_WeinInfoID, ArtZus_Position, 
                        ' ' + CASE ISNULL(ArtZus_Anteil, -1) WHEN -1 THEN '' ELSE TRIM(str(ArtZus_Anteil)) + '% ' END + Sorte_Bezeichnung
                                            FROM ArtikelZusammensetzung
                                            JOIN Traubensorte ON ArtZus_TraubensorteID = Sorte_ID 
                        ),
                        ArtikelSorten (AS_ID, AS_Sorten) AS 
                        (
                        SELECT Art_ID, TRIM(LIST(Bezeichnung ORDER BY ArtZus_Position))
                                            FROM SORTENANTEIL
                                            JOIN WeinInfo ON ArtZus_WeinInfoID = Wein_ID
                                            JOIN Artikel ON Art_WeinInfoID = Wein_ID 
                                            GROUP BY Art_ID          
                                            ORDER BY Art_ID
                            )
                    SELECT DISTINCT
                        Art_Id, trim(str(Art_Artikelnummer, 20)), trim(str(Art_Artikelnummer, 20)), isnull(10 * Abf_InhaltInCl, 0), 
                        Abf_Kuerzel, IsNull(Convert(Varchar, Art_Jahrgang), ''), isnull(Art_Gewichtsanteil, 0), Art_Bezeichnung, 
                        isnull(Klass_Bezeichnung, isnull(Wein_Appelation, ''), ''), isnull(Prod_Bezeichnung, ''), isnull(Reg_Bezeichnung, ''), isnull(AS_Sorten, isnull(Wein_SageTraubensorte, ''), ''),
                        isnull(Wein_Terroir, ''), isnull(Wein_Vinifikation, ''), isnull(Wein_Charakter, ''), isnull(Wein_Konsumhinweis, ''), 
                        isnull(Wein_Bewertung, ''), isnull(Land_Bezeichnung, ''), isnull(Reg_Bezeichnung, ''), isnull(Prod_Bezeichnung, ''), 
                        isnull(ArtKat_Bezeichnung, ''), isnull(ABS(Wein_Bio), 0), isnull(ABS(Wein_Vegan), 0)
                    FROM Artikel
                    JOIN Art_All ON Art_Id = AArt_Id
                    JOIN Abfuellung ON Art_AbfId = Abf_Id
                    LEFT OUTER JOIN Land ON Art_landId = Land_Id
                    LEFT OUTER JOIN Region ON Reg_ID = Art_RegId
                    LEFT OUTER JOIN Produzent on Art_ProdId = Prod_ID
                    LEFT OUTER JOIN WeinInfo on Art_WeininfoId = Wein_Id
                    LEFT OUTER JOIN ArtikelSorten ON AS_ID = Art_ID
                    LEFT OUTER JOIN Klassifikation ON Wein_KlassifikationID = Klass_ID
                    LEFT OUTER JOIN Artikelkategorie on Art_AKatID = ArtKat_Id ORDER BY Art_Id";
        using var cmd = SybaseConnection.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            articles.Items.Add(new Article
            {
                artvarnum = Convert.ToUInt32(reader[0]),
                artnum = reader[1].ToString(),
                matchcode = reader[2].ToString(),
                status = 2,
                bottlesize = Convert.ToUInt16(reader[3]),
                packagetyp = reader[4].ToString(),
                jahrgang = reader[5].ToString(),
                gewicht = Convert.ToDecimal(reader[6]),
                bezeichnung1d = reader[7].ToString(),
                bezeichnung2d = RtfCleaner.Clean(reader[8].ToString()),
                bezeichnung3d = RtfCleaner.Clean(reader[9].ToString()),
                landregion = RtfCleaner.Clean(reader[10].ToString()),
                traubensorteproz = RtfCleaner.Clean(reader[11].ToString()),
                herkunft = RtfCleaner.Clean(reader[12].ToString()),
                vinifikation = RtfCleaner.Clean(reader[13].ToString()),
                charakteristik = RtfCleaner.Clean(reader[14].ToString()),
                passendzu = RtfCleaner.Clean(reader[15].ToString()),
                praemierungen = RtfCleaner.Clean(reader[16].ToString()),
                menustufe1 = reader[20].ToString(),
                menustufe2 = reader[17].ToString(),
                menustufe3 = reader[18].ToString(),
                menustufe4 = reader[19].ToString(),
                menustufe5 = string.Empty,
                symbio = (byte)Convert.ToUInt16(reader[21]) == 0 ? byte.MinValue : byte.MaxValue,
                symvegan = (byte)Convert.ToUInt16(reader[22]) == 0 ? byte.MinValue : byte.MaxValue
            });
        }

        return articles;
    }

    public ArticleDescs GetArticleDescs()
    {
        var articleDescs = new ArticleDescs();
        var sql = "WITH " + ArticleCteFilter + @"
                    SELECT DISTINCT
                        Art_Id, Prod_Text
                    FROM Artikel
                    JOIN Art_All ON Art_Id = AArt_Id
                    JOIN Produzent on Art_ProdId = Prod_ID ORDER BY Art_Id";
        using var cmd = SybaseConnection.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var desc = new ArticleDesc
            {
                artvarnum = Convert.ToUInt32(reader[0]),
                text = reader[1].ToString(),
            };
            articleDescs.Items.Add(desc);
        }

        return articleDescs;
    }

    public Stocks GetStock()
    {
        var stocks = new Stocks();
        var sql = "WITH " + ArticleCteFilter + @"
                    SELECT
                        Art_Id, 1000, 'Shop Höngg', 'H', Sum(Qty)
                    FROM Artikel                    
                    JOIN Art_All ON Art_Id = AArt_Id
                    GROUP BY Art_Id
                    ORDER BY Art_Id";
        using var cmd = SybaseConnection.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var desc = new Stock
            {
                artvarnum = Convert.ToUInt32(reader[0]),
                lagernum = Convert.ToInt32(reader[1]),
                lagername = reader[2].ToString(),
                lagertyp = reader[3].ToString(),
                menge = Convert.ToInt32(reader[4])
            };
            stocks.Items.Add(desc);
        }

        return stocks;
    }

    public Prices GetPrice()
    {
        var prices = new Prices();
        var sql = "WITH " + ArticleCteFilter + @"
                    ,
                    prices(Art_Id, VonDatum, BisDatum, Preis, MWSt, Rownum, IsNetto) AS 
                    (
                        SELECT 
                            Art_Id, Isnull(VK_VonDatum, CURRENT DATE), Isnull(VK_BisDatum, CURRENT DATE + 300), VK_Preis, 
                            MWST_Prozent,
                            ROW_NUMBER() over (partition by art_id order by Isnull(VK_VonDatum, CURRENT DATE) desc) as Rownum,
                            IF ABS(VK_KundenrabattErlaubt) = 0 AND ABS(VK_MengenrabattErlaubt) = 0 AND ABS(VK_SkontoErlaubt) = 1 THEN 1 ELSE 0 ENDIF
                        FROM Artikel                    
                        JOIN Art_All ON Art_Id = AArt_Id
                        JOIN VKPreis ON art_id = vk_ArtId 
                                     AND Isnull(VK_VonDatum, CURRENT DATE) <= CURRENT DATE and Isnull(VK_BisDatum, CURRENT DATE) >= CURRENT DATE 
                                     AND VK_KGrpPreisID = 1      
                        JOIN MWST on Art_MwstId = MWST_Id
                        UNION
                        SELECT 
                            Art_Id, Isnull(VK_DatumVon, CURRENT DATE), Isnull(VK_DatumBis, CURRENT DATE + 300), VK_PreisAktion, 
                            MWST_Prozent,
                            ROW_NUMBER() over (partition by art_id order by Isnull(VK_DatumVon, CURRENT DATE) desc) as Rownum,
                            IF ABS(VK_KundenrabattErlaubt) = 0 AND ABS(VK_MengenrabattErlaubt) = 0 AND ABS(VK_SkontoErlaubt) = 1 THEN 1 ELSE 0 ENDIF
                        FROM Artikel                    
                        JOIN Art_All ON Art_Id = AArt_Id
                        JOIN VKPreis ON art_id = vk_ArtId 
                                     AND Isnull(VK_DatumVon, CURRENT DATE) <= CURRENT DATE and Isnull(VK_DatumBis, CURRENT DATE) >= CURRENT DATE 
                                     AND VK_KGrpPreisID = 1 and VK_PreisAktion is not null     
                        JOIN MWST on Art_MwstId = MWST_Id
                    )
                    SELECT * FROM prices
                    WHERE Rownum = 1
                    ORDER BY Art_Id";
        using var cmd = SybaseConnection.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var desc = new Price
            {
                artvarnum = Convert.ToUInt32(reader[0]),
                gultvon = Convert.ToDateTime(reader[1]).ToString("yyyy-MM-dd"),
                gultbis = Convert.ToDateTime(reader[2]).ToString("yyyy-MM-dd"),
                prs = Convert.ToDecimal(reader[3]).ToString("######.00"),
                satz = Convert.ToDecimal(reader[4]).ToString("##.00"),
                artgrp = Convert.ToInt16(reader[6]) == 1 ? "netto" : "brutto"
            };
            prices.Items.Add(desc);
        }

        return prices;
    }

    public Customers GetCustomers()
    {
        var customers = new Customers();
        var sql = @"
                    SELECT
                        Adr_id, Adr_Adressnummer,  IsNull(Anr_Anrede, ''), Adr_Name,  
                        IsNull(Adr_Vorname, ''), IsNull(Adr_Zusatz1, ''), IsNull(Adr_Strasse, ''), IsNull(Adr_Zusatz2, ''), 
                        IsNull(Land_Code, 'CH'), IsNull(Adr_PLZ, ''), IsNull(Adr_Ort, ''), IsNull(Adr_TelG, ''), 
                        IsNull(Adr_TelD, ''), IsNull(Adr_Natel, ''), IsNull(Adr_EMail, ''), IsNull(Adr_ShopId, 0), 
                        Case Adr_Aktiv when -1 then 'N' else 'J' end
                    FROM Adresse
                    LEFT OUTER JOIN Anrede on Adr_AnrID = Anr_Id
                    LEFT OUTER JOIN Land on Land_Id = Adr_LandId
                    WHERE Adr_KGrpPreisID = 1 AND Adr_Kunde = -1 and Adr_Adressnummer is not null and Adr_Name is not null";
        using var cmd = SybaseConnection.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var desc = new Customer
            {
                adrnbr = reader[0].ToString(),
                debinbr = Convert.ToDecimal(reader[1]).ToString("#######.#"),
                anrede = reader[2].ToString(),
                namefirma = reader[3].ToString(),
                vorname = reader[4].ToString(),
                adresse1 = reader[5].ToString(),
                adresse2 = reader[6].ToString(),
                postfach = reader[7].ToString(),
                landcode = reader[8].ToString(),
                plz = reader[9].ToString(),
                ort = reader[10].ToString(),
                telefon1 = reader[11].ToString(),
                telefon2 = reader[12].ToString(),
                mobil = reader[13].ToString(),
                email1 = reader[14].ToString(),
                idshop = Convert.ToUInt32(reader[15]),
                idshopadr = Convert.ToUInt32(reader[15]),
                sperrcode = reader[16].ToString(),
                b2b = "b2c"
            };
            customers.Items.Add(desc);
        }

        return customers;
    }

    public void SetShopId(Customer customer)
    {
        using var cmd = SybaseConnection.CreateCommand("UPDATE Adresse SET Adr_ShopId = ? WHERE Adr_Id = ?",
            customer.idshop.ToString(),
            customer.adrnbr);
        cmd.ExecuteNonQuery();
    }

    public void UpdateCustomer(Customer customer)
    {
        UpdateCustomer(customer, false);
    }

    public void InsertCustomer(Customer customer)
    {
        UpdateCustomer(customer, true);
    }

    /// <summary>
    /// Insert order
    /// </summary>
    /// <param name="order"></param>
    /// <returns>true if ok, false if duplicate id</returns>
    public bool InsertOrder(Order order)
    {
        BeginTransaction();
        try
        {
            var sql = @"INSERT INTO OnlineBestellung (
                    Best_Datum, Best_Lieferdatum, Best_ExterneID, Best_AdrID, Best_LieferAdresseID, Best_InklMWST, Best_TotalBetrag, Best_Hinweis,
                    Best_ZahlungskonditionID, Best_ZahlungswegKarteID)
                    SELECT ?, ?, ?, Min(Adr_Id), ?, ?, ?, ?, ?, ?
                    FROM Adresse WHERE Adr_ShopId = ?;
                    SELECT @@identity;";
            using (var cmd = SybaseConnection.CreateCommand(sql))
            {
                var orderDate = DateTime.ParseExact(order.Bestelldatum, "yyyyMMdd", CultureInfo.InvariantCulture);
                cmd.AddParameter(orderDate);
                cmd.AddParameter(string.IsNullOrWhiteSpace(order.Lieferdatum) ? orderDate : DateTime.ParseExact(order.Lieferdatum, "yyyyMMdd", CultureInfo.InvariantCulture));
                cmd.AddParameter(order.ShopBelegnr.ToString());
                cmd.AddParameter((int)order.liefadr);
                cmd.AddParameter(-1);
                cmd.AddParameter(order.saldoBeleg);
                cmd.AddParameter($"Belegart {order.Belegtyp}, Kondition {order.Kondition}, Betrag: {order.saldoBeleg:n2}");
                cmd.AddParameter(order.PamenyConditionRef);
                cmd.AddParameter(order.PamenyMethodRef);
                cmd.AddParameter(order.Kundennumer); 
                var orderId = cmd.ExecuteScalar();
                cmd.CommandText = @"
                        INSERT INTO OnlineBestellPosition (
                            BestPos_BestID, BestPos_ArtikelID, BestPos_Bezeichnung, BestPos_Anzahl, BestPos_Preis, BestPos_Total, 
                            BestPos_PreisErmittlung, BestPos_MWSTProzentInPreis)
                        SELECT ?, Art_Id, Art_Bezeichnung, ?, ?, ?, 55, MWST_Prozent
                        FROM Artikel 
                        JOIN MWST on Art_MwstId = MWST_Id 
                        WHERE Art_SageArtikelcode = ?";
                foreach (var orderPos in order.OrderPos)
                {
                    cmd.Parameters.Clear();
                    object price = DBNull.Value;
                    object total = DBNull.Value;
                    if (!string.IsNullOrWhiteSpace(orderPos.Preis))
                    {
                        price = Convert.ToDecimal(orderPos.Preis);
                        total = (decimal) price * orderPos.BestellMenge;
                    }
                    cmd.AddParameter(orderId);
                    cmd.AddParameter(orderPos.BestellMenge);
                    cmd.AddParameter(price);
                    cmd.AddParameter(total);
                    cmd.AddParameter(orderPos.Artikelnummer);
                    cmd.ExecuteNonQuery();
                }
            }
            Commit();
            return true;
        }
        catch (OdbcException ex)
        {
            Rollback();
            if (ex.Errors.Count > 0 && ex.Errors[0].NativeError == -196)
                return false;
            throw;
        }
    }

    #endregion

    #region private

    private void UpdateCustomer(Customer customer, bool isNew)
    {
        var sql = @"
                    INSERT INTO OnlineAdresse (
                        Adr_EintragTyp, Adr_AdresseID, Adr_Name, Adr_Vorname, Adr_Zusatz1, Adr_Strasse, Adr_Zusatz2, Adr_PLZ, Adr_Ort, Adr_TelG, Adr_TelD, 
                        Adr_Natel, Adr_EMail, Adr_ShopId, Adr_Sprache, Adr_LandId, Adr_AnrID)
                    SELECT ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, Land_ID, Anr_Id
                    FROM Dummy 
                    LEFT OUTER JOIN Land ON Land_Code = ?
                    LEFT OUTER JOIN Anrede ON Anr_Anrede = ?;";
        using var cmd = SybaseConnection.CreateCommand(sql);
        if (isNew)
        {
            cmd.AddParameter(1);
            cmd.AddParameter(DBNull.Value);
        }
        else
        {
            cmd.AddParameter(6);
            cmd.AddParameter(customer.adrnbr);
        }

        cmd.AddParameter(customer.namefirma);
        cmd.AddParameter(customer.vorname);
        cmd.AddParameter(customer.adresse1);
        cmd.AddParameter(customer.adresse2);
        cmd.AddParameter(customer.postfach);
        cmd.AddParameter(customer.plz);
        cmd.AddParameter(customer.ort);
        cmd.AddParameter(customer.telefon1);
        cmd.AddParameter(customer.telefon2);
        cmd.AddParameter(customer.mobil);
        cmd.AddParameter(customer.email1);
        cmd.AddParameter(customer.idshop.ToString());
        cmd.AddParameter("DE");
        cmd.AddParameter(customer.landcode);
        cmd.AddParameter(customer.anrede);
        cmd.ExecuteNonQuery();
    }

    #endregion
}