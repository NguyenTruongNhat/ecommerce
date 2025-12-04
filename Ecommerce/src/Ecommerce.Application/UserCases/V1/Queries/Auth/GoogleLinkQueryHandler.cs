using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Identity;

namespace Ecommerce.Application.UserCases.V1.Queries.Auth;
public class GoogleLinkQueryHandler : IQueryHandler<Query.GoogleLink, Response.GoogleOAuthAuthenticated>
{
    private readonly IGoogleAuthenService _googleAuthenService;
    public GoogleLinkQueryHandler(IGoogleAuthenService googleAuthenService)
    {
        _googleAuthenService = googleAuthenService;
    }
    public async Task<Result<Response.GoogleOAuthAuthenticated>> Handle(Query.GoogleLink request, CancellationToken cancellationToken)
    {
        var url = _googleAuthenService.GetAuthorizationUrl(request);

        var response = new Response.GoogleOAuthAuthenticated
        {
            Link = url
        };
        return Result<Response.GoogleOAuthAuthenticated>.Success(response);
    }
}
