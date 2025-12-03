using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class ReviewMediaRepository : RepositoryBase<ReviewMedia, int>, IReviewMediaRepository
{
    public ReviewMediaRepository(ApplicationDbContext context) : base(context)
    {
    }
}
