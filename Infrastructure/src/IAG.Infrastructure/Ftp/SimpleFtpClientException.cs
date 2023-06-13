using System;

namespace IAG.Infrastructure.Ftp;

public class SimpleFtpClientException : ApplicationException
{
    public SimpleFtpClientException(string message)
        : base(message)
    {
    }

    public SimpleFtpClientException(string message, System.Exception innerException) :
        base(message, innerException)
    {
    }
}