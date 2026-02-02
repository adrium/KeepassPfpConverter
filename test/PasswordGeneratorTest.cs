using Adrium.KeepassPfpConverter.Algo;
using Adrium.KeepassPfpConverter.Objects;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class PasswordGeneratorTest
	{
		[TestCase(100, "example.com", "user", null, ExpectedResult = "str(h(example.com\0user,100))")]
		[TestCase(100, "example.com", "user", "", ExpectedResult = "str(h(example.com\0user,100))")]
		[TestCase(100, "example.com", "user", "rev", ExpectedResult = "str(h(example.com\0user\0rev,100))")]
		public string TestGetHash(int length, string site, string name, string revision)
		{
			var fn = Fn((str, a) => $"h({str},{a})", (str, a, b, c, d) => $"str({str})");
			return fn(new GeneratedEntry { site = site, name = name, revision = revision, length = length });
		}

		[TestCase(false, false, false, true, ExpectedResult = "False,False,False,True")]
		[TestCase(false, false, true, false, ExpectedResult = "False,False,True,False")]
		[TestCase(false, true, false, false, ExpectedResult = "False,True,False,False")]
		[TestCase(true, false, false, false, ExpectedResult = "True,False,False,False")]
		public string TestGetParams(bool a1, bool a2, bool a3, bool a4)
		{
			var fn = Fn((str, a) => "", (str, a, b, c, d) => $"{a},{b},{c},{d}");
			return fn(new GeneratedEntry { lower = a1, upper = a2, number = a3, symbol = a4 });
		}

		private GetPassword Fn(Hash hash, Stringify stringify)
		{
			var algo = new PasswordGenerator(hash, stringify);
			return arg => algo.Get((GeneratedEntry)arg);
		}
	}
}
