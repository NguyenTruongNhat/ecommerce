using Ecommerce.Contract.Abstractions.Shared;
using MediatR;

namespace Ecommerce.Contract.Abstractions.Message;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{ }
