using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class CipherV1Test
	{
		[TestCase("iviv", "test", ExpectedResult = "BisSfK7C1m8P4+/FYOTShQ89+Fg=")]
		public string TestEncrypt(string iv, string data)
		{
			return Fn("pass", "salt")(true, iv, data);
		}

		[TestCase("iviv", "BisSfK7C1m8P4+/FYOTShQ89+Fg=", ExpectedResult = "test")]
		public string TestDecrypt(string iv, string data)
		{
			return Fn("pass", "salt")(false, iv, data);
		}

		private Cipher Fn(string pass, string salt)
		{
			Hash hash = (s, l) => new string('.', l).Replace(".", pass).Substring(0, l);
			return new CipherV1(hash, salt).Process;
		}
	}
}
