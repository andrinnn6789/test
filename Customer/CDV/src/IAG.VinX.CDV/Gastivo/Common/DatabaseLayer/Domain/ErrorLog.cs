using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class ErrorLog
{
    public virtual int Id { get; set; }
    public virtual string Title { get; set; }
    public virtual string Occurence { get; set; }
    public virtual DateTime LogDate { get; set; }
    public virtual int DateMillisecond { get; set; }
    public virtual byte[] Description { get; set; }
    public virtual string User { get; set; }
}