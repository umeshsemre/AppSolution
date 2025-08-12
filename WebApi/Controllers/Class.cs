using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Model;
using WebApi.Service;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        // Demo users
        private static readonly Dictionary<string, (string Password, List<RefreshToken> RefreshTokens)> Users =
            new()
            {
                ["admin"] = ("admin@123", new List<RefreshToken>()),
                ["bob"] = ("P@ssw0rd2", new List<RefreshToken>())
            };

        public AuthController(IJwtService jwtService) => _jwtService = jwtService;

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (!Users.TryGetValue(req.Username, out var user) || user.Password != req.Password)
                return Unauthorized(new { message = "Invalid credentials" });

            // create claims
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, req.Username),
            new Claim(ClaimTypes.Name, req.Username),
            new Claim(ClaimTypes.NameIdentifier, req.Username) // use real id in real app
        };

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // store refresh token server-side (demo: per user list)
            Users[req.Username].RefreshTokens.Add(refreshToken);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15) // keep in sync with JwtOptions
            };

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // find user by refresh token
            var userEntry = Users.FirstOrDefault(u => u.Value.RefreshTokens.Any(t => t.Token == request.RefreshToken));
            if (userEntry.Key == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            var username = userEntry.Key;
            var token = userEntry.Value.RefreshTokens.Single(t => t.Token == request.RefreshToken);

            if (!token.IsActive)
                return Unauthorized(new { message = "Refresh token expired or revoked" });

            // Optionally revoke the used refresh token and issue a new one
            token.Revoked = DateTime.UtcNow;

            // generate new tokens
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username)
        };
            var newAccessToken = _jwtService.GenerateAccessToken(claims);
            var newRefresh = _jwtService.GenerateRefreshToken();

            Users[username].RefreshTokens.Add(newRefresh);

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefresh.Token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });
        }

        [HttpPost("revoke")]
        public IActionResult Revoke([FromBody] RefreshRequest request)
        {
            var userEntry = Users.FirstOrDefault(u => u.Value.RefreshTokens.Any(t => t.Token == request.RefreshToken));
            if (userEntry.Key == null) return NotFound();

            var token = userEntry.Value.RefreshTokens.Single(t => t.Token == request.RefreshToken);
            token.Revoked = DateTime.UtcNow;
            return NoContent();
        }
    }
}
