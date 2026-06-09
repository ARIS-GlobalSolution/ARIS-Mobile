using Aris.Application.DTOs.Estufas;
using Aris.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aris.api.Controllers;

[ApiController]
[Authorize]
[Route("api/estufas")]
public class EstufasController(IEstufaService estufaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await estufaService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var estufa = await estufaService.GetByIdAsync(id, cancellationToken);
        return estufa is null ? NotFound() : Ok(estufa);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstufaDto request, CancellationToken cancellationToken)
    {
        var created = await estufaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEstufaDto request, CancellationToken cancellationToken)
        => Ok(await estufaService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await estufaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
