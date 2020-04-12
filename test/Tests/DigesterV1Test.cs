using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class DigesterV1Test
	{
		[TestCase("test", ExpectedResult = "ChWPtnp/i7/EhH/32W5QE8tP7w64/Xq+AWMGtMa2Jqc=")]
		public string TestDigest(string data)
		{
			return Fn("hmac")(data);
		}

		private Digest Fn(string hmac)
		{
			return new DigesterV1(hmac).Digest;
		}
	}
}
