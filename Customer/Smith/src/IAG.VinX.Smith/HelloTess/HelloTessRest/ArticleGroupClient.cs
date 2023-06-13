using IAG.Infrastructure.Rest;
using IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest;

class ArticleGroupClient : HelloTessBaseClient<ArticleGroup>
{
    public ArticleGroupClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "article-groups", logger)
    {
    }
}