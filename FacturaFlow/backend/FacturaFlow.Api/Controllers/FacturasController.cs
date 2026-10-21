using FacturaFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace FacturaFlow.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class FacturasController(IReminderService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpPost("{id}/procesar")] public async Task<IActionResult> Process(string id, CancellationToken ct) => Ok(await service.ProcessAsync(id, ct));
    [HttpPost("procesar-pendientes")] public async Task<IActionResult> ProcessPending(CancellationToken ct) => Ok(await service.ProcessPendingAsync(ct));
}
