using System.IO;
using System.Net.Mail;

using IAG.Common.MailSender;
using IAG.Common.MailSender.Config;

namespace IAG.VinX.Smith.BossExport.BusinessLogic;

public class SendAsMail
{
    public void Send(MailSenderConfig mailConfig, byte[] fileData, string fileName, string receiver)
    {
        var mail = new MailMessage
        {
            To = { receiver },
            From = new MailAddress(mailConfig.MailSenderAddress),
            Subject = "Export BOSS / Migros",
            Attachments = { new Attachment(new MemoryStream(fileData), fileName) }
        };
        new MailSenderSmtpClient(mailConfig).SendDoc(mail);
    }
}