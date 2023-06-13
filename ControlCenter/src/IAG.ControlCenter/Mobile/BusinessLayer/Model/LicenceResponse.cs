using System;
using System.Collections.Generic;

using IAG.ControlCenter.Mobile.DataLayer.Model;

namespace IAG.ControlCenter.Mobile.BusinessLayer.Model;

public class LicenceResponse
{
    public LicenceStatusAppEnum LicenceStatus { get; set; }

    public Guid? TenantId { get; set; }

    public List<Backend> Backends { get; set; }

    public List<Module> Modules { get; set; }
}