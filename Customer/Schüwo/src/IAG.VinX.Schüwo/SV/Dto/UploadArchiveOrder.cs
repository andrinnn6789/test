#pragma warning disable IDE1006 // Naming Styles

using System.Collections.Generic;
using IAG.VinX.Schüwo.SV.Dto.Interface;

using JetBrains.Annotations;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
public class UploadArchiveOrder : IOrder<UploadOrderPos>
{
    public string _gpoid { get; set; }

    public int _typecode { get; set; }

    public int _statuscode { get; set; }

    public string _oid { get; set; }

    public string _cid { get; set; }

    public string _date { get; set; }

    public string _executiondate { get; set; }

    public int _poscount { get; set; }

    public IEnumerable<UploadOrderPos> PosData { get; set; }
}

#pragma warning restore IDE1006 // Naming Styles
