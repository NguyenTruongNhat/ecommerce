using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Product;
using Ecommerce.Domain.Abstractions;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.Product;
public sealed class CreateProductCommandHandler : ICommandHandler<Command.CreateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context; 
    private readonly IPublisher _publisher;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductRepository productRepository,
        IPublisher publisher,
        ApplicationDbContext context, ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result> Handle(Command.CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"CreateProductCommand::: {DateTime.Now.ToString()}");
        var product = Domain.Entities.Product.CreateProduct(Guid.NewGuid(), request.Name, request.Price, request.Description, request.Variants);

        _productRepository.Add(product);
        await _context.SaveChangesAsync();

        // Try to get product ID
        var productCreated = await _productRepository.FindByIdAsync(product.Id);

        var productSecond = Domain.Entities.Product.CreateProduct(Guid.NewGuid(), productCreated.Name + " Second",
            productCreated.Price,
            productCreated.Id.ToString(),"test");

        _productRepository.Add(productSecond);
        await _context.SaveChangesAsync();

        await Task.WhenAll(
            _publisher.Publish(new DomainEvent.ProductCreated(productCreated.Id), cancellationToken),
            _publisher.Publish(new DomainEvent.ProductDeleted(product.Id), cancellationToken));

        return Result.Success();
    }
}

