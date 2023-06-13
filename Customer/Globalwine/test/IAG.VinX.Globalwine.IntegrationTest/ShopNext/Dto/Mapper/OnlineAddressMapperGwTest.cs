using System;

using IAG.VinX.Basket.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Mapper;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.Dto.Mapper;

public class OnlineAddressMapperGwTest
{
    [Fact]
    public void FromPrivatTest()
    {
        var mapper = new OnlineAddressMapperFromShop();
        var source = new OnlineAddressRestGw
        {
            AdditionalAddressLine1 = "source.AdditionalAddressLine1",
            AdditionalAddressLine2 = "source.AdditionalAddressLine2",
            AddressId = 1,
            ChangeType = AddressChangeTypeGw.Change,
            City = "source.City",
            Country = "source.Country",
            CountryId = 2,
            Email = "source.Email",
            FirstName = "source.FirstName",
            Homepage = "source.Homepage",
            Company = "source.Company",
            Id = 3,
            Language = "source.Language",
            LastName = "source.LastName",
            LoginName = "source.LoginName",
            OnlineActive = true,
            PhoneNumber = "source.PhoneNumber",
            SalutationId = 4,
            Street = "source.Street",
            Title = "source.Title",
            Zipcode = "source.Zipcode",
            CustomerCategory = AddressstructureGw.Private
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.AdditionalAddressLine1, destination.AdditionalAddressLine1);
        Assert.Equal(source.AdditionalAddressLine2, destination.AdditionalAddressLine2);
        Assert.Equal(source.AddressId, destination.AddressId);
        Assert.Equal((int)source.ChangeType, (int)destination.ChangeType);
        Assert.Equal(source.City, destination.City);
        Assert.Equal(source.Country, destination.Country);
        Assert.Equal(source.CountryId, destination.CountryId);
        Assert.Equal(source.Email, destination.Email);
        Assert.Equal(source.FirstName, destination.FirstName);
        Assert.Equal(source.Homepage, destination.Homepage);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Language, destination.Language);
        Assert.Equal(source.LastName, destination.LastName);
        Assert.Equal(source.LoginName, destination.LoginName);
        Assert.Equal(source.OnlineActive, destination.OnlineActive);
        Assert.Equal(source.PhoneNumber, destination.PhoneMobile);
        Assert.Equal(source.SalutationId, destination.SalutationId);
        Assert.Equal(source.Street, destination.Street);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Zipcode, destination.Zipcode);
    }

    [Fact]
    public void FromCompanyTest()
    {
        var mapper = new OnlineAddressMapperFromShop();
        var source = new OnlineAddressRestGw
        {
            AdditionalAddressLine1 = "source.AdditionalAddressLine1",
            AdditionalAddressLine2 = "source.AdditionalAddressLine2",
            AddressId = 1,
            ChangeType = AddressChangeTypeGw.Change,
            City = "source.City",
            Country = "source.Country",
            CountryId = 2,
            Email = "source.Email",
            FirstName = "source.FirstName",
            Homepage = "source.Homepage",
            Id = 3,
            Company = "source.Company",
            Language = "source.Language",
            LastName = "source.LastName",
            LoginName = "source.LoginName",
            OnlineActive = true,
            PhoneNumber = "source.PhoneNumber",
            SalutationId = 4,
            Street = "source.Street",
            Title = "source.Title",
            Zipcode = "source.Zipcode",
            CustomerCategory = AddressstructureGw.Company
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.AdditionalAddressLine1, destination.AdditionalAddressLine1);
        Assert.Equal(source.AdditionalAddressLine2, destination.AdditionalAddressLine2);
        Assert.Equal(source.AddressId, destination.AddressId);
        Assert.Equal((int)source.ChangeType, (int)destination.ChangeType);
        Assert.Equal(source.City, destination.City);
        Assert.Equal(source.Country, destination.Country);
        Assert.Equal(source.CountryId, destination.CountryId);
        Assert.Equal(source.Email, destination.Email);
        Assert.Null(destination.FirstName);
        Assert.Equal(source.Homepage, destination.Homepage);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Language, destination.Language);
        Assert.Equal(source.Company, destination.LastName);
        Assert.Equal(source.LoginName, destination.LoginName);
        Assert.Equal(source.OnlineActive, destination.OnlineActive);
        Assert.Equal(source.PhoneNumber, destination.PhoneMobile);
        Assert.Equal(source.Street, destination.Street);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Zipcode, destination.Zipcode);
        Assert.Equal(source.FirstName, destination.ContactFirstName);
        Assert.Equal(source.LastName, destination.ContactLastName);
        Assert.Equal(source.SalutationId, destination.ContactSalutationId);
    }

    [Fact]
    public void ToTestPrivate()
    {
        var mapper = new OnlineAddressMapperToShop();
        var source = new OnlineAddressGw
        {
            AdditionalAddressLine1 = "source.AdditionalAddressLine1",
            AdditionalAddressLine2 = "source.AdditionalAddressLine2",
            AddressId = 1,
            ChangeType = AddressChangeType.Change,
            City = "source.City",
            Country = "source.Country",
            CountryId = 2,
            Email = "source.Email",
            FirstName = "source.FirstName",
            Homepage = "source.Homepage",
            Id = 3,
            Language = "source.Language",
            LastName = "source.LastName",
            LoginName = "source.LoginName",
            OnlineActive = true,
            PhoneMobile = "source.PhoneNumber",
            SalutationId = 4,
            Street = "source.Street",
            Title = "source.Title",
            Zipcode = "source.Zipcode",
            CustomerCategory = AddressstructureGw.Private
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.AdditionalAddressLine1, destination.AdditionalAddressLine1);
        Assert.Equal(source.AdditionalAddressLine2, destination.AdditionalAddressLine2);
        Assert.Equal(source.AddressId, destination.AddressId);
        Assert.Equal((int)source.ChangeType, (int)destination.ChangeType);
        Assert.Equal(source.City, destination.City);
        Assert.Equal(source.Country, destination.Country);
        Assert.Equal(source.CountryId, destination.CountryId);
        Assert.Equal(source.Email, destination.Email);
        Assert.Equal(source.FirstName, destination.FirstName);
        Assert.Equal(source.Homepage, destination.Homepage);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Language, destination.Language);
        Assert.Equal(source.LastName, destination.LastName);
        Assert.Equal(source.LoginName, destination.LoginName);
        Assert.Equal(source.OnlineActive, destination.OnlineActive);
        Assert.Equal(source.PhoneMobile, destination.PhoneNumber);
        Assert.Equal(source.SalutationId, destination.SalutationId);
        Assert.Equal(source.Street, destination.Street);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Zipcode, destination.Zipcode);
    }
        
    [Fact]
    public void ToTestCompany()
    {
        var mapper = new OnlineAddressMapperToShop();
        var source = new OnlineAddressGw
        {
            AdditionalAddressLine1 = "source.AdditionalAddressLine1",
            AdditionalAddressLine2 = "source.AdditionalAddressLine2",
            AddressId = 1,
            ChangeType = AddressChangeType.Change,
            City = "source.City",
            Country = "source.Country",
            CountryId = 2,
            Email = "source.Email",
            FirstName = "source.FirstName",
            Homepage = "source.Homepage",
            Id = 3,
            Language = "source.Language",
            LastName = "source.LastName",
            LoginName = "source.LoginName",
            OnlineActive = true,
            PhoneMobile = "source.PhoneNumber",
            SalutationId = 4,
            Street = "source.Street",
            Title = "source.Title",
            Zipcode = "source.Zipcode",
            CustomerCategory = AddressstructureGw.Company
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.AdditionalAddressLine1, destination.AdditionalAddressLine1);
        Assert.Equal(source.AdditionalAddressLine2, destination.AdditionalAddressLine2);
        Assert.Equal(source.AddressId, destination.AddressId);
        Assert.Equal((int)source.ChangeType, (int)destination.ChangeType);
        Assert.Equal(source.City, destination.City);
        Assert.Equal(source.Country, destination.Country);
        Assert.Equal(source.CountryId, destination.CountryId);
        Assert.Equal(source.Email, destination.Email);
        Assert.Null(destination.FirstName);
        Assert.Equal(source.Homepage, destination.Homepage);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Language, destination.Language);
        Assert.Equal(source.LoginName, destination.LoginName);
        Assert.Equal(source.OnlineActive, destination.OnlineActive);
        Assert.Equal(source.PhoneMobile, destination.PhoneNumber);
        Assert.Equal(source.Street, destination.Street);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Zipcode, destination.Zipcode);
        Assert.Equal(source.ContactFirstName, destination.FirstName);
        Assert.Equal(source.ContactLastName, destination.LastName);
        Assert.Equal(source.ContactSalutationId, destination.SalutationId);
        Assert.Equal(source.LastName, destination.Company);
    }

    [Fact]
    public void EnumExceptionTest()
    {
        var source = new OnlineAddressRestGw
        {
            ChangeType = 0
        };
        var sourceGw = new OnlineAddressGw()
        {
            ChangeType = 0
        };

        Assert.Throws<NotSupportedException>(() => new OnlineAddressMapperToShop().NewDestination(sourceGw));
        Assert.Throws<NotSupportedException>(() => new OnlineAddressMapperFromShop().NewDestination(source));
    }
}