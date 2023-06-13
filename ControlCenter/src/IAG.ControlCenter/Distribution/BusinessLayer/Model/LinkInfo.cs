using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class LinkInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}