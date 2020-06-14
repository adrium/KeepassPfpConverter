using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class StringifierVaepTest
	{
		[TestCase("AA==", true, true, true, true, ExpectedResult = "a")]
		[TestCase("M/adrium", true, true, true, true, ExpectedResult = "Zq-4R2")]
		[TestCase("AAAAAA==", true, true, true, true, ExpectedResult = "aA0!")]
		[TestCase("MxoZCQ==", true, true, true, true, ExpectedResult = "Z0z@")]
		[TestCase("AAAAAA==", false, true, true, false, ExpectedResult = "A0A0")]
		[TestCase("EQxLIA==", false, true, true, false, ExpectedResult = "R2D2")]
		[TestCase("EQxLIA==", true, false, false, true, ExpectedResult = "r$d$")]
		public string TestStringify(string bytes, bool lower, bool upper, bool number, bool symbol)
		{
			return Fn()(bytes, lower, upper, number, symbol);
		}

		private Stringify Fn()
		{
			var algo = new StringifierVaep();
			return algo.Stringify;
		}
	}
}
