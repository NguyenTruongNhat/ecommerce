using MediatR;

namespace Ecommerce.Contract.Abstractions.Message;
public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
}

