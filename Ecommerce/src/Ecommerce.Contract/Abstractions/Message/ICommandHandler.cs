using Ecommerce.Contract.Abstractions.Shared;
using MediatR;

namespace Ecommerce.Contract.Abstractions.Message;
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{ }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{ }
