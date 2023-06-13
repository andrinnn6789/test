using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace IAG.Infrastructure.Atlas;

public static class XmlCleaner
{
    public static byte[] Serialize<T>(T data, Encoding encoding)
    {
        var ser = new XmlSerializer(typeof(T));
        // beware: sybase can contain invalid unicode characters
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            Encoding = encoding,
            CheckCharacters = false
        };

        using var stream = new MemoryStream();
        using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
        {
            ser.Serialize(xmlWriter, data, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
        }

        var xmlToCheck = encoding.GetString(stream.ToArray());
        var checkedXml = new Regex("&#x[0-7][\\w]*;").Replace(xmlToCheck, "");
        return encoding.GetBytes(checkedXml);
    }
}