namespace Ecommerce.Application.Abstractions;
public interface IHashingService
{
    string Hash(string value);

    bool Compare(string value, string hashedValue);
}
