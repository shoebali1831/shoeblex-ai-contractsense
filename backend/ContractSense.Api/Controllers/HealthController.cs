using Microsoft.AspNetCore.Mvc;

namespace ContractSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        service = "Shoeblex AI ContractSense API",
        timestampUtc = DateTime.UtcNow
    });
}
