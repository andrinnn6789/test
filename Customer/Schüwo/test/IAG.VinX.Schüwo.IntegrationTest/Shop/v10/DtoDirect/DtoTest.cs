using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Schüwo.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.Shop.v10.DtoDirect;

public class DtoTest : IDisposable
{
     private readonly ISybaseConnection _connection;

     public DtoTest()
     {
          _connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
     }

     public void Dispose()
     {
          _connection?.Dispose();
     }

     [Fact]
     public void CharacterSw()
     {
          _ = _connection.GetQueryable<WineCharacterSw>().FirstOrDefault();
     }

     [Fact]
     public void ArtikelSw()
     {
          _ = _connection.GetQueryable<ArticleSw>().FirstOrDefault();
     }

     [Fact]
     public void TradingUnitSw()
     {
          _ = _connection.GetQueryable<TradingUnitSw>().FirstOrDefault();
     }

     [Fact]
     public void ArticelRelationSw()
     {
          _ = _connection.GetQueryable<ArticleRelationSw>().FirstOrDefault();
     }

     [Fact]
     public void CustomerMarketingSw()
     {
          _ = _connection.GetQueryable<CustomerMarketingSw>().FirstOrDefault();
     }

     [Fact]
     public void MarketingCodeSw()
     {
          _ = _connection.GetQueryable<MarketingCodeSw>().FirstOrDefault();
     }

     [Fact]
     public void SelectionCodeSw()
     {
          _ = _connection.GetQueryable<SelectionCodeSw>().FirstOrDefault();
     }

     [Fact]
     public void CountrySw()
     {
          _ = _connection.GetQueryable<CountrySw>().FirstOrDefault();
     }

     [Fact]
     public void EcommerceGroupSw()
     {
          _ = _connection.GetQueryable<EcommerceGroupSw>().FirstOrDefault();
     }

     [Fact]
     public void AddressSw()
     {
          _ = _connection.GetQueryable<AddressSw>().FirstOrDefault();
     }
}