using System.Collections.Generic;
using System.Linq;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.BusinessLogic;

public class AddressTransformer
{
    public List<ContactWithAddress> Transform(IQueryable<ContactWithAddressFlat> contactWithAddresses)
    {
        var sortedList = contactWithAddresses.OrderBy(c => c.AdCustomerNumber).ToList();
        var contacts = new List<ContactWithAddress>();
        foreach (var addressFlat in sortedList)
        {
            var contact = new ContactWithAddress
            {
                Active = addressFlat.KpActive,
                AddressId = addressFlat.AdId,
                Id = addressFlat.KpId,
                ShopId = addressFlat.KpShopId,
                CategoryGroupId = addressFlat.CategoryGroupId,
                CategoryGroupName = addressFlat.CategoryGroupName,
                CustomerNumber = addressFlat.AdCustomerNumber,
                PriceGroupId = addressFlat.PriceGroupId,
                PriceGroupName = addressFlat.PriceGroupName,
                PaymentConditionId = addressFlat.PaymentConditionId,
                Company = addressFlat.AdCustomerCategory == AddressstructureGw.Private ? null : addressFlat.AdLastName,
                ChangedOn = addressFlat.ChangedOn,
                Addresses = new List<AddressGw>(),

                Email = addressFlat.KpEmail,
                FirstName = addressFlat.KpFirstName,
                LastName = addressFlat.KpLastName,
                LoginName = addressFlat.KpLoginName,
                PhoneNumber = addressFlat.KpPhoneNumber,
                Salutation = addressFlat.KpSalutation,
                Birthday = addressFlat.KpBirthday
            };

            var mainAddress = new AddressGw
            {
                ShopId = addressFlat.AdShopId,
                AdditionalAddressLine1 = addressFlat.AdAdditionalAddressLine1,
                AdditionalAddressLine2 = addressFlat.AdAdditionalAddressLine2,
                AddresseTypes = new List<AddressTypeGw>
                {
                    AddressTypeGw.Main, 
                    AddressTypeGw.Delivery
                },
                City = addressFlat.AdCity,
                Company = addressFlat.AdCustomerCategory == AddressstructureGw.Private ? null : addressFlat.AdLastName,
                LastName = addressFlat.AdCustomerCategory == AddressstructureGw.Private ? addressFlat.AdLastName : null,
                Country = addressFlat.AdCountry,
                CustomerNumber = addressFlat.AdCustomerNumber,
                DeliveryBlock = !addressFlat.AdDeliveryOk,
                Email = addressFlat.AdEmail,
                FirstName = addressFlat.AdFirstName,
                Homepage = addressFlat.AdHomepage,
                Id = addressFlat.AdId,
                PhoneNumber = addressFlat.AdPhoneNumber,
                LoginName = addressFlat.AdLoginName,
                SalutationName = addressFlat.AdSalutation,
                Street = addressFlat.AdStreet,
                Title = addressFlat.AdTitle,
                Zipcode = addressFlat.AdZipcode,
                Birthday = addressFlat.AdBirthday,
                SalesRight = addressFlat.AdSalesRight,
                CustomerCategory = addressFlat.AdCustomerCategory
            };
            contact.Addresses.Add(mainAddress);
            if (addressFlat.BiId.HasValue)
            {
                contact.Addresses.Add(new AddressGw
                {
                    ShopId = addressFlat.BiShopId,
                    AdditionalAddressLine1 = addressFlat.BiAdditionalAddressLine1,
                    AdditionalAddressLine2 = addressFlat.BiAdditionalAddressLine2,
                    AddresseTypes = new List<AddressTypeGw>
                    {
                        AddressTypeGw.Billing
                    },
                    City = addressFlat.BiCity,
                    Company = addressFlat.AdCustomerCategory == AddressstructureGw.Private ? null : addressFlat.BiLastName,
                    LastName = addressFlat.AdCustomerCategory == AddressstructureGw.Private ? addressFlat.BiLastName : null,
                    Country = addressFlat.BiCountry,
                    CustomerNumber = addressFlat.BiCustomerNumber,
                    DeliveryBlock = !addressFlat.AdDeliveryOk,
                    Email = addressFlat.BiEmail,
                    FirstName = addressFlat.BiFirstName,
                    Homepage = addressFlat.BiHomepage,
                    Id = addressFlat.BiId.Value,
                    PhoneNumber = addressFlat.BiPhoneNumber,
                    LoginName = addressFlat.BiLoginName,
                    SalutationName = addressFlat.BiSalutation,
                    Street = addressFlat.BiStreet,
                    Title = addressFlat.BiTitle,
                    Zipcode = addressFlat.BiZipcode,
                    Birthday = addressFlat.BiBirthday,
                    SalesRight = addressFlat.BiSalesRight,
                    CustomerCategory = addressFlat.BiCustomerCategory
                });
            }
            else
            {
                mainAddress.AddresseTypes.Add(AddressTypeGw.Billing);
            }

            contacts.Add(contact);
        }
        return contacts;
    }
}