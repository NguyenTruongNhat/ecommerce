using Ecommerce.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

public class StatusController : ApiController
{
    public StatusController(ISender sender) : base(sender)
    {
    }

    [HttpGet(Name = "status")]
    public string Get()
    {
        return "ok";
    }
}
