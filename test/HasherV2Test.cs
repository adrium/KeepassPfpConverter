using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class HasherV2Test
	{
		[TestCase("test", 12, ExpectedResult = "IyXP5UXMht3+wGcz")]
		public string TestHash(string salt, int length)
		{
			return Fn("pass")(salt, length);
		}

		private Hash Fn(string pass)
		{
			return new HasherV2(pass).Hash;
		}
	}
}
