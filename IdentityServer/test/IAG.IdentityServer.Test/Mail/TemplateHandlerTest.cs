using System.Collections.Generic;
using System.IO;

using IAG.IdentityServer.Mail;
using IAG.Infrastructure.IdentityServer.Model;

using Xunit;

namespace IAG.IdentityServer.Test.Mail;

public class TemplateHandlerTest
{
    [Fact]
    public void GetMessageTest()
    {
        var templateHandler = new TemplateHandler();
        var templateFileName = Path.GetTempFileName() + ".html";
        var replacements = new Dictionary<string, string>() { { "world", "Welt" }, { "answer", "42" }, { "name", "Kurt" } };
        File.WriteAllText(templateFileName, "Hallo {world}, the answer is {answer}");

        var config = new MailTemplateConfig()
        {
            PathToTemplate = templateFileName,
            Sender = "ego@i-ag.ch",
            Subject = "Test für {name}"
        };

        var message = templateHandler.GetMessage(config, replacements);

        Assert.NotNull(message);
        Assert.NotNull(message.From);
        Assert.Equal(config.Sender, message.From.Address);
        Assert.EndsWith("Kurt", message.Subject);
        Assert.StartsWith("Hallo Welt", message.Body);
        Assert.EndsWith("42", message.Body);
        Assert.True(message.IsBodyHtml);
    }
}