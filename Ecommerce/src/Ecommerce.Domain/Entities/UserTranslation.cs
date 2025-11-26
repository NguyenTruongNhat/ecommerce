using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class UserTranslation : DomainEntity<Guid>
{
    public int UserId { get; set; }
    public string LanguageId { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Language Language { get; set; }
}
