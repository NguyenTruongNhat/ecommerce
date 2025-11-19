using Ecommerce.Contract.Abstractions.Shared;
using MediatR;

namespace Ecommerce.Contract.Abstractions.Message;
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }
