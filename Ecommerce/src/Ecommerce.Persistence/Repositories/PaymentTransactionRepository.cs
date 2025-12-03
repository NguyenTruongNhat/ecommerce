using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class PaymentTransactionRepository : RepositoryBase<PaymentTransaction, int>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
