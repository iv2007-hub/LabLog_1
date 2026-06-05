using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LabLog_1.DTos;
using LabLog_1.Models;
using Microsoft.IdentityModel.Tokens;

namespace LabLog_1.Services;

public class AuthService
{
    private readonly FirebaseService _firebaseService;
    private readonly IConfiguration _configuration;

    public AuthService(FirebaseService firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDTo> RegisterAsync(RegisterDTo dto)
    {
        var collection = _firebaseService.GetCollection("users");
        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con ese correo");

        var userId = Guid.NewGuid().ToString();
        await collection.Document(userId).SetAsync(new Dictionary<string, object>
        {
            { "Id", userId },
            { "DisplayName", dto.DisplayName },
            { "Email", dto.Email },
            { "PasswordHash", HashPassword(dto.Password) },
            { "CreatedAt", DateTime.UtcNow }
        });

        var token = GenerateToken(userId, dto.Email);
        return new AuthResponseDTo { IdToken = token, LocalId = userId, Email = dto.Email };
    }

    public async Task<AuthResponseDTo> LoginAsync(LoginDTo dto)
    {
        var collection = _firebaseService.GetCollection("users");
        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (snapshot.Count == 0)
            throw new Exception("No existe ningun usuario con esa credencial");

        var data = snapshot.Documents[0].ToDictionary();
        var userId = data["Id"].ToString()!;
        var passwordHash = data["PasswordHash"].ToString()!;

        if (!VerifyPassword(dto.Password, passwordHash))
            throw new Exception("Password incorrecto");

        var token = GenerateToken(userId, dto.Email);
        return new AuthResponseDTo { IdToken = token, LocalId = userId, Email = dto.Email };
    }

    private string GenerateToken(string userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}