using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.VinX.Basket.BusinessLogic;
using IAG.VinX.Basket.DataAccess;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Interfaces;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.DataAccess;

public class OnlineClientGw<TOnlineAddressGw, TBasketGw, TOnlineOrderGw>: OnlineClient<TOnlineAddressGw, TBasketGw, TOnlineOrderGw> 
    where TOnlineAddressGw: OnlineAddressGw, new()
    where TBasketGw: BasketGw<TOnlineAddressGw>
    where TOnlineOrderGw: OnlineOrderGw, new()
{
    private readonly IBasketDataReader _basketDataReader;

    private class AddressHelper
    {
        public int? Id;
        public TOnlineAddressGw OnlineAddress;
    }

    #region public

    public OnlineClientGw(ISybaseConnection connection, IBasketDataReader basketDataReader): base(connection)
    {
        _basketDataReader = basketDataReader;
    }

    public ContactGw NewContact(ContactGw contact)
    {
        if (contact.Id.HasValue)
            throw new ArgumentException("Contact id not allowed for new records");
            
        CheckContactUsernameUnique(contact);
        Connection.Insert(contact);
        return contact;
    }

    public ContactGw UpdateContact(ContactGw contact)
    {
        if (!contact.Id.HasValue)
            throw new ArgumentException("Missing contact id");

        CheckContactUsernameUnique(contact);
        _ = Connection.GetQueryable<ContactGw>().First(c => c.Id == contact.Id);
        Connection.Update(contact);
        return contact;
    }

    public IEnumerable<ProductDetailGw> GetProductDetail(PriceParameter priceParameter)
    {
        _basketDataReader.SetSqlFilter(ArticleGw.MasterFilter);
        var pricesRuler = new PositionPriceCalculator();
        var basePrices = pricesRuler.GetPriceData(_basketDataReader, priceParameter);
        var stocks = _basketDataReader.Stock;
        var articles = LoadArticles(priceParameter);
        var productDetails = new List<ProductDetailGw>();
        foreach (var basePrice in basePrices)
        {
            if (!basePrice.UnitPrice.HasValue)
                continue;

            var productDetail = new ProductDetailGw
            {
                ArticleId = basePrice.ArticleId,
                PriceGroupId = basePrice.PriceGroupId,
                Stock = stocks.Where(s => s.ArticleId == basePrice.ArticleId).Sum(a => a.QuantityAvailable),
                Price = basePrice.UnitPrice.Value,
                Quantity = basePrice.QuantityOrdered,
                ApplicableTaxRate = basePrice.VatRate,
                TemporairySoldOut = articles.First(a => a.Id == basePrice.ArticleId).TemporairySoldOut
            };
            if (basePrice.PriceKind == VinX.Basket.Enum.PriceCalculationKind.Promotion)
            {
                if (basePrice.SalePrice != null)
                    productDetail.Price = basePrice.SalePrice.Value;
                productDetail.PricePromotion = basePrice.UnitPrice;
            }

            productDetails.Add(productDetail);
        }

        return productDetails;
    }

    #endregion

    #region protected

    protected override void AppendOrderData(TOnlineOrderGw order)
    {
        base.AppendOrderData(order);
        order.OrderingContactId = Basket.OrderingContactId;
        order.CarrierId = Basket.CarrierId;
        order.CrifDescription = Basket.CrifDescription;
        order.CrifCheckDate = Basket.CrifCheckDate;
        var orderText = Basket.OrderText;
        if (!string.IsNullOrWhiteSpace(Basket.DeliveryTime))
            orderText += (string.IsNullOrWhiteSpace(orderText) ? string.Empty : Environment.NewLine) + $"{Basket.DeliveryDateRequested:dd.MM.yyyy} {Basket.DeliveryTime}";
        if (!string.IsNullOrWhiteSpace(Basket.DeliveryLocation))
            orderText += (string.IsNullOrWhiteSpace(orderText) ? string.Empty : Environment.NewLine) + Basket.DeliveryLocation;
        if (!string.IsNullOrWhiteSpace(Basket.DeliveryLocationRemark))
            orderText += (string.IsNullOrWhiteSpace(orderText) ? string.Empty : Environment.NewLine + Environment.NewLine) + Basket.DeliveryLocationRemark;
        if (!string.IsNullOrWhiteSpace(Basket.OrderText) && !string.IsNullOrWhiteSpace(_basketDataReader.BasketAddress.InvoiceText))
            orderText += Environment.NewLine + Environment.NewLine;
        order.OrderText = orderText;
    }

    protected override void HandleAddresses(TOnlineOrderGw order)
    {
        var isB2B = Basket.OrderingAddressId.HasValue &&
                    _basketDataReader.BasketAddress.VatPriceBase == VinX.Basket.Enum.VatPriceBaseType.WithoutVat;
        // ordering address
        var addressHelper = new AddressHelper
        {
            Id = Basket.OrderingAddressId,
            OnlineAddress = Basket.OrderingOnlineAddress
        };
        ResolveAddress(addressHelper, true);
        Basket.OrderingAddressId = addressHelper.Id;

        // delivery address
        addressHelper.Id = Basket.DeliveryAddressId;
        addressHelper.OnlineAddress = Basket.DeliveryOnlineAddress;
        ResolveAddress(addressHelper, isB2B);
        Basket.DeliveryAddressId = addressHelper.Id;

        // billing address
        addressHelper.Id = Basket.BillingAddressId;
        addressHelper.OnlineAddress = Basket.BillingOnlineAddress;
        ResolveAddress(addressHelper, true);
        Basket.BillingAddressId = addressHelper.Id;


        if (!Basket.OrderingAddressId.HasValue && (Basket.DeliveryAddressId.HasValue || Basket.BillingAddressId.HasValue))
            throw new BadRequestException("OrderingAddressId must not be empty with deliveryAddressId or billingAddressId");

        order.OrderingAddressId = Basket.OrderingAddressId;
        order.BillingAddressId = Basket.BillingAddressId;
        order.DeliveryAddressId = Basket.DeliveryAddressId;

        order.OrderingOnlineAddressId = Basket.OrderingOnlineAddress?.Id;
        order.BillingOnlineAddressId = Basket.BillingOnlineAddress?.Id;

        if (isB2B)
        {
            order.DeliveryOnlineAddressId = Basket.DeliveryOnlineAddress?.Id;
        }
        else if (Basket.DeliveryOnlineAddress != null)
        {
            SetAdhoc(order, Basket.DeliveryOnlineAddress);
        }
    }

    #endregion

    #region private

    private List<ArticleHelperGw> LoadArticles(PriceParameter priceParameter)
    {
        var articleQry = Connection.GetQueryable<ArticleHelperGw>();
        if (priceParameter.ArticleParameters is {Count: > 0})
        {
            var articleFilter = priceParameter.ArticleParameters.Select(p => p.ArticleId).ToList();
            articleQry = articleQry.Where(a => articleFilter.Contains(a.Id));
        }

        return articleQry.ToList();
    }

    private void ResolveAddress(AddressHelper address, bool createOnlineAddress)
    {
        if (address.Id.HasValue) 
            return;

        if (!string.IsNullOrWhiteSpace(address.OnlineAddress?.ShopId))
        {
            address.OnlineAddress.Id = FindOnlineAddress(address.OnlineAddress.ShopId);
            if (address.OnlineAddress.Id.HasValue)
                return;

            address.Id = FindVxAddress(address.OnlineAddress.ShopId);
            address.OnlineAddress.AddressId = address.Id;
        }

        if (createOnlineAddress && !address.Id.HasValue && address.OnlineAddress is { Id: null })
            address.OnlineAddress.Id = NewOnlineAddress(address.OnlineAddress).Id;
    }

    private int? FindVxAddress(string shopId)
    {
        return CheckShopId(Connection.GetQueryable<AddresShopId>().Where(a => a.ShopId == shopId).ToList());
    }

    private int? FindOnlineAddress(string shopId)
    {
        return CheckShopId(Connection.GetQueryable<OnlineAddressGw>().Where(a => a.ShopId == shopId).ToList());
    }

    private static int? CheckShopId(IReadOnlyCollection<IShopId> shopIds)
    {
        return shopIds.Count switch
        {
            0 => null,
            1 => shopIds.First().Id,
            _ => throw new DuplicateKeyException($"Shop-Id {shopIds.First().Id} is not unique"),
        };
    }

    private void CheckContactUsernameUnique(ContactGw contact)
    {
        if (string.IsNullOrWhiteSpace(contact.LoginName))
            return;

        var contacts = Connection.GetQueryable<ContactGw>().Where(a => a.LoginName == contact.LoginName && a.Id != contact.Id).ToList();
        if (contacts.Count > 0)
            throw new DuplicateKeyException($"Loginname {contact.LoginName} is not unique");
    }

    private static void SetAdhoc(TOnlineOrderGw order, OnlineAddressGw delAddress)
    {
        order.AdHocAddress = (delAddress.FirstName + " " + delAddress.LastName).Trim();
        if (!string.IsNullOrWhiteSpace(delAddress.AdditionalAddressLine1))
            order.AdHocAddress += string.IsNullOrWhiteSpace(order.AdHocAddress) ? string.Empty : Environment.NewLine + delAddress.AdditionalAddressLine1;
        if (!string.IsNullOrWhiteSpace(delAddress.ContactFirstName + " " + delAddress.ContactLastName))
            order.AdHocAddress += string.IsNullOrWhiteSpace(order.AdHocAddress) ? string.Empty : Environment.NewLine + delAddress.ContactFirstName + " " + delAddress.ContactLastName;

        order.AdHocCountry = delAddress.Country;
        order.AdHocCity = delAddress.City;
        order.AdHocStreet = delAddress.Street;
        order.AdHocPhoneNumber = delAddress.PhoneMobile;
        order.AdHocZip = delAddress.Zipcode;
        order.HasAdhoc = true;
    }
    #endregion
}