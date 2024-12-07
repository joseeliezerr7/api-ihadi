using api_ihadi.Models;
using api_ihadi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_ihadi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors) });
                }

                if (!registerDto.Email.EndsWith("wycliffeassociates.org", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { mensaje = "Solo se permiten correos de wycliffeassociates.org" });
                }

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return BadRequest(new { mensaje = "El correo electrónico ya está registrado" });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                var user = new User
                {
                    Email = registerDto.Email,
                    Password = hashedPassword,
                    Name = registerDto.Name,
                    
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    mensaje = "Usuario registrado exitosamente",
                    datos = new
                    {
                        token,
                        usuario = new { user.Id, user.Email, user.Name }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar usuario: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!loginDto.Email.EndsWith("wycliffeassociates.org", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { mensaje = "Solo se permiten correos de wycliffeassociates.org" });
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso",
                    datos = new
                    {
                        token,
                        usuario = new { user.Id, user.Email, user.Name }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en inicio de sesión: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                return Ok(new
                {
                    mensaje = "Perfil recuperado exitosamente",
                    datos = new { user.Id, user.Email, user.Name }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al recuperar perfil: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                }

                if (!string.IsNullOrEmpty(profileDto.Email) && profileDto.Email != user.Email)
                {
                    if (!profileDto.Email.EndsWith("wycliffeassociates.org", StringComparison.OrdinalIgnoreCase))
                    {
                        return BadRequest(new { mensaje = "Solo se permiten correos de wycliffeassociates.org" });
                    }

                    if (await _context.Users.AnyAsync(u => u.Email == profileDto.Email))
                    {
                        return BadRequest(new { mensaje = "El correo electrónico ya está en uso" });
                    }
                    user.Email = profileDto.Email;
                }

                if (!string.IsNullOrEmpty(profileDto.Name))
                {
                    user.Name = profileDto.Name;
                }

                if (!string.IsNullOrEmpty(profileDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(profileDto.Password);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Perfil actualizado exitosamente",
                    datos = new { user.Id, user.Email, user.Name }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar perfil: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(8); // Token válido por 8 horas

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}