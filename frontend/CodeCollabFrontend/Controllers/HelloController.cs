using Microsoft.AspNetCore.Mvc;
using CodeCollabFrontend.Services;

namespace CodeCollabFrontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    private readonly ApiService _api;

    public HelloController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        return await _api.GetHello();
    }
}