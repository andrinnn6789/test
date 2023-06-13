using System;

using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

public class RegistrationResultMapper : ObjectMapper<RegistrationListResponseItem, AtlasRegistrationResult>
{
    protected override AtlasRegistrationResult MapToDestination(RegistrationListResponseItem source, AtlasRegistrationResult destination)
    {
        destination.HgfLgavBestaetigung = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        destination.HgfLgavId = source.RegistrationId;
        destination.HgfLgavUebermittelt = true;

        return destination;
    }
}