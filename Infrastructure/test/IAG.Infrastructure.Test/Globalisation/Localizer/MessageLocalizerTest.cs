using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Resource;
using IAG.Infrastructure.TestHelper.Globalization.ResourceProvider;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Localizer;

[UsedImplicitly]
public class MessageLocalizerFixture
{
    public MessageLocalizerFixture()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var resourceContext = ResourceContextBuilder.GetNewContext();
        resourceContext.Resources.AddRange(
            new Infrastructure.Globalisation.Model.Resource {Name = "test"},
            new Infrastructure.Globalisation.Model.Resource {Name = "error"},
            new Infrastructure.Globalisation.Model.Resource {Name = "warn"},
            new Infrastructure.Globalisation.Model.Resource {Name = "ok"},
            new Infrastructure.Globalisation.Model.Resource {Name = "summary"}
        );
        resourceContext.Cultures.AddRange(
            new Culture {Name = "de"},
            new Culture {Name = "de-CH"},
            new Culture {Name = "en"});
        resourceContext.SaveChanges();
        var resTest = resourceContext.Resources.First(r => r.Name == "test");
        var cultureDe = resourceContext.Cultures.First(c => c.Name == "de");
        resourceContext.Translations.AddRange(
            new Translation
            {
                Culture = cultureDe, Resource = resTest, Value = "Simpel"
            },
            new Translation
            {
                Culture = cultureDe, ResourceId = resourceContext.Resources.First(r => r.Name == "error").Id, Value = "error"
            },
            new Translation
            {
                Culture = cultureDe, ResourceId = resourceContext.Resources.First(r => r.Name == "warn").Id, Value = "warn"
            },
            new Translation
            {
                Culture = cultureDe, ResourceId = resourceContext.Resources.First(r => r.Name == "ok").Id, Value = "ok"
            },
            new Translation
            {
                Culture = cultureDe, ResourceId = resourceContext.Resources.First(r => r.Name == "summary").Id, Value = "summary"
            },
            new Translation
            {
                CultureId = resourceContext.Cultures.First(c => c.Name == "de-CH").Id, Resource = resTest, Value = "Mit Parameter {0}"
            },
            new Translation
            {
                CultureId = resourceContext.Cultures.First(c => c.Name == "en").Id, Resource = resTest, Value = "With param {0}"
            });
        resourceContext.SaveChanges();
        Factory = new DbStringLocalizerFactory(resourceContext);
    }

    public IStringLocalizerFactoryReloadable Factory { get; }
}

public class MessageLocalizerTest : IClassFixture<MessageLocalizerFixture>
{
    private readonly MessageLocalizerFixture _fix;

    public MessageLocalizerTest(MessageLocalizerFixture fix)
    {
        _fix = fix;
    }

    [Fact]
    public void TranslateTestDeSimpel()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de");
        var localizerDe = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "test");
        var msgTrans = localizerDe.Localize(msgStruct);
        Assert.Equal("Simpel", msgTrans.Text);
        Assert.Equal(msgStruct.Timestamp, msgTrans.Timestamp);
    }

    [Fact]
    public void TranslateTestDeSimpelWithParam()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de");
        var localizerDe = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "test", 1);
        var msgTrans = localizerDe.Localize(msgStruct);
        Assert.Equal("Simpel", msgTrans.Text);
    }

    [Fact]
    public void TranslateTestDeWithParam()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizerDe = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "test", "hallo");
        var msgTrans = localizerDe.Localize(msgStruct);
        Assert.Equal("Mit Parameter hallo", msgTrans.Text);
        Assert.NotEmpty(msgTrans.TypeName);
        Assert.Equal(MessageTypeEnum.Information, msgTrans.Type);
    }

    [Fact]
    public void TranslateTestEnWithParam()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var localizerDe = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgStruct = new MessageStructure(MessageTypeEnum.Information, "test", "hallo");
        var msgTrans = localizerDe.Localize(msgStruct);
        Assert.Equal("With param hallo", msgTrans.Text);
    }

    [Fact]
    public void TranslateStacked()
    {
        var messageStructures = new List<MessageStructure>
        {
            new (MessageTypeEnum.Information, "processing invoice {0}", 
                "123"),
            new (MessageTypeEnum.Information, "{0}\r\n{1}\r\n{2}",
                "error in mapping",
                new LocalizableParameter("VinX.ExternalProvider.Failed to create invoice '{0}': {1}", "123", "inner detail"),
                "stack")
        };
        var localizer = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgTrans = localizer.Localize(messageStructures);
        Assert.NotNull(msgTrans);
        Assert.Equal(3, msgTrans.Count);
        Assert.Equal("processing invoice 123", msgTrans[1].Text);
    }

    [Fact]
    public void GetLocalizedExceptionMessageTest()
    {
        var testResource = "{0} {1}";
        var testMessage = string.Format(testResource, "TestMessage", 42);
        var localizableException = new LocalizableException(testResource, "TestMessage", 42);
        var exception = new System.Exception(testMessage);

        var localizer = new MessageLocalizer(_fix.Factory.Create(GetType()));
        var msgFromException = localizer.LocalizeException(exception);
        var msgFromLocalizedException = localizer.LocalizeException(localizableException);

        Assert.NotNull(msgFromException);
        Assert.NotNull(msgFromLocalizedException);
        Assert.Equal(testMessage, msgFromException);
        Assert.Equal(testMessage, msgFromLocalizedException);
    }

    [Fact]
    public void TranslateMsgList()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de");
        var msgs = new List<MessageStructure>
        {
            new(MessageTypeEnum.Information, "test")
        };
        var localizer = new MessageLocalizer(_fix.Factory.Create(GetType()));

        var msgTrans = localizer.Localize(msgs);
        var summary = msgTrans.FirstOrDefault(m => m.Type == MessageTypeEnum.Summary);
        Assert.NotNull(summary);
        Assert.Equal(ResourceIds.GenericOk, summary.Text);

        msgs.Add(new MessageStructure(MessageTypeEnum.Success, "ok"));
        msgTrans = localizer.Localize(msgs);
        summary = msgTrans[0];
        Assert.Equal(MessageTypeEnum.Summary, summary.Type);
        Assert.Equal("ok", summary.Text);

        msgs.Add(new MessageStructure(MessageTypeEnum.Warning, "warn"));
        msgTrans = localizer.Localize(msgs);
        summary = msgTrans[0];
        Assert.Equal("warn", summary.Text);

        msgs.Add(new MessageStructure(MessageTypeEnum.Error, "error"));
        msgTrans = localizer.Localize(msgs);
        summary = msgTrans[0];
        Assert.Equal("error", summary.Text);

        msgs.Add(new MessageStructure(MessageTypeEnum.Summary, "summary"));
        msgTrans = localizer.Localize(msgs);
        summary = msgTrans[0];
        Assert.Equal("summary", summary.Text);
    }
}