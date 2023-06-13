using System;

namespace IAG.ControlCenter.Mobile.BusinessLayer.Model;

public class Backend
{
    public Guid Id { get; set; }

    public string Url { get; set; }

    public string Name { get; set; }

    public int SyncInterval { get; set; }

    /// <summary>
    /// 3 byte RGB-color as string, eg. "#EFDECD"
    /// </summary>
    public string Color { get; set; }
}