using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Websocket
{
    public string Id { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual AppUser User { get; set; }
}
