using System;
using System.Linq;
using System.Text;

namespace IAG.Infrastructure.Rest;

public static class UrlHelper
{
    public static string Combine(params string[] urlParts)
    {
        if (urlParts == null)
        {
            throw new ArgumentNullException(nameof(urlParts));
        }
        if (urlParts.Length == 0)
        {
            return string.Empty;
        }

        var previousPart = urlParts.First();
        var url = new StringBuilder(previousPart);
        foreach (string urlPart in urlParts.Skip(1))
        {
            if (urlPart == null)
            {
                throw new ArgumentNullException(nameof(urlParts));
            }

            var delimiterAtEnd = previousPart.EndsWith('/');
            var delimiterAtBegin = urlPart.StartsWith('/');
            if (delimiterAtEnd && delimiterAtBegin)
            {
                url.Append(urlPart.Substring(1));
            }
            else if (!delimiterAtEnd && !delimiterAtBegin)
            {
                url.Append('/').Append(urlPart);
            }
            else
            {
                url.Append(urlPart);
            }
            previousPart = urlPart;
        }

        return url.ToString();
    }
}