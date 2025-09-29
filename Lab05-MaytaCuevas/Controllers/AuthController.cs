using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Authorization;
using Lab05_MaytaAlberth.UnitOfWork;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User login)
    {
        try
        {
            // Buscar usuario en la base de datos
            var user = await _unitOfWork.Repository<User>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null)
                return Unauthorized();

            // Verificar contraseña (texto plano o hasheada)
            bool isPasswordValid = IsPasswordValid(login.Password, user.Password);

            if (!isPasswordValid)
                return Unauthorized();

            // Verificar si es admin y generar claims correspondientes
            if (user.Username.ToLower() == "admin")
            {
                var adminClaims = new[]
                {
                    new Claim(type: ClaimTypes.Name, user.Username),
                    new Claim(type: ClaimTypes.Role, "Admin"),
                    // Claims de permisos para Admin
                    new Claim("Permission", "articles.read"),
                    new Claim("Permission", "articles.write"),
                    new Claim("Permission", "articles.edit"),
                };

                var adminToken = GenerateJwtToken(adminClaims);
                return Ok(new { token = adminToken });
            }
            // Si no es admin, es user
            else
            {
                var userClaims = new[]
                {
                    new Claim(type: ClaimTypes.Name, user.Username),
                    new Claim(type: ClaimTypes.Role, "User"),
                    // Claims de permisos para User (solo lectura)
                    new Claim("Permission", "articles.read"),
                };

                var userToken = GenerateJwtToken(userClaims);
                return Ok(new { token = userToken });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error durante el login: {ex.Message}" });
        }
    }

    [HttpPost("encrypt-passwords")]
    public async Task<IActionResult> EncryptPasswords()
    {
        try
        {
            // Obtener todos los usuarios de la base de datos
            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var updatedCount = 0;

            foreach (var user in users)
            {
                // Solo encriptar si la contraseña no está ya encriptada
                if (!IsPasswordHashed(user.Password))
                {
                    // Encriptar la contraseña usando BCrypt
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 11);
                    
                    // Actualizar el usuario en la base de datos
                    _unitOfWork.Repository<User>().Update(user);
                    updatedCount++;
                }
            }

            // Guardar todos los cambios
            if (updatedCount > 0)
            {
                await _unitOfWork.Complete();
            }

            return Ok(new 
            { 
                message = $"Se encriptaron {updatedCount} contraseñas exitosamente",
                total_users = users.Count(),
                updated_passwords = updatedCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al encriptar contraseñas: {ex.Message}" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult GetAdminData()
    {
        return Ok("Datos solo para administradores");
    }

    [Authorize(Roles = "User")]
    [HttpGet("user")]
    public IActionResult GetUserData()
    {
        return Ok("Datos solo para usuarios");
    }

    private bool IsPasswordValid(string inputPassword, string storedPassword)
    {
        // Si la contraseña está hasheada con BCrypt, verificar con BCrypt
        if (IsPasswordHashed(storedPassword))
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }
        // Si no está hasheada, comparar como texto plano
        return inputPassword == storedPassword;
    }

    private bool IsPasswordHashed(string password)
    {
        // BCrypt genera hashes que empiezan con $2a$, $2b$ o $2y$
        return password.StartsWith("$2a$") || 
               password.StartsWith("$2b$") || 
               password.StartsWith("$2y$");
    }

    private string GenerateJwtToken(Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, algorithm: SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}