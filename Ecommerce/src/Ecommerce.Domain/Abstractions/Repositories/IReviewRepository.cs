using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IReviewRepository : IRepositoryBase<Review, int>
{
    // Add review-specific methods if needed
}
