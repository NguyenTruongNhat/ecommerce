using System.Text;
using Ecommerce.Application.Abstractions;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Authentication;

public interface IGoogleService
{
    string GetAuthorizationUrl(GoogleAuthState state);
    Task<AuthTokenResponse> GoogleCallbackAsync(string code, string state);
}

public class AuthTokenResponse
{
}
public class GoogleAuthState
{
    public string UserAgent { get; set; } = "Unknown";
    public string Ip { get; set; } = "Unknown";
}
public class GoogleService : IGoogleService
{
    private readonly IHashingService _hashingService;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authService;

    private readonly GoogleAuthorizationCodeFlow _flow;

    public GoogleService(
        //IAuthRepository authRepository,
        IHashingService hashingService,
        IRoleRepository roleRepository,
        IAuthenticationService authService,
        IConfiguration config)
    {
        _hashingService = hashingService;
        _roleRepository = roleRepository;
        _authService = authService;

        _flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = config["Google:ClientId"],
                ClientSecret = config["Google:ClientSecret"]
            },
            Scopes = new[]
            {
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile"
            }
        });

        RedirectUri = config["Google:RedirectUri"];
    }

    public string RedirectUri { get; }

    // Tạo URL đăng nhập
    public string GetAuthorizationUrl(GoogleAuthState state)
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
        try
        {
            if (!string.IsNullOrEmpty(state))
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(state));
                var clientInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleAuthState>(decoded);
                userAgent = clientInfo?.UserAgent ?? "Unknown";
                ip = clientInfo?.Ip ?? "Unknown";
            }
        }
        catch
        {
            // Không throw — giống NestJS
        }

        try
        {
            // 2. Exchange code → token
            var token = await _flow.ExchangeCodeForTokenAsync(
                userId: "current-user",
                code: code,
                redirectUri: RedirectUri,
                taskCancellationToken: CancellationToken.None
            );

            var credential = new UserCredential(_flow, "current-user", token);

            // 3. Lấy thông tin user từ Google API
            var oauthService = new Oauth2Service(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Ecommerce"
            });

            // 5. Lấy thông tin người dùng Google
            var googleUser = await oauthService.Userinfo.Get().ExecuteAsync();
            if (googleUser.Email == null)
            {
                Console.WriteLine("+++++++++");
                //throw new GoogleUserInfoException();
            }

            //// 4. Tìm user trong database
            //var user = await _authRepository.FindUniqueUserIncludeRole(googleUser.Email);

            //// Nếu chưa có → tạo mới
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

            //// 5. Tạo device
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
}

