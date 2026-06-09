using Aris.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aris.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TelemetriasController : ControllerBase
{
    private readonly TelemetrySnapshotStore _store;

    public TelemetriasController(TelemetrySnapshotStore store)
    {
        _store = store;
    }

    [HttpGet("latest")]
    public ActionResult<TelemetrySnapshot> GetLatest()
    {
        return Ok(_store.Current);
    }
}
