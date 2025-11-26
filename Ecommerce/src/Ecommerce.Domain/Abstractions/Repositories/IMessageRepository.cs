using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IMessageRepository : IRepositoryBase<Message, int>
{
    // Add message-specific methods if needed
}
