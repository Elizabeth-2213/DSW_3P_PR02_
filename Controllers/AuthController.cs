using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DSW_3P_PR02.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DSW_3P_PR02.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration, SymmetricSecurityKey claveFirma) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public IActionResult Login(LoginDto login)
    {
        var usuario = configuration["Demo:Usuario"];
        var password = configuration["Demo:Password"];
        if (string.IsNullOrWhiteSpace(password) ||
            !string.Equals(login.Usuario, usuario, StringComparison.Ordinal) ||
            !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(login.Password), Encoding.UTF8.GetBytes(password)))
        {
            return Problem(statusCode: 401, title: "Usuario o contraseña incorrectos.");
        }

        var jwt = configuration.GetRequiredSection("Jwt");
        var ahora = DateTime.UtcNow;
        var vigencia = TimeSpan.FromMinutes(jwt.GetValue<int>("ExpiresMinutes"));
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: [new Claim(JwtRegisteredClaimNames.Sub, login.Usuario)],
            notBefore: ahora,
            expires: ahora.Add(vigencia),
            signingCredentials: new SigningCredentials(claveFirma, SecurityAlgorithms.HmacSha256));

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            token_type = "Bearer",
            expires_in = (int)vigencia.TotalSeconds
        });
    }
}
