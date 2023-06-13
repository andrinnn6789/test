using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.Dto;
using IAG.Common.EBill.BusinessLogic;
using IAG.VinX.SwissDrink.Dto;
using IAG.VinX.SwissDrink.MailSender;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.MailSender;

public class MailEnhancerZugferdTest
{
    private readonly MailEnhancerZugferd _mailEnhancer;
    private readonly Mock<ISybaseConnection> _connectionMock;

    public MailEnhancerZugferdTest()
    {
        _connectionMock = new Mock<ISybaseConnection>();
        _connectionMock.Setup(m => m.GetQueryable<ArchiveLink>()).Returns(
            new List<ArchiveLink>
            {
                new()
                {
                    Id = 1,
                    ForeignId = 2
                },
                new()
                {
                    Id = 2,
                    ForeignId = 3,
                    Tablename = "Beleg"
                }
            }.AsQueryable);
        _mailEnhancer = new MailEnhancerZugferd(new Mock<IZugferdEmbedder>().Object, _connectionMock.Object);
    }

    [Fact]
    public void NopArchivLink()
    {
        using var memStream = new MemoryStream(new byte[10]);
        _mailEnhancer.Enhance(memStream, 1);
        Assert.Equal(10, memStream.Length);
    }

    [Fact]
    public void NopOp()
    {
        using var memStream = new MemoryStream(new byte[10]);
        _mailEnhancer.Enhance(memStream, 2);
        Assert.Equal(10, memStream.Length);
    }

    [Fact]
    public void NopOpZugferd()
    {
        _connectionMock.Setup(m => m.GetQueryable<OpData>()).Returns(
            new List<OpData>
            {
                new()
                {
                    Id = 4,
                    ReceiptId = 3,
                    HasZugferd = false
                }
            }.AsQueryable);
        using var memStream = new MemoryStream(new byte[10]);
        _mailEnhancer.Enhance(memStream, 2);
        Assert.Equal(10, memStream.Length);
    }

    [Fact]
    public void EmbeddPdf()
    {
        _connectionMock.Setup(m => m.GetQueryable<OpData>()).Returns(
            new List<OpData>
            {
                new()
                {
                    Id = 4,
                    ReceiptId = 3,
                    HasZugferd = true
                }
            }.AsQueryable);
        using var memStream = new MemoryStream(new byte[10]);
        _mailEnhancer.Enhance(memStream, 2);
        Assert.Equal(10, memStream.Length);
    }
}