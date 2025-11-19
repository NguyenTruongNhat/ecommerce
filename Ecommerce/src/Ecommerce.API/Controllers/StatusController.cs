using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    
    [HttpGet(Name = "status")]
    public string Get()
    {
        return "ok";
    }
}
