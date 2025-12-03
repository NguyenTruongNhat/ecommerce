using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class DeviceRepository : RepositoryBase<Device, int>, IDeviceRepository
{
    public DeviceRepository(ApplicationDbContext context) : base(context)
    {
    }
}
