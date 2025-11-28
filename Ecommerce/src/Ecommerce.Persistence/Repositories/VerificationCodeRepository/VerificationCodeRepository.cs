using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Persistence.Repositories.VerificationCodeRepository;

public sealed class VerificationCodeRepository : RepositoryBase<VerificationCode, int>, IVerificationCodeRepository
{
    public VerificationCodeRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task UpsertAsync(
        string email,
        string code,
        VerificationCodeType type,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default)
    {
        var existing = await FindSingleAsync(
            x => x.Email == email && x.Type == type,
            cancellationToken
        );
        if (existing is null)
        {
            var vc = new VerificationCode
            {
                Email = email,
                Code = code,
                Type = type,
                ExpiresAt = expiresAt.UtcDateTime
            };
            Add(vc);
        }
        else
        {
            existing.Code = code;
            existing.ExpiresAt = expiresAt.UtcDateTime;
        }

    }
}
