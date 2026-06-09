using Aris.Application.DTOs.Sensores;
using Aris.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aris.api.Controllers;

[ApiController]
[Authorize]
[Route("api/sensores")]
public class SensoresController(ISensorService sensorService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await sensorService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var sensor = await sensorService.GetByIdAsync(id, cancellationToken);
        return sensor is null ? NotFound() : Ok(sensor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSensorDto request, CancellationToken cancellationToken)
    {
        var created = await sensorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSensorDto request, CancellationToken cancellationToken)
        => Ok(await sensorService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await sensorService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
