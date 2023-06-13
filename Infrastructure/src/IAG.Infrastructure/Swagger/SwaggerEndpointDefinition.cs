namespace IAG.Infrastructure.Swagger;

public class SwaggerEndpointDefinition
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string UrlEnd => "swagger.json";
}