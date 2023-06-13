using System;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public interface IWamasRecord
{
    string Source { get; set; }

    string Target { get; set; }

    int SerialNumber { get; set; }

    DateTime RecordDate { get; set; }

    string DatasetType { get; set; }
}