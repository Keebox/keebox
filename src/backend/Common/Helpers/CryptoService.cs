using System;
using System.Security.Cryptography;
using System.Text;


namespace Keebox.Common.Helpers
{
	public class CryptoService : ICryptoService
	{
		public string GetHash(string input)
		{
			using var algorithm = SHA512.Create();

			return BitConverter.ToString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty).ToLower();
		}
	}
}