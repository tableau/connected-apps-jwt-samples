using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SampleServer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapPost("/token", (ConnectedApp connectedApp) =>
{
    ArgumentException.ThrowIfNullOrEmpty(connectedApp.ClientId, nameof(connectedApp.ClientId));
    ArgumentException.ThrowIfNullOrEmpty(connectedApp.SecretId, nameof(connectedApp.SecretId));
    ArgumentException.ThrowIfNullOrEmpty(connectedApp.SecretValue, nameof(connectedApp.SecretValue));
    ArgumentException.ThrowIfNullOrEmpty(connectedApp.UserName, nameof(connectedApp.UserName));

    string kid = connectedApp.SecretId;
    string iss = connectedApp.ClientId;
    string sub = connectedApp.UserName;
    string aud = "tableau";
    DateTime exp = DateTime.UtcNow.AddMinutes(1);
    string jti = Guid.NewGuid().ToString();
    string scp = JsonSerializer.Serialize(new[] { "tableau:views:embed", "tableau:views:embed_authoring" });

    Dictionary<string, object> headerClaims = new() { { "iss", iss } };

    byte[] key = Encoding.ASCII.GetBytes(connectedApp.SecretValue);

    // Provides 'kid' and 'alg' header claims.
    SigningCredentials signingCredentials = new(
        new SymmetricSecurityKey(key) { KeyId = kid },
        SecurityAlgorithms.HmacSha256Signature);

    List<Claim> claims = new(
        new[]
        {
            new Claim("sub", sub),
            new Claim("jti", jti),
            new Claim("scp", scp, JsonClaimValueTypes.JsonArray),
        });

    claims.AddRange(connectedApp.UserAttributes.Select(att => new Claim(att.Key, att.Value)));

    ClaimsIdentity subject = new(claims);

    SecurityTokenDescriptor tokenDescriptor = new()
    {
        Audience = aud,
        Subject = subject,
        AdditionalInnerHeaderClaims = headerClaims,
        SigningCredentials = signingCredentials,
        Expires = exp
    };

    JwtSecurityTokenHandler tokenHandler = new();
    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
    string jwt = tokenHandler.WriteToken(token);

    return new { jwt };
});

app.Run();