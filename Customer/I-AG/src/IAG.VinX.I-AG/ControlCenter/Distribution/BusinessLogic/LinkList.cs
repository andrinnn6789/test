using System.Collections.Generic;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public struct LinkList
{
    public List<LinkData> Links { get; set; }
}

public struct LinkData
{
    public string Name { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
}