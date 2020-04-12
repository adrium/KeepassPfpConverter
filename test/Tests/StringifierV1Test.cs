using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class StringifierV1Test
	{
		[TestCase(false, false, false, true, ExpectedResult = "!#$%#$%^>)<}")]
		[TestCase(false, false, true, false, ExpectedResult = "234578987858")]
		[TestCase(false, false, true, true, ExpectedResult = "2345345^(^9;")]
		[TestCase(false, true, false, false, ExpectedResult = "ABCDABCTWQXF")]
		[TestCase(false, true, false, true, ExpectedResult = "ABCD}~A@EY[Q")]
		[TestCase(false, true, true, false, ExpectedResult = "ABCDFGH8CWPN")]
		[TestCase(false, true, true, true, ExpectedResult = "ABCDUVWM+|/8")]
		[TestCase(true, false, false, false, ExpectedResult = "abcdabctwqxf")]
		[TestCase(true, false, false, true, ExpectedResult = "abcd}~a@ey[q")]
		[TestCase(true, false, true, false, ExpectedResult = "abcdfgh8cwpn")]
		[TestCase(true, false, true, true, ExpectedResult = "abcduvwm+|/8")]
		[TestCase(true, true, false, false, ExpectedResult = "abcdABCtwQXF")]
		[TestCase(true, true, false, true, ExpectedResult = "abcdJKMBkDX}")]
		[TestCase(true, true, true, false, ExpectedResult = "abcdRSTH5pXe")]
		[TestCase(true, true, true, true, ExpectedResult = "abcdhjka]mP8")]
		public string TestStringify(bool lower, bool upper, bool number, bool symbol)
		{
			return Fn()("AAECA/3+//adrium", lower, upper, number, symbol);
		}

		private Stringify Fn()
		{
			var algo = new StringifierV1();
			return algo.Stringify; 
		}
	}
}
