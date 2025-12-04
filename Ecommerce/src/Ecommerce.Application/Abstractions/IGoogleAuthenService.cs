using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Contract.Services.V1.Identity.Models;
using static Ecommerce.Contract.Services.V1.Identity.Response;

namespace Ecommerce.Application.Abstractions;
public interface IGoogleAuthenService
{
    string GetAuthorizationUrl(Query.GoogleLink state);
    Task<AuthTokenResponse> GoogleCallbackAsync(string code, string state);
}
