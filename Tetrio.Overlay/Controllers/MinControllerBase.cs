using Microsoft.AspNetCore.Mvc;
using TetraLeague.Overlay.Network.Api;

namespace TetraLeague.Overlay.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class MinControllerBase : ControllerBase
{
    protected readonly TetrioApi _api;

    public MinControllerBase(TetrioApi api)
    {
        _api = api;
    }
}