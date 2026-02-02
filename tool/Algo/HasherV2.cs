using System;
using System.Text;
using Org.BouncyCastle.Crypto.Generators;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class HasherV2
	{
		private const int N = 32768;
		private const int r = 8;
		private const int p = 1;

		private readonly byte[] passBytes;

		public HasherV2(string password)
		{
			passBytes = Encoding.UTF8.GetBytes(password);
		}

		public string Hash(string salt, int length)
		{
			var S = Encoding.UTF8.GetBytes(salt);
			var hash = SCrypt.Generate(passBytes, S, N, r, p, length);
			var result = Convert.ToBase64String(hash);
			return result;
		}
	}
}
