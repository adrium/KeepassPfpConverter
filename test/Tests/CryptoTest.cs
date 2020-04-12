using System;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class CryptoTest
	{
		[Test]
		public void TestHashException()
		{
			var crypto = new Crypto();
			Assert.Throws<InvalidOperationException>(() => crypto.Hash("test", 12)); ;
		}

		[Test]
		public void TestDigestException()
		{
			var crypto = new Crypto();
			Assert.Throws<InvalidOperationException>(() => crypto.Digest("test")); ;
		}

		[Test]
		public void TestEncryptException()
		{
			var crypto = new Crypto();
			Assert.Throws<InvalidOperationException>(() => crypto.Encrypt("test")); ;
		}

		[Test]
		public void TestDecryptExceptionIv()
		{
			var crypto = new Crypto();
			Assert.Throws<InvalidOperationException>(() => crypto.Decrypt("test")); ;
		}

		[Test]
		public void TestDecryptExceptionKey()
		{
			var crypto = new Crypto();
			Assert.Throws<InvalidOperationException>(() => crypto.Decrypt("test_test")); ;
		}

		[Test]
		public void TestHashNormal()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("pass");
			var result = crypto.Hash("test", 12);

			Assert.AreEqual("IyXP5UXMht3+wGcz", result);
		}

		[Test]
		public void TestDigestNormal()
		{
			var crypto = new Crypto();
			crypto.SetHmacSecret("hmac");
			var result = crypto.Digest("test");

			Assert.AreEqual("ChWPtnp/i7/EhH/32W5QE8tP7w64/Xq+AWMGtMa2Jqc=", result);
		}

		[Test]
		public void TestEncryptNormal()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("pass");
			crypto.SetSalt("salt");
			var result = crypto.Encrypt("test", "iviv");

			Assert.AreEqual("iviv_18tHh7ppMBeEeehfegADF67pgjA=", result);
		}

		[Test]
		public void TestDecryptNormal()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("pass");
			crypto.SetSalt("salt");
			var result = crypto.Decrypt("iviv_18tHh7ppMBeEeehfegADF67pgjA=");

			Assert.AreEqual("test", result);
		}

		[Test]
		public void TestGenerateHmacSecret()
		{
			var crypto = new Crypto();
			crypto.GenerateHmacSecret();
			var result = crypto.GetHmacSecret();

			Assert.AreEqual(32, Convert.FromBase64String(result).Length);
		}

		[Test]
		public void TestGenerateSalt()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("pass");
			crypto.GenerateSalt();
			var result = crypto.GetSalt();

			Assert.AreEqual(16, Convert.FromBase64String(result).Length);
		}

		[Test]
		public void TestGenerateRandom()
		{
			var random1 = Crypto.GenerateRandom(12);
			var random2 = Crypto.GenerateRandom(12);

			Assert.AreNotEqual(random1, random2);

			Assert.AreEqual(12, Convert.FromBase64String(random1).Length);
			Assert.AreEqual(12, Convert.FromBase64String(random2).Length);
		}
	}
}
