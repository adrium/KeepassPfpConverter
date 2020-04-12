using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adrium.KeepassPfpConverter.Objects;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class PfpConvertTest
	{
		[Test]
		public void TestLoad()
		{
			var file = Data.Json;
			var pfp = new PfpConvert();
			var entries = pfp.Load(Data.Json, "password");

			Assert.AreEqual("example.com", (entries[0] as SiteEntry).site);
			Assert.AreEqual("user", (entries[1] as GeneratedEntry).name);
			Assert.AreEqual(16, (entries[1] as GeneratedEntry).length);
		}

		[Test]
		public void TestSave()
		{
			var stream = new MemoryStream();
			var pfp = new PfpConvert();
			var list = new List<BaseEntry> {
				new StoredEntry { site = "example.com", name = "user", password = "secret" },
			};

			pfp.Save(stream, "password", list);

			stream.Position = 0;
			var entries = pfp.Load(stream, "password");

			Assert.AreEqual("example.com", (entries[0] as SiteEntry).site);
			Assert.AreEqual("user", (entries[1] as StoredEntry).name);
			Assert.AreEqual("secret", (entries[1] as StoredEntry).password);
		}

		[TestCase("secret", ExpectedResult = "secret")]
		public string TestGetPasswordStored(string password)
		{
			return Fn("password")(new StoredEntry { password = password });
		}

		[TestCase(ExpectedResult = "xqzrttvppgfmghyz")]
		public string TestGetPasswordGenerated2()
		{
			return Fn("password")(new GeneratedEntry { site = "example.com", name = "user", length = 16, lower = true });
		}

		[Test]
		public void TestGetPasswordException()
		{
			Assert.Throws<System.ArgumentException>(() => Fn("")(new GeneratedEntry { type = "unknown" }));
		}

		private GetPassword Fn(string password)
		{
			return new PfpConvert().GetPasswordGetter(password);
		}
	}
}
