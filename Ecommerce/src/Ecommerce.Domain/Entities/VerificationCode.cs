using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class VerificationCode : DomainEntity<int>
{
    public string Email { get; set; }
    public string Code { get; set; }
    public VerificationCodeType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
}
