using System;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public class GenericWamasRecord
{
    public GenericWamasRecord(Type recordType, IWamasRecord recordValue)
    {
        RecordType = recordType;
        RecordValue = recordValue;
    }

    public Type RecordType { get; set; }

    public IWamasRecord RecordValue { get; set; }
}