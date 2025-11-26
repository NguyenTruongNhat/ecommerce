using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IPaymentRepository : IRepositoryBase<Payment, int>
{
    // Add payment-specific methods if needed
}
