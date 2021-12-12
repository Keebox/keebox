using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Keebox.Common.DataAccess.Entities;

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
			return GenerateJwtTokenInternal(userId, roles, true, lifetime);
		}

		public string GenerateNonExpiresJwtToken(Guid userId, Role[] roles)
		{
			return GenerateJwtTokenInternal(userId, roles, false, default);
		}

		private string GenerateJwtTokenInternal(Guid userId, Role[] roles, bool expires, TimeSpan lifetime)
		{
			var signingKey = _keyProvider.GetTokenSigningKey();

			var tokenHandler = new JwtSecurityTokenHandler();
			var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.IsSystem ? role.Name : role.Id.ToString()));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = Constants.JwtTokenIssuer,
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(ClaimTypes.NameIdentifier, userId.ToString())
				}.Concat(roleClaims)),
				Expires = expires ? _dateTimeProvider.UtcNow().Add(lifetime) : null,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		private readonly IKeyProvider _keyProvider;
		private readonly IDateTimeProvider _dateTimeProvider;
	}
}