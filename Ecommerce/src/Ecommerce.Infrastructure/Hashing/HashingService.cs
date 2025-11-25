using Ecommerce.Application.Abstractions;

namespace Ecommerce.Infrastructure.Hashing;
public class HashingService : IHashingService
{
    private const int SaltRounds = 10;

    public string Hash(string value)
    {
        return BCrypt.Net.BCrypt.HashPassword(value, SaltRounds);
    }

    public bool Compare(string value, string hashedValue)
    {
        return BCrypt.Net.BCrypt.Verify(value, hashedValue);
    }
}
