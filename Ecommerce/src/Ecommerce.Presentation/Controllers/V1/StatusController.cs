using Ecommerce.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Controllers;

public class StatusController : ApiController
{
    private readonly ILogger<StatusController> _logger;

    public StatusController(ISender sender,
        ILogger<StatusController> logger) : base(sender)
    {
        _logger = logger;
    }

    [HttpGet(Name = "status")]
    public string Get()
    {
        _logger.LogInformation("######Status checked at {Time}", DateTime.UtcNow);
        return "ok";
    }
}
