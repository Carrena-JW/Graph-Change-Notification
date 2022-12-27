using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace msgraphapp;

public class GraphHelper
{
    private readonly GraphConfig _config;
    
    public GraphHelper(GraphConfig config)
    {
        _config = config;
    }
    
    public GraphServiceClient GetGraphClient()
    {
        var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) => {
            // get an access token for Graph
            var accessToken = GetAccessToken().Result;

            requestMessage
                .Headers
                .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            return Task.FromResult(0);
        }));

        return graphClient;
    }

    private async Task<string> GetAccessToken()
    {
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(_config.AppId)
            .WithClientSecret(_config.AppSecret)
            .WithAuthority($"https://login.microsoftonline.com/{_config.TenantId}")
            .WithRedirectUri("https://daemon")
            .Build();

        string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

        var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        return result.AccessToken;
    }
}