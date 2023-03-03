namespace CardsAgainstMySanity.Domain.Identity.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenValidator
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IGuestRepository _guestRepository;

    public TokenValidator(IOptions<JwtConfiguration> jwtConfiguration,
                    IGuestRepository guestRepository)
    {
        _jwtConfiguration = jwtConfiguration.Value;
        _guestRepository = guestRepository;
    }

    public async Task<Guest> ValidateAsync(string token, bool validateRefresh = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.SigningKey);

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

            var guestId = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(guestId))
                return null;

            if (validateRefresh)
            {
                var refreshTokenCheck = principal.Claims.FirstOrDefault(x => x.Type == "rsh")?.Value;
                if (string.IsNullOrWhiteSpace(refreshTokenCheck) || refreshTokenCheck != "y")
                    return null;
            }

            var guest = await _guestRepository.FindByIdAsync(Guid.Parse(guestId));

            return guest;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}