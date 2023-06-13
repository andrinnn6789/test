using System;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;
using IAG.PerformX.HGf.Resource;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

public class RegistrationMapper : ObjectMapper<AtlasRegistration, RegistrationListItem>
{
    private readonly string _fileBasePath;

    public RegistrationMapper(string atlasBasePath)
    {
        if (!string.IsNullOrEmpty(atlasBasePath))
        {
            _fileBasePath = atlasBasePath.EndsWith("\\") ? atlasBasePath.Substring(0, atlasBasePath.Length - 1) : atlasBasePath;
        } 
    }

    protected override RegistrationListItem MapToDestination(AtlasRegistration source, RegistrationListItem destination)
    {
        destination.Mode = "upsert";
        destination.RegistrationUid = source.RegistrationUid;
        destination.RegistrationDate = source.RegistrationDate?.ToString("s");

        destination.Registration.EventUid = source.EventUid;
        destination.Registration.Notes = source.Notes;

        MapParticipant(source, destination);
        MapEmployer(source, destination);
        MapContact(source, destination);
        MapEmployment(source, destination);
        MapDocuments(source, destination);

        return destination;
    }

    private void MapParticipant(AtlasRegistration atlasRegistration, RegistrationListItem registrationListItem)
    {
        var atlasParticipant = atlasRegistration.Participant.FirstOrDefault();
        if (atlasParticipant != null)
        {
            registrationListItem.Registration.Participant.PersonUid = atlasParticipant.PersonUid;
            registrationListItem.Registration.Participant.Name = atlasParticipant.Name;
            registrationListItem.Registration.Participant.Name2 = atlasParticipant.Name2;
            registrationListItem.Registration.Participant.Firstname = atlasParticipant.Firstname;
            registrationListItem.Registration.Participant.Title = atlasParticipant.Title;
            registrationListItem.Registration.Participant.AddressRow1 = atlasParticipant.AddressRow1;
            registrationListItem.Registration.Participant.AddressRow2 = atlasParticipant.AddressRow2;
            registrationListItem.Registration.Participant.ZipCode = atlasParticipant.ZipCode;
            registrationListItem.Registration.Participant.Location = atlasParticipant.Location;
            registrationListItem.Registration.Participant.CountryIso = atlasParticipant.CountryIso;
            registrationListItem.Registration.Participant.DateOfBirth = atlasParticipant.DateOfBirth?.ToString("s");
            registrationListItem.Registration.Participant.Gender = MapGender(atlasParticipant);
            registrationListItem.Registration.Participant.SocialSecurityNumber = atlasParticipant.SocialSecurityNumber;
        }
    }

    private string MapGender(AtlasParticipant atlasParticipant)
    {
        switch (atlasParticipant.Gender)
        {
            case 0:
                return "m";
            case 1:
                return "f";
            case 2:
                return "i";
            default:
                return null;
        }
    }

    private void MapEmployer(AtlasRegistration atlasRegistration, RegistrationListItem registrationListItem)
    {
        var atlasEmployer = atlasRegistration.Employer.FirstOrDefault();
        if (atlasEmployer != null)
        {
            registrationListItem.Registration.Employer = new Employer();
            registrationListItem.Registration.Employer.EmployerUid = atlasEmployer.EmployerUid;
            registrationListItem.Registration.Employer.Name = atlasEmployer.Name;
            registrationListItem.Registration.Employer.Name2 = atlasEmployer.Name2;
            registrationListItem.Registration.Employer.AddressRow1 = atlasEmployer.AddressRow1;
            registrationListItem.Registration.Employer.AddressRow2 = atlasEmployer.AddressRow2;
            registrationListItem.Registration.Employer.ZipCode = atlasEmployer.ZipCode;
            registrationListItem.Registration.Employer.Location = atlasEmployer.Location;
            registrationListItem.Registration.Employer.CountryIso = atlasEmployer.CountryIso;
            registrationListItem.Registration.Employer.EMail = atlasEmployer.EMail;
            registrationListItem.Registration.Employer.Phone = atlasEmployer.Phone;
            registrationListItem.Registration.Employer.Url = atlasEmployer.Url;
        }
    }

    private void MapContact(AtlasRegistration atlasRegistration, RegistrationListItem registrationListItem)
    {
        var atlasContact = atlasRegistration.Contact.FirstOrDefault();
        if (atlasContact != null && registrationListItem.Registration.Employer != null)
        {
            registrationListItem.Registration.Employer.Contact = new Contact();
            registrationListItem.Registration.Employer.Contact.ContactId = atlasContact.ContactId;
            registrationListItem.Registration.Employer.Contact.Name = atlasContact.Name;
            registrationListItem.Registration.Employer.Contact.Firstname = atlasContact.Firstname;
            registrationListItem.Registration.Employer.Contact.Title = atlasContact.Title;
            registrationListItem.Registration.Employer.Contact.EMail = atlasContact.EMail;
            registrationListItem.Registration.Employer.Contact.Phone = atlasContact.Phone;
        }
    }

    private void MapEmployment(AtlasRegistration atlasRegistration, RegistrationListItem registrationListItem)
    {
        registrationListItem.Registration.Employment.Position = atlasRegistration.Participant.First()?.PositionHgf;
        registrationListItem.Registration.Employment.EmployedDate = atlasRegistration.EmployedDate?.ToString("s");
        registrationListItem.Registration.Employment.Engagement = atlasRegistration.Engagement;
        registrationListItem.Registration.Employment.LgavSubordinated = atlasRegistration.LgavSubordinated ? "Y" : "F";
        registrationListItem.Registration.Employment.Compensation = atlasRegistration.Compensation ? "Y" : "F";
        registrationListItem.Registration.Employment.ValidFrom = atlasRegistration.ValidFrom?.ToString("s");
    }

    private void MapDocuments(AtlasRegistration atlasRegistration, RegistrationListItem registrationListItem)
    {
        foreach (var atlasDocument in atlasRegistration.Documents)
        {
            if (string.IsNullOrEmpty(atlasDocument.HgfLgavDatei)) continue;

            try
            {
                var fullPath = FormatFilePath(atlasDocument.HgfLgavDatei);

                var bytes = File.ReadAllBytes(fullPath);
                var base64String = Convert.ToBase64String(bytes);
                registrationListItem.Registration.Documents.Add(base64String);
            }
            catch (Exception ex)
            {
                throw new LocalizableException(ResourceIds.RegistrationsLoadFileErrorFormatMessage, ex, atlasDocument.HgfLgavDatei);
            }
        }
    }

    private string FormatFilePath(string relativePath)
    {
        if (relativePath.StartsWith("."))
        {
            relativePath = relativePath.Substring(1);
        }

        return _fileBasePath + relativePath;
    }
}