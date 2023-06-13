using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Zweifel.S1M.BusinessLogic;
using IAG.VinX.Zweifel.S1M.Dto.S1M;

using Moq;

using Newtonsoft.Json;

using Xunit;


namespace IAG.VinX.Zweifel.IntegrationTest.S1M.BusinessLogic;

public class S1MDeliveryComposerTest
{
    [Fact]
    public void ComposeDelivery()
    {
        #region Testdata

        var deliveryList = new List<S1MDelivery>
        {
            new()
            {
                DeliveryId = 1,
                TourNumber = "1",
                TourName = "1",
                VehicleId = 1,
                DriverUref = "FRANZ",
                Status = DeliveryStatus.Fixed,
                DeliveryDate = DateTime.Today,
                StartKms = 1,
                EndKms = 2,
                StartTime = TimeSpan.MinValue,
                EndTime = TimeSpan.MinValue
            }
        };

        var vehiclesList = new List<Vehicle>
        {
            new()
            {
                Id = 1,
                VehicleName = "Mercedes-Benz",
                VehiclePlate = "Sprinter"
            }
        };

        var haltsList = new List<S1MHalt>
        {
            new()
            {
                StationId = 1,
                DeliveryId = 1,
                AddressId = 1,
                DocumentId = 1,
                HaltPosition = 1
            }
        };

        var addressList = new List<S1MAddress>()
        {
            new()
            {
                Id = 1,
                StationId = 1,
                Number = 1,
                Name = "Franz",
                Street = "Street",
                City = "City",
                Zip = "Zip",
                EMail = "Email",
                DeliveryTimeFrom1 = default,
                DeliveryTimeTo1 = default,
                DeliveryTimeFrom2 = default,
                DeliveryTimeTo2 = default,
                DeliveryTimeFrom3 = default,
                DeliveryTimeTo3 = default,
                NotifyByTelephone = false,
                KeyBadgeCode = "123",
                DeliverySlipWithPrice = false,
                ContactPersonId = 1
            }
        };

        var contactPersonList = new List<ContactPerson>
        {
            new()
            {
                Id = 1,
                FirstName = "Contact",
                LastName = "Person",
                Telephone = "Telephone",
                Mobile = "Mobile",
                Email = "Email",
                Function = "Function",
                Remarks = "Remarks",
                AddressId = 1,
                DepartmentId = 1
            }
        };

        var documentList = new List<S1MDocument>()
        {
            new()
            {
                Id = 1,
                Number = 1,
                IRef = "FRANZ",
                RemarkRtf = "Remark",
                Type = 1,
            }
        };

        var artPosList = new List<S1MArticlePosition>()
        {
            new()
            {
                Id = 1,
                DocumentId = 1,
                PositionOnDocument = 1,
                ArticleId = 1,
                ArticleNumber = 1,
                ArticleName = @"Erste Zeile
Zweite Zeile
Dritte Zeile",
                ArticleFilling = "Filling",
                ArticleGgName = "GGName",
                ArticleGgBreakable = false,
                ArticleQuantity = 1,
                ArticleQuantityGg = 1,
                ArticlePrice = 12.6M
            }
        };

        var bulkPkgPosList = new List<S1MBulkPackagePosition>()
        {
            new()
            {
                Id = 1,
                DocumentId = 1,
                ArticleNumber = 1,
                ArticleDescription = "Description",
                QuantityDelivered = 1,
                QuantityReturned = 1
            }
        };

        var composedDeliveryList = new List<S1MExtDelivery>
        {
            new()
            {
                DeliveryId = 1,
                TourNumber = "1",
                TourName = "1",
                Vehicle = vehiclesList.FirstOrDefault(),
                DriverUref = "FRANZ",
                Status = DeliveryStatus.Fixed,
                DeliveryDate = DateTime.Today,
                StartKms = 1,
                EndKms = 2,
                StartTime = TimeSpan.MinValue,
                EndTime = TimeSpan.MinValue,
                Halts = new List<S1MHalt>
                {
                    new()
                    {
                        StationId = 1,
                        DeliveryId = 1,
                        AddressId = 1,
                        DocumentId = 1,
                        HaltPosition = 1,
                        S1MAddress = new S1MAddress
                        {
                            Id = 1,
                            StationId = 1,
                            Number = 1,
                            Name = "Franz",
                            Street = "Street",
                            City = "City",
                            Zip = "Zip",
                            EMail = "Email",
                            DeliveryTimeFrom1 = default,
                            DeliveryTimeTo1 = default,
                            DeliveryTimeFrom2 = default,
                            DeliveryTimeTo2 = default,
                            DeliveryTimeFrom3 = default,
                            DeliveryTimeTo3 = default,
                            NotifyByTelephone = false,
                            KeyBadgeCode = "123",
                            DeliverySlipWithPrice = false,
                            ContactPersons = new List<ContactPerson>
                            {
                                new()
                                {
                                    Id = 1,
                                    AddressId = 1,
                                    DepartmentId = 1,
                                    FirstName = "Contact",
                                    LastName = "Person",
                                    Telephone = "Telephone",
                                    Mobile = "Mobile",
                                    Email = "Email",
                                    Function = "Function",
                                    Remarks = "Remarks"
                                }
                            }
                        },
                        S1MDocument = new S1MDocument
                        {
                            Id = 1,
                            Number = 1,
                            IRef = "FRANZ",
                            RemarkRtf = "Remark",
                            Type = 1,
                            ArticlePositions = new List<S1MArticlePosition>
                            {
                                new()
                                {
                                    Id = 1,
                                    DocumentId = 1,
                                    PositionOnDocument = 1,
                                    ArticleId = 1,
                                    ArticleNumber = 1,
                                    ArticleName = @"Erste Zeile
Zweite Zeile
Dritte Zeile",
                                    ArticleFilling = "Filling",
                                    ArticleGgName = "GGName",
                                    ArticleGgBreakable = false,
                                    ArticleQuantity = 1,
                                    ArticleQuantityGg = 1,
                                    ArticlePrice = 12.6M
                                }
                            },
                            BulkPackagePositions = new List<S1MBulkPackagePosition>
                            {
                                new()
                                {
                                    Id = 1,
                                    DocumentId = 1,
                                    ArticleNumber = 1,
                                    ArticleDescription = "Description",
                                    QuantityDelivered = 1,
                                    QuantityReturned = 1
                                }
                            }
                        }
                    }
                }
            }
        };

        #endregion

        var fakeConnection = new Mock<ISybaseConnection>();
        fakeConnection.Setup(fc => fc.GetQueryable<S1MDelivery>()).Returns(deliveryList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<Vehicle>()).Returns(vehiclesList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<S1MHalt>()).Returns(haltsList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<S1MAddress>()).Returns(addressList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<ContactPerson>()).Returns(contactPersonList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<S1MDocument>()).Returns(documentList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<S1MArticlePosition>()).Returns(artPosList.AsQueryable());
        fakeConnection.Setup(fc => fc.GetQueryable<S1MBulkPackagePosition>()).Returns(bulkPkgPosList.AsQueryable());

        var composer = new S1MDeliveryComposer();
        composer.SetConfig(fakeConnection.Object);

        Assert.Equal(JsonConvert.SerializeObject(composedDeliveryList),
            JsonConvert.SerializeObject(composer.ComposeDeliveries(deliveryList)));
        Assert.Equal("Erste Zeile",
            composedDeliveryList.First().Halts.First().S1MDocument.ArticlePositions.First().ArticleName1);
        Assert.Equal("Zweite Zeile",
            composedDeliveryList.First().Halts.First().S1MDocument.ArticlePositions.First().ArticleName2);
        Assert.Equal("Dritte Zeile",
            composedDeliveryList.First().Halts.First().S1MDocument.ArticlePositions.First().ArticleName3);
    }
}