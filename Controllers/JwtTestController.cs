using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens; 
using System.Security.Claims;
using System.Text;

namespace secure_code.Controllers;

[ApiController]
[Route("[controller]")]
// ใช้ Primary Constructor สำหรับ .NET 10 / C# 12+
public class JwtTestController : ControllerBase
{
    private readonly string _key = "ThisIsASecretKeyForJWTTokenGeneration123456789"; 
    private readonly string _issuer = "SecureCodeApp";
    private readonly string _audience = "SecureCodeUsers";
    private readonly JsonWebTokenHandler _tokenHandler = new(); // ประสิทธิภาพสูงกว่า

    [HttpGet("encode")]
    public IActionResult EncodeJwt()
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("firstname", "Somchai"),
                    new Claim("lastname", "Jaidee")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = credentials
            };

            // สร้าง Token แบบรวดเร็ว
            var jwt = _tokenHandler.CreateToken(descriptor);

            return Ok(new
            {
                jwt = jwt,
                message = "JWT encoded successfully (optimized for .NET 10)",
                payload = new
                {
                    iss = _issuer,
                    aud = _audience,
                    iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("decode")]
    public async Task<IActionResult> DecodeJwt([FromQuery] string jwt)
    {
        if (string.IsNullOrEmpty(jwt))
            return BadRequest(new { error = "JWT token is required" });

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ClockSkew = TimeSpan.FromSeconds(60)
        };

        // .NET 10 แนะนำการตรวจสอบแบบ Async และใช้ ValidationResult
        var result = await _tokenHandler.ValidateTokenAsync(jwt, validationParameters);

        if (!result.IsValid)
        {
            return BadRequest(new { 
                error = "Failed to decode JWT", 
                reason = result.Exception?.Message ?? "Invalid Token" 
            });
        }

        // ดึง Claims ออกมาเป็น Dictionary แบบง่ายๆ
        var decodedClaims = result.Claims.ToDictionary(c => c.Key, c => c.Value);

        return Ok(new
        {
            decoded = decodedClaims,
            message = "JWT decoded successfully using JsonWebTokenHandler",
            valid = true
        });
    }

    [HttpPost("test-full-cycle")]
    public async Task<IActionResult> TestFullCycle()
    {
        // สร้าง descriptor เพื่อทดสอบ
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Claims = new Dictionary<string, object> { 
                { "user_id", "12345" }, 
                { "username", "test_user" } 
            },
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };

        var jwt = _tokenHandler.CreateToken(descriptor);
        
        // Decode ทันที
        var result = await _tokenHandler.ValidateTokenAsync(jwt, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            IssuerSigningKey = securityKey
        });

        return Ok(new
        {
            jwt = jwt,
            valid = result.IsValid,
            claims = result.ClaimsIdentity.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpGet("expired-token")]
    public IActionResult GenerateExpiredToken()
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // กำหนดเวลาที่หมดอายุไปแล้ว
            var iat = DateTime.UtcNow.AddHours(-2);
            var exp = DateTime.UtcNow.AddMinutes(-1); // หมดอายุเมื่อ 1 นาทีที่แล้ว

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("test_mode", "expired_test")
                }),
                IssuedAt = iat,
                NotBefore = iat,
                Expires = exp,
                SigningCredentials = credentials
            };

            // ใช้ JsonWebTokenHandler (ประสิทธิภาพสูงกว่า JwtSecurityTokenHandler เดิม)
            var jwt = _tokenHandler.CreateToken(descriptor);

            return Ok(new
            {
                jwt = jwt,
                payload = new
                {
                    iss = _issuer,
                    aud = _audience,
                    iat = new DateTimeOffset(iat).ToUnixTimeSeconds(),
                    nbf = new DateTimeOffset(iat).ToUnixTimeSeconds(),
                    exp = new DateTimeOffset(exp).ToUnixTimeSeconds()
                },
                message = "Expired JWT generated for testing (optimized for .NET 10)",
                expired = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}