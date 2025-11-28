using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;

public interface IVerificationCodeRepository : IRepositoryBase<VerificationCode, int>
{
    Task UpsertAsync(
        string email,
        string code,
        VerificationCodeType type,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default);
}
