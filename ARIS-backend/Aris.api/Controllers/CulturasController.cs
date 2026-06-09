using Aris.Application.DTOs.Culturas;
using Aris.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aris.api.Controllers;

[ApiController]
[Authorize]
[Route("api/culturas")]
public class CulturasController(ICulturaService culturaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await culturaService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var cultura = await culturaService.GetByIdAsync(id, cancellationToken);
        return cultura is null ? NotFound() : Ok(cultura);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCulturaDto request, CancellationToken cancellationToken)
    {
        var created = await culturaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCulturaDto request, CancellationToken cancellationToken)
        => Ok(await culturaService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await culturaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
