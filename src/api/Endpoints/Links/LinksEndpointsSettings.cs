namespace LinkForge.API.Endpoints.Links;

public static class LinksEndpointsSettings
{
    public static string[] Tags => [ "links", ];
    
    public const string GetLinkEndpointPattern = "links/{code?}";
    public const string GetLinkEndpointName = "get-link";
    
    public const string PostLinkEndpointPattern = "links";
    public const string PostLinkEndpointName = "post-link";
}