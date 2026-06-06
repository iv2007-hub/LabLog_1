using LabLog_1.DTos;
using LabLog_1.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabLog_1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    // Endpoint POST: api/Auth/register
    // Permite registrar un nuevo usuario
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTo dto)
    {
        // Llama al servicio para registrar el usuario
        var result = await _authService.RegisterAsync(dto);
        
        // Retorna una respuesta HTTP 200 con los datos del usuario registrado
        return Ok(result);
    }
    
    // Endpoint POST: api/Auth/login
    // Permite iniciar sesión con correo y contraseña
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTo dto)
    {
        // Llama al servicio para validar credenciales y generar el token JWT
        var result = await _authService.LoginAsync(dto);
        
        // Retorna una respuesta HTTP 200 con el token y datos del usuario
        return Ok(result);
    }
}