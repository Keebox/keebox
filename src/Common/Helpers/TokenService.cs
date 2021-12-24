using System;
using System.Linq;
using System.Security.Claims;

using Keebox.Common.DataAccess.Entities;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;


namespace Keebox.Common.Helpers
{
	public class TokenService : ITokenService
	{
		public TokenService(IKeyProvider keyProvider, IDateTimeProvider dateTimeProvider)
		{
			_keyProvider = keyProvider;
			_dateTimeProvider = dateTimeProvider;
		}

		public string GenerateStatelessToken()
		{
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		}

		public string GenerateJwtToken(Guid userId, Role[] roles, TimeSpan lifetime)
		{
			return GenerateJwtTokenInternal(userId, roles, lifetime);
		}

		public string GenerateNonExpiresJwtToken(Guid userId, Role[] roles)
		{
			return GenerateJwtTokenInternal(userId, roles, null);
		}

		private string GenerateJwtTokenInternal(Guid userId, Role[] roles, TimeSpan? lifetime)
		{
			var signingKey = _keyProvider.GetTokenSigningKey();

			var tokenHandler = new JsonWebTokenHandler();
			var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.IsSystem ? role.Name : role.Id.ToString()));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = Constants.JwtTokenIssuer,
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(ClaimTypes.NameIdentifier, userId.ToString())
				}.Concat(roleClaims)),
				Expires = lifetime.HasValue ? _dateTimeProvider.UtcNow().Add(lifetime.Value) : _dateTimeProvider.UtcNow().AddYears(50),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
			};

			return tokenHandler.CreateToken(tokenDescriptor);
		}

		private readonly IKeyProvider _keyProvider;
		private readonly IDateTimeProvider _dateTimeProvider;
	}
}