using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class PasswordTest
	{
		[TestCase(false, false, false, true, ExpectedResult = "!#$%&(=~!#$%")]
		[TestCase(false, false, true, false, ExpectedResult = "234567256789")]
		[TestCase(false, false, true, true, ExpectedResult = "234567/~2345")]
		[TestCase(false, true, false, false, ExpectedResult = "ABCDEFQYZABC")]
		[TestCase(false, true, false, true, ExpectedResult = "ABCDEF%{|}~A")]
		[TestCase(false, true, true, false, ExpectedResult = "ABCDEFEDEFG9")]
		[TestCase(false, true, true, true, ExpectedResult = "ABCDEFMSTU4%")]
		[TestCase(true, false, false, false, ExpectedResult = "abcdefqyzabc")]
		[TestCase(true, false, false, true, ExpectedResult = "abcdef%{|}~a")]
		[TestCase(true, false, true, false, ExpectedResult = "abcdefedefg9")]
		[TestCase(true, false, true, true, ExpectedResult = "abcdefmstu4%")]
		[TestCase(true, true, false, false, ExpectedResult = "abcdefQyzABC")]
		[TestCase(true, true, false, true, ExpectedResult = "abcdef+GHJKM")]
		[TestCase(true, true, true, false, ExpectedResult = "abcdefxPQRS9")]
		[TestCase(true, true, true, true, ExpectedResult = "abcdef2fgh~C")]
		public string TestToPassword(bool lower, bool upper, bool number, bool symbol)
		{
			return Password.ToPassword("AAECAwQFgPv8/f7/", lower, upper, number, symbol);
		}

		[Test]
		public void TestGetPasswordStored()
		{
			var crypto = new Crypto();
			var entry = Data.StoredEntry;
			var result = Password.GetPassword(crypto, entry);

			Assert.AreEqual(entry.password, result);
		}

		[Test]
		public void TestGetPasswordGenerated2()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("password");

			var entry = Data.Generated2Entry;
			var result = Password.GetPassword(crypto, entry);

			Assert.AreEqual(Data.Generated2Password, result);
		}

		[Test]
		public void TestGeneratePassword2()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("password");

			var entry = Data.Generated2Entry;
			var result = Password.GeneratePassword2(crypto, entry);

			Assert.AreEqual(Data.Generated2Password, result);
		}
	}
}
