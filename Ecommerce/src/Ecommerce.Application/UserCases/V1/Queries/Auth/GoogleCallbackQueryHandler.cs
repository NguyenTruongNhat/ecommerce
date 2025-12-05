using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Identity;

namespace Ecommerce.Application.UserCases.V1.Queries.Auth;
public class GoogleCallbackQueryHandler : IQueryHandler<Query.GoogleCallback, Response.GoogleCallbackResponse>
{
    private readonly IGoogleAuthenService _googleAuthenService;
    public GoogleCallbackQueryHandler(IGoogleAuthenService googleAuthenService)
    {
        _googleAuthenService = googleAuthenService;
    }

    public async Task<Result<Response.GoogleCallbackResponse>> Handle(Query.GoogleCallback request, CancellationToken cancellationToken)
    {
        var response = await _googleAuthenService.GoogleCallbackAsync(request.code, request.state);
        return Result<Response.GoogleCallbackResponse>.Success(response);
    }
}
