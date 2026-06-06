using LabLog_1.DTos;
using LabLog_1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabLog_1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere que el usuario esté autenticado para acceder a los endpoints
public class LabNoteController : ControllerBase
{
    private readonly LabNoteService _labNoteService;

    // Inyección de dependencias del servicio encargado de las notas de laboratorio
    public LabNoteController(LabNoteService labNoteService)
    {
        _labNoteService = labNoteService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLabNoteDTo dto)
    {
        // Obtiene el ID del usuario autenticado desde el token JWT
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var note = await _labNoteService.CreateAsync(dto, userId);
        return Ok(note);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Obtiene únicamente las notas asociadas al usuario autenticado
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var notes = await _labNoteService.GetByUserAsync(userId);
        return Ok(notes);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        // Verifica que la nota pertenezca al usuario antes de eliminarla
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var deleted = await _labNoteService.DeleteAsync(id, userId);
        if (!deleted) return Forbid();
        return NoContent();
    }
}