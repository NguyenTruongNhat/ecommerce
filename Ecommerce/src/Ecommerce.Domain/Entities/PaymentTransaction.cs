using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class PaymentTransaction : DomainEntity<int>
{
    public string Gateway { get; set; }
    public DateTime TransactionDate { get; set; }
    public string AccountNumber { get; set; }
    public string SubAccount { get; set; }
    public int AmountIn { get; set; }
    public int AmountOut { get; set; }
    public int Accumulated { get; set; }
    public string Code { get; set; }
    public string TransactionContent { get; set; }
    public string ReferenceNumber { get; set; }
    public string Body { get; set; }
}
