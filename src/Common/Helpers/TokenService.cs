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
		public TokenService(IKeyProvider keyProvider)
		{
			_keyProvider = keyProvider;
		}

		public string GenerateStatelessToken()
		{
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		}

		public string GenerateJwtToken(Guid userId, Role[] roles)
		{
			var signingKey = _keyProvider.GetTokenSigningKey();

			var tokenHandler = new JwtSecurityTokenHandler();
			var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.IsSystem ? role.Name : role.Id.ToString()));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(ClaimTypes.NameIdentifier, userId.ToString())
				}.Concat(roleClaims)),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		private readonly IKeyProvider _keyProvider;
	}
}