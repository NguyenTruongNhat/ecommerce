using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Websocket : DomainEntity<Guid>
{
    public string Id { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
}
