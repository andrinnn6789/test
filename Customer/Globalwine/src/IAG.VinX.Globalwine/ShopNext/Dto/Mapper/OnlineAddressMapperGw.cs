using System;

using IAG.Infrastructure.ObjectMapper;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Mapper;

public class OnlineAddressMapperFromShop : ObjectMapper<OnlineAddressRestGw, OnlineAddressGw>
{
    protected override OnlineAddressGw MapToDestination(OnlineAddressRestGw source, OnlineAddressGw destination)
    {
        destination.AdditionalAddressLine1 = source.AdditionalAddressLine1;
        destination.AdditionalAddressLine2 = source.AdditionalAddressLine2;
        destination.AddressId = source.AddressId;
        destination.City = source.City;
        destination.Country = source.Country;
        destination.CountryId = source.CountryId;
        destination.Email = source.Email;
        destination.FirstName = source.FirstName;
        destination.Homepage = source.Homepage;
        destination.Id = source.Id;
        destination.Language = source.Language;
        destination.LastName = source.LastName;
        destination.LoginName = source.LoginName;
        destination.OnlineActive = source.OnlineActive;
        destination.PhoneMobile = source.PhoneNumber;
        destination.SalutationId = source.SalutationId;
        destination.Street = source.Street;
        destination.Title = source.Title;
        destination.Zipcode = source.Zipcode;
        destination.Birthday = source.Birthday;

        destination.CustomerCategory = source.CustomerCategory;
        destination.ChangedOn = source.ChangedOn;
        destination.ShopId = source.ShopId;

        if (source.CustomerCategory == AddressstructureGw.Company)
        {
            destination.FirstName = null;
            destination.SalutationId = null;
            destination.LastName = source.Company;
            destination.ContactFirstName = source.FirstName;
            destination.ContactLastName = source.LastName;
            destination.ContactSalutationId = source.SalutationId;
        }

        destination.ChangeType = source.ChangeType switch
        {
            AddressChangeTypeGw.New => Basket.Enum.AddressChangeType.New,
            AddressChangeTypeGw.Change => Basket.Enum.AddressChangeType.Change,
            AddressChangeTypeGw.Nop => Basket.Enum.AddressChangeType.Nop,
            _ => throw new NotSupportedException($"Enum value '{source.ChangeType}' is not supported")
        };

        return destination;
    }
}

public class OnlineAddressMapperToShop : ObjectMapper<OnlineAddressGw, OnlineAddressRestGw>
{
    protected override OnlineAddressRestGw MapToDestination(OnlineAddressGw source, OnlineAddressRestGw destination)
    {
        destination.AdditionalAddressLine1 = source.AdditionalAddressLine1;
        destination.AdditionalAddressLine2 = source.AdditionalAddressLine2;
        destination.AddressId = source.AddressId;
        destination.City = source.City;
        destination.Country = source.Country;
        destination.CountryId = source.CountryId;
        destination.Email = source.Email;
        destination.FirstName = source.FirstName;
        destination.Homepage = source.Homepage;
        destination.Id = source.Id;
        destination.Language = source.Language;
        destination.LastName = source.LastName;
        destination.LoginName = source.LoginName;
        destination.OnlineActive = source.OnlineActive;
        destination.PhoneNumber = source.PhoneMobile;
        destination.SalutationId = source.SalutationId;
        destination.Street = source.Street;
        destination.Title = source.Title;
        destination.Zipcode = source.Zipcode;
        destination.Birthday = source.Birthday;

        destination.CustomerCategory = source.CustomerCategory;
        destination.ChangedOn = source.ChangedOn;
        destination.ShopId = source.ShopId;

        if (source.CustomerCategory == AddressstructureGw.Company)
        {
            destination.FirstName = null;
            destination.Company = source.LastName;
            destination.FirstName = source.ContactFirstName;
            destination.LastName = source.ContactLastName;
            destination.SalutationId = source.ContactSalutationId;
        }

        destination.ChangeType = source.ChangeType switch
        {
            Basket.Enum.AddressChangeType.New => AddressChangeTypeGw.New,
            Basket.Enum.AddressChangeType.Change => AddressChangeTypeGw.Change,
            Basket.Enum.AddressChangeType.Nop => AddressChangeTypeGw.Nop,
            _ => throw new NotSupportedException($"Enum value '{source.ChangeType}' is not supported")
        };

        return destination;
    }
}