using System;
using System.Collections.Generic;
using Adrium.KeepassPfpConverter.Algo;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class StringifierUniversalTest
	{
		[TestCase("", ExpectedResult = "")]
		[TestCase("", "ABCD", 0, 4, ExpectedResult = "")]
		[TestCase(new byte[] { 0 }, "ABCD", 0, 4, ExpectedResult = "A")]
		[TestCase(new byte[] { 0, 0, 0, 0 }, "ABCD", 0, 4, "abcd", 0, 4, "1234", 0, 4, ExpectedResult = "AAAA")]
		[TestCase(new byte[] { 0, 0, 0, 0 }, "ABCD", 0, 4, "abcd", 1, 4, "1234", 2, 4, ExpectedResult = "A1a1")]
		[TestCase(new byte[] { 0, 4, 8, 16 }, "ABCD", 0, 4, "abcd", 0, 4, "1234", 0, 4, ExpectedResult = "Aa1a")]
		[TestCase("o6QibwbXIomkX26dXFTgMw==", "ABCD", 0, 16, "abcd", 0, 16, "1234", 0, 16, ExpectedResult = "d13Dc43b14CB1A1D")]
		[TestCase("o6QibwbXIomkX26dXFTgMw==", "ABCD", 2, 4, "abcd", 2, 4, "1234", 2, 4, ExpectedResult = "d13Dc43BadCB")]
		public string TestStringifyNormal(object bytes, params object[] charsettings)
		{
			var settings = new List<StringifierUniversal.Settings>();

			for (var i = 0; i < charsettings.Length; i += 3)
				settings.Add(new StringifierUniversal.Settings((string)charsettings[i], (int)charsettings[i + 1], (int)charsettings[i + 2]));

			var bytestr = bytes is byte[] a ? Convert.ToBase64String(a) : bytes is string s ? s : null;
			var stringifier = new StringifierUniversal(settings);

			return stringifier.Stringify(bytestr);
		}

		[Test]
		public void TestStringifyState()
		{
			var settings = new List<StringifierUniversal.Settings> {
				new StringifierUniversal.Settings("ABCD"),
				new StringifierUniversal.Settings("abcd"),
			};

			var stringifier = new StringifierUniversal(settings);
			var bytes = new byte[12];
			new Random().NextBytes(bytes);

			var bytestr = Convert.ToBase64String(bytes);
			var result1 = stringifier.Stringify(bytestr);
			var result2 = stringifier.Stringify(bytestr);

			Assert.AreEqual(result1, result2);
		}
	}
}
