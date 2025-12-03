using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class BrandRepository : RepositoryBase<Brand, int>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context)
    {
    }
}
