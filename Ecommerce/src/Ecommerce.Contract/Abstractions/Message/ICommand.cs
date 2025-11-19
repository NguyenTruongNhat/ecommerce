using Ecommerce.Contract.Abstractions.Shared;
using MediatR;

namespace Ecommerce.Contract.Abstractions.Message;
public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
