using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Domain.Entities;

public class VerificationCode
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
    public VerificationCodeType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
}
