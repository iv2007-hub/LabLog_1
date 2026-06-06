using LabLog_1.DTos;
using LabLog_1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabLog_1.Controllers;

/// <summary>
/// Controlador para gestionar las notas de laboratorio.
/// Expone endpoints REST para crear, listar y eliminar notas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Solo usuarios autenticados pueden acceder
public class LabNoteController : ControllerBase
{
    // Servicio que contiene la lógica de negocio para las notas
    private readonly LabNoteService _labNoteService;
    
    /// <summary>
    /// Inyecta el servicio de notas mediante el constructor (Dependency Injection).
    /// </summary>
    public LabNoteController(LabNoteService labNoteService)
    {
        _labNoteService = labNoteService;
    }
    /// <summary>
    /// Crea una nueva nota de laboratorio para el usuario autenticado.
    /// </summary>
    /// <param name="dto">Datos necesarios para crear la nota.</param>
    /// <returns>La nota creada con sus datos persistidos.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateLabNoteDTo dto)
    {
        // Extrae el ID del usuario desde el token JWT (claim NameIdentifier)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var note = await _labNoteService.CreateAsync(dto, userId);
        return Ok(note); // 200 OK con la nota creada
    }
    /// <summary>
    /// Obtiene todas las notas de laboratorio del usuario autenticado.
    /// </summary>
    /// <returns>Lista de notas pertenecientes al usuario.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Cada usuario solo puede ver sus propias notas
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var notes = await _labNoteService.GetByUserAsync(userId);
        return Ok(notes); // 200 OK con la lista de notas
    }
    /// <summary>
    /// Elimina una nota de laboratorio por su ID,
    /// validando que pertenezca al usuario autenticado.
    /// </summary>
    /// <param name="id">Identificador único de la nota a eliminar.</param>
    /// <returns>
    /// 204 NoContent si se eliminó correctamente,
    /// 403 Forbid si la nota no pertenece al usuario.
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        // Se valida ownership: el servicio retorna false si la nota no es del usuario
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var deleted = await _labNoteService.DeleteAsync(id, userId);
        if (!deleted) return Forbid(); // 403 — la nota no le pertenece al usuario
        return NoContent(); // 204 — eliminación exitosa
    }
}