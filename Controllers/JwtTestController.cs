using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace secure_code.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JwtTestController : ControllerBase
    {
        private readonly string _key = "ThisIsASecretKeyForJWTTokenGeneration123456789"; // In production, use secure key from configuration
        private readonly string _issuer = "SecureCodeApp";
        private readonly string _audience = "SecureCodeUsers";

        [HttpGet("encode")]
        public IActionResult EncodeJwt()
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("firstname", "Somchai"),
                    new Claim("lastname", "Jaidee"),
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    jwt = jwt,
                    message = "JWT encoded successfully",
                    payload = new
                    {
                        iss = _issuer,
                        aud = _audience,
                        iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
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
        public IActionResult DecodeJwt([FromQuery] string jwt)
        {
            try
            {
                if (string.IsNullOrEmpty(jwt))
                {
                    return BadRequest(new { error = "JWT token is required" });
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                // Configure validation parameters
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(60) // Leeway of 60 seconds
                };

                // Validate and decode the token
                var principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);
                var decodedToken = (JwtSecurityToken)validatedToken;

                // Extract claims as dictionary
                var decodedClaims = new Dictionary<string, object>();
                foreach (var claim in decodedToken.Claims)
                {
                    decodedClaims[claim.Type] = claim.Value;
                }

                return Ok(new
                {
                    decoded = decodedClaims,
                    message = "JWT decoded successfully",
                    valid = true
                });
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest(new { error = "Failed to decode expired token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to decode JWT", details = ex.Message });
            }
        }

        [HttpPost("test-full-cycle")]
        public IActionResult TestFullCycle()
        {
            try
            {
                // Create token
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("iss", _issuer),
                    new Claim("aud", _audience),
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("nbf", DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds().ToString()),
                    new Claim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()),
                    new Claim("user_id", "12345"),
                    new Claim("username", "test_user")
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                // Decode token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(60)
                };

                var principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);
                var decodedToken = (JwtSecurityToken)validatedToken;

                var decodedClaims = new Dictionary<string, object>();
                foreach (var claim in decodedToken.Claims)
                {
                    decodedClaims[claim.Type] = claim.Value;
                }

                return Ok(new
                {
                    jwt = jwt,
                    encoded_payload = new
                    {
                        iss = _issuer,
                        aud = _audience,
                        iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        nbf = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds(),
                        exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
                        user_id = "12345",
                        username = "test_user"
                    },
                    decoded_payload = decodedClaims,
                    message = "Full encode/decode cycle completed successfully",
                    valid = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Full cycle test failed", details = ex.Message });
            }
        }

        [HttpGet("expired-token")]
        public IActionResult GenerateExpiredToken()
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Create token that's already expired 
                var claims = new[]
                {
                    new Claim("iss", _issuer),
                    new Claim("aud", _audience),
                    new Claim("iat", DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds().ToString()),
                    new Claim("nbf", DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds().ToString()),
                    new Claim("exp", DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds().ToString()) // Expired 1 minute ago
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(-1), // Expired token
                    signingCredentials: credentials
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    jwt = jwt,
                    payload = new
                    {
                        iss = _issuer,
                        aud = _audience,
                        iat = DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds(),
                        nbf = DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds(),
                        exp = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds()
                    },
                    message = "Expired JWT generated for testing",
                    expired = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}