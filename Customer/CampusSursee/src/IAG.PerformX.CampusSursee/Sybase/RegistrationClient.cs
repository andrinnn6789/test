using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.PerformX.CampusSursee.Dto;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Registration;

namespace IAG.PerformX.CampusSursee.Sybase;

public class RegistrationClient
{
    private readonly ISybaseConnection _connection;

    public RegistrationClient(ISybaseConnection sybaseConnection)
    {
        _connection = sybaseConnection;
    }

    public RegistrationAddress AddressChange(RegistrationAddress change)
    {
        change.EntryType = (int)ChangeTypeEnum.AddressChange;
        CheckAndComplete(change);
        _connection.Insert(change);
        return change;
    }

    public RegistrationAddress AddressNew(RegistrationAddress newAddress)
    {
        CheckUsernameUnique(newAddress.UserName);
        newAddress.EntryType = (int)ChangeTypeEnum.OrderAddress;
        CheckAndComplete(newAddress);
        _connection.Insert(newAddress);
        return newAddress;
    }

    public RegistrationPending RegistrationNew(RegistrationPending registration)
    {
        try
        {
            _connection.Insert(registration);
        }
        catch (OdbcException e)
        {
            Exception ex = e;
            if (e.Errors.Count > 0)
            {
                switch (e.Errors[0].NativeError)
                {
                    case -194:  // FK-Constraint
                        ex = new BadRequestException(e.Message);
                        break;
                }
            }

            throw ex;
        }
        return registration;
    }

    public RegistrationPending RegistrationWithAddress(RegistrationWithAddress registrationWithAddress)
    {
        try
        {
            _connection.ExecuteInTransaction(() =>
            {
                var registration = registrationWithAddress.Registration;
                var regAdr = registrationWithAddress.RegistrationAddress;
                if (regAdr == null)
                    throw new ArgumentException("RegistrationAddress missing");
                if (regAdr.ChangeType != AddressChangeTypeEnum.Nop && string.IsNullOrWhiteSpace(regAdr.UserName))
                    throw new NoNullAllowedException("UserName must not be empty");
                TransferAddress(regAdr, registration, r => r.AddressId, r => r.RegistrationAddressId);

                regAdr = registrationWithAddress.RegistrationBillingAddress;
                if (regAdr != null)
                {
                    TransferAddress(regAdr, registration, r => r.BillingAddressId, r => r.RegistrationBillingAddressId);
                }

                regAdr = registrationWithAddress.RegistrationCompanyAddress;
                if (regAdr != null)
                {
                    TransferAddress(regAdr, registration, r => r.CompanyAddressId, r => r.RegistrationCompanyAddressId);
                }

                _connection.Insert(registrationWithAddress.Registration);
            });
        }
        catch (OdbcException e)
        {
            Exception ex = e;
            if (e.Errors.Count > 0)
            {
                switch (e.Errors[0].NativeError)
                {
                    case -194:  // FK-Constraint
                        ex = new BadRequestException(e.Message);
                        break;
                }
            }

            throw ex;
        }

        return registrationWithAddress.Registration;
    }
                
    public void UpdateWeblink(int id, string changeWeblink)
    {
        var registrationPending = _connection.GetQueryable<RegistrationPending>().Where(a => a.Id == id).ToList().FirstOrDefault();

        if (registrationPending == null)
            throw new NotFoundException(id.ToString());

        if (string.IsNullOrWhiteSpace(changeWeblink)) 
            throw new NoNullAllowedException("Weblink must not be empty");

        if (registrationPending.IsProcessed)
        {
            var registration = _connection.GetQueryable<RegistrationChange>().Where(a => a.Id == registrationPending.RegistrationId).ToList().FirstOrDefault();

            if (registration == null)
                throw new NotFoundException(id.ToString());

            registration.WebLinkForUserDocuments = changeWeblink;
            _connection.Update(registration);
        }

        registrationPending.WebLinkForUserDocuments = changeWeblink;
        _connection.Update(registrationPending);
    }

    private void TransferAddress<T>(RegistrationAddress regAdr, T target, Expression<Func<T, int?>> addressIdExprOut, Expression<Func<T, int?>> registrationAddressIdExprOut)
    {
        ValidateAddress(regAdr);
        var propAddressId = (PropertyInfo)((MemberExpression)addressIdExprOut.Body).Member;
        var propRegistrationAddressId = (PropertyInfo)((MemberExpression)registrationAddressIdExprOut.Body).Member;
        switch (regAdr.ChangeType)
        {
            case AddressChangeTypeEnum.Nop:
                propAddressId.SetValue(target, regAdr.AddressId, null);
                break;
            case AddressChangeTypeEnum.New:
                propRegistrationAddressId.SetValue(target, AddressNew(regAdr).Id, null);
                break;
            case AddressChangeTypeEnum.Change:
                propRegistrationAddressId.SetValue(target, AddressChange(regAdr).Id, null);
                propAddressId.SetValue(target, regAdr.AddressId, null);
                break;
        }
    }

    private void ValidateAddress(RegistrationAddress address)
    {
        switch (address.ChangeType)
        {
            case AddressChangeTypeEnum.Nop:
                if (!address.AddressId.HasValue)
                    throw new ArgumentException("Missing addressId with changeType 'no change'");
                break;
            case AddressChangeTypeEnum.New:
                if (address.AddressId.HasValue)
                    throw new ArgumentException("AddressId not allowed with changeType 'new'");
                break;
            case AddressChangeTypeEnum.Change:
                if (!address.AddressId.HasValue)
                    throw new ArgumentException("Missing addressId with changeType 'change'");
                break;
            default:
                throw new ArgumentException($"Invalid changeType '{address.ChangeType}' received");
        }
    }

    private void CheckAndComplete(RegistrationAddress change)
    {
        change.Processed = false;
        if (change.Language == null) 
            return;
        change.Language = change.Language.ToUpper();
        if (new[] {"DE", "FR", "IT", "EN"}.All(l => l != change.Language))
            throw new ArgumentException($"Invalid language: {change.Language}");
        if (new[] {10, 20}.All(l => l != change.SbvIsMember))
            throw new ArgumentException($"Invalid SbvIsMember: {change.SbvIsMember}");
    }

    private void CheckUsernameUnique(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return;

        var addresses = _connection.GetQueryable<Address>().Where(a => a.UserName == userName).ToList();
        if (addresses.Count > 0)
            throw new DuplicateKeyException(userName);
    }
}