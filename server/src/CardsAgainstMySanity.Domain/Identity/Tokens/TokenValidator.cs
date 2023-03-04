namespace CardsAgainstMySanity.Domain.Identity.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenValidator
{
    private readonly JwtConfiguration _jwtConfiguration;

    public TokenValidator(IOptions<JwtConfiguration> jwtConfiguration) => _jwtConfiguration = jwtConfiguration.Value;

    public bool TryValidate(string token, bool validateRefresh, out Guid principalId)
    {
        principalId = Guid.Empty;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };
            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.SigningKey);

            principalId = Guid.Parse(tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfiguration.Audience,
                ValidateLifetime = true,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validateRefresh)
                {
                    var refreshTokenCheck = principal.Claims.FirstOrDefault(x => x.Type == "rsh")?.Value;
                    if (string.IsNullOrWhiteSpace(refreshTokenCheck) || refreshTokenCheck != "y")
                        return false;
                }

                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
