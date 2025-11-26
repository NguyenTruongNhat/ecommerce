using Ecommerce.Contract.Abstractions.Message;

namespace Ecommerce.Contract.Services.V1.Product;

public static class Command
{
    public record CreateProductCommand(string Name, decimal Price, string Description, string Variants) : ICommand;

    public record UpdateProductCommand(Guid Id, string Name, decimal Price, string Description) : ICommand;

    public record DeleteProductCommand(Guid Id) : ICommand;
}
