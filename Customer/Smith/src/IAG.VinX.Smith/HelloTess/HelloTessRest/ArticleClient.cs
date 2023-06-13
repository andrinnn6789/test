using IAG.Infrastructure.Rest;
using IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest;

class ArticleClient : HelloTessBaseClient<Article>
{
    public ArticleClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "articles", logger)
    {
    }
}