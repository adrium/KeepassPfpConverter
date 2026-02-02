using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class StringifierV1Test
	{
		[TestCase("AA==", true, true, true, true, ExpectedResult = "a")]
		[TestCase("M/adrium", true, true, true, true, ExpectedResult = "7a]mXc")]
		[TestCase("AAAAAA==", true, true, true, true, ExpectedResult = "aA2!")]
		[TestCase("LRcWGw==", true, true, true, true, ExpectedResult = "Z2z~")]
		[TestCase("AAAAAA==", false, true, true, false, ExpectedResult = "AAA2")]
		[TestCase("Dy9XeQ==", false, true, true, false, ExpectedResult = "ST47")]
		[TestCase("Dy9XeQ==", true, false, false, true, ExpectedResult = "s{:w")]
		public string TestStringify(string bytes, bool lower, bool upper, bool number, bool symbol)
		{
			return Fn()(bytes, lower, upper, number, symbol);
		}

		private Stringify Fn()
		{
			var algo = new StringifierV1();
			return algo.Stringify;
		}
	}
}
