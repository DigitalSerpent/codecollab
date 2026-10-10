using Microsoft.AspNetCore.Mvc;
using CodeCollabFrontend.Services;
namespace CodeCollabFrontend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    private readonly ApiService _api;
    public HelloController(ApiService api) { _api = api; }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var alive = await _api.IsWebSocketServerAlive();
        return Ok(new { status = "backend connected", ws_alive = alive });
    }
}
