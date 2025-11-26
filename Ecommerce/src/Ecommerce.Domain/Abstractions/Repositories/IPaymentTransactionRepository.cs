using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IPaymentTransactionRepository : IRepositoryBase<PaymentTransaction, int>
{
    // Add payment-transaction specific methods if needed
}
