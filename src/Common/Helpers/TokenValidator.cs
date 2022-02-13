using System.Security.Claims;

using Keebox.Common.DataAccess.Repositories;
using Keebox.Common.DataAccess.Repositories.Abstractions;
using Keebox.Common.Security;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;


namespace Keebox.Common.Helpers
{
	public class TokenValidator : ITokenValidator
	{
		public TokenValidator(ICryptoService cryptoService, IRepositoryContext repositoryContext, IKeyProvider keyProvider)
		{
			_cryptoService = cryptoService;
			_keyProvider = keyProvider;
			_accountRepository = repositoryContext.GetAccountRepository();
		}

		public bool Validate(string token)
		{
			var tokenHash = _cryptoService.GetHash(token);

			return _accountRepository.ExistsWithToken(tokenHash);
		}

		public bool ValidateHash(string tokenHash)
		{
			return _accountRepository.ExistsWithToken(tokenHash);
		}

		public bool ValidateJwtToken(string jwtToken, out ClaimsPrincipal? identity)
		{
			identity = null;

			var signingKey = _keyProvider.GetTokenSigningKey();
			var tokenHandler = new JsonWebTokenHandler();

			try
			{
				var result = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
				{
					ValidateAudience = false,
					ValidateIssuer = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = Constants.JwtTokenIssuer,
					IssuerSigningKey = new SymmetricSecurityKey(signingKey)
				});

				identity = new ClaimsPrincipal(result.ClaimsIdentity);

				return true;
			}
			catch
			{
				return false;
			}
		}

		private readonly IKeyProvider _keyProvider;
		private readonly ICryptoService _cryptoService;
		private readonly IAccountRepository _accountRepository;
	}
}