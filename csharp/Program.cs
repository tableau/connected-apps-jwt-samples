using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

#region Variables

// Replace the example values below (remove the brackets).
// Store secrets securely based on your team's best practices.
// See: https://help.tableau.com/current/online/en-us/connected_apps_direct.htm

string secretId = "[Tableau Connected App Direct Trust Secret ID]";
string secretValue = "[Tableau Connected App Direct Trust Secret Value]";
string clientId = "[Tableau Connected App Direct Trust Client ID]";
string username = "[Tableau Username]";
double tokenExpiryInMinutes = 1; // Max of 10 minutes.

Dictionary<string, string> userAttributes = new()
{
    // User attributes are optional.
    // Add entries to this dictionary if desired.
    //{ "[User Attribute Name]", "[User Attribute Value]" }
};

// Remove 'embed_authoring' scope if Authoring is not needed.
string[] scopes = new[] { "tableau:views:embed", "tableau:views:embed_authoring" };

#endregion

#region JWT generation

string kid = secretId;
string iss = clientId;
string sub = username;
string aud = "tableau";
DateTime exp = DateTime.UtcNow.AddMinutes(tokenExpiryInMinutes);
string jti = Guid.NewGuid().ToString();
string scp = JsonSerializer.Serialize(scopes);

Dictionary<string, object> headerClaims = new() { { "iss", iss } };

byte[] key = Encoding.ASCII.GetBytes(secretValue);

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

claims.AddRange(userAttributes.Select(att => new Claim(att.Key, att.Value)));

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

Console.WriteLine(jwt);

#endregion
