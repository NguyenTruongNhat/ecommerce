using System.Text;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Contract.Services.V1.Identity.Models;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using static Ecommerce.Contract.Services.V1.Identity.Response;

namespace Ecommerce.Infrastructure.Authentication;
public class GoogleAuthenService : IGoogleAuthenService
{
    private readonly IHashingService _hashingService;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authService;

    private readonly GoogleAuthorizationCodeFlow _flow;
    private readonly GoogleAuthOptions _options;

    public string RedirectUri => _options.RedirectUri;

    public GoogleAuthenService(
        //IAuthRepository authRepository,
        IHashingService hashingService,
        IRoleRepository roleRepository,
        IAuthenticationService authService
        )
    {
        _hashingService = hashingService;
        _roleRepository = roleRepository;
        _authService = authService;

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

    public async Task<AuthTokenResponse> GoogleCallbackAsync(string code, string state)
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



        try
        {
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
                Console.WriteLine("+++++++++");
                //throw new GoogleUserInfoException();
            }

            //var user = await _authRepository.FindUniqueUserIncludeRole(googleUser.Email);

            //if (user == null)
            //{
            //    var roleId = await _roleRepository.GetClientRoleId();
            //    var randomPassword = Guid.NewGuid().ToString();
            //    var hashedPassword = _hashingService.Hash(randomPassword);

            //    user = await _authRepository.CreateUserIncludeRole(new CreateUserModel
            //    {
            //        Email = googleUser.Email,
            //        Name = googleUser.Name ?? "",
            //        Password = hashedPassword,
            //        RoleId = roleId,
            //        PhoneNumber = "",
            //        Avatar = googleUser.Picture
            //    });
            //}

            //// 5. Create device
            //var device = await _authRepository.CreateDevice(new CreateDeviceModel
            //{
            //    UserId = user.Id,
            //    UserAgent = userAgent,
            //    Ip = ip
            //});

            //// 6. Generate JWT
            //var tokens = await _authService.GenerateTokens(new GenerateTokenRequest
            //{
            //    UserId = user.Id,
            //    DeviceId = device.Id,
            //    RoleId = user.RoleId,
            //    RoleName = user.Role.Name
            //});

            return new AuthTokenResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GoogleCallback: " + ex);
            throw;
        }
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

