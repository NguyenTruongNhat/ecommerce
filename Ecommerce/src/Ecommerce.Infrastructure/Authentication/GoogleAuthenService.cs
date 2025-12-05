using System.Text;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Contract.Services.V1.Identity.Models;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using static Ecommerce.Contract.Services.V1.Identity.Response;

namespace Ecommerce.Infrastructure.Authentication;
public class GoogleAuthenService : IGoogleAuthenService
{

    private readonly GoogleAuthorizationCodeFlow _flow;
    private readonly GoogleAuthOptions _options;

    public string RedirectUri => _options.RedirectUri;

    public GoogleAuthenService(
        IOptions<GoogleAuthOptions> options
        )
    {
        _options = options.Value;

        _flow = BuildGoogleFlow(_options);

    }

    public string GetAuthorizationUrl(Query.GoogleLink state)
    {
        var stateJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            System.Text.Json.JsonSerializer.Serialize(state)
        ));

        var request = _flow.CreateAuthorizationCodeRequest(RedirectUri);
        request.State = stateJson;

        return request.Build().ToString();
    }

    public async Task<GoogleCallbackResponse> GoogleCallbackAsync(string code, string state)
    {
        string userAgent = "Unknown";
        string ip = "Unknown";

        // 1. Decode state (base64 → object)

        if (!string.IsNullOrEmpty(state))
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(state));
            var clientInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleAuthState>(decoded);
            userAgent = clientInfo?.UserAgent ?? "Unknown";
            ip = clientInfo?.Ip ?? "Unknown";
        }

        var token = await _flow.ExchangeCodeForTokenAsync(
            userId: "current-user",
            code: code,
            redirectUri: RedirectUri,
            taskCancellationToken: CancellationToken.None
        );

        var credential = new UserCredential(_flow, "current-user", token);

        // 3. get user info from Google API
        var oauthService = new Oauth2Service(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Ecommerce"
        });

        // 5. Get user infor
        var googleUser = await oauthService.Userinfo.Get().ExecuteAsync();
        if (googleUser.Email == null)
        {
            //throw new GoogleUserInfoException();
        }

        /// At this step, we already have the user's information from Google.
        // We will use the email to check whether the user already exists in the system.
        // If not, create a new user; if yes, log them in and return a JWT.

        return new GoogleCallbackResponse();

    }

    private static GoogleAuthorizationCodeFlow BuildGoogleFlow(GoogleAuthOptions opts)
    {
        return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = opts.ClientId,
                ClientSecret = opts.ClientSecret
            },
            Scopes = new[]
            {
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile"
            }
        });
    }
}

