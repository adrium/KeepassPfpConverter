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
			var pfp = new PfpConvert();
			var entries = pfp.Load(Data.Json, "password");

			Assert.AreEqual("example.com", (entries[0] as SiteEntry).site);
			Assert.AreEqual("user", (entries[1] as GeneratedEntry).name);
			Assert.AreEqual(16, (entries[1] as GeneratedEntry).length);
		}

		[Test]
		public void TestLoadException()
		{
			var pfp = new PfpConvert();
			Assert.Throws<PfpConvert.PfpConvertException>(() => pfp.Load(Data.Invalid, ""));
		}

		[Test]
		public void TestSave()
		{
			var stream = new MemoryStream();
			var pfp = new PfpConvert();
			var entry = new StoredEntry { site = "example.com", name = "user", password = "secret" };
			var list = new List<BaseEntry> { entry };

			pfp.Save(stream, "password", list);

			stream.Position = 0;
			var entries = pfp.Load(stream, "password");

			Assert.AreEqual("example.com", (entries[0] as SiteEntry).site);
			Assert.AreEqual("user", (entries[1] as StoredEntry).name);
			Assert.AreEqual("secret", (entries[1] as StoredEntry).password);
		}

		[Test]
		public void TestSaveException()
		{
			var stream = new MemoryStream();
			var pfp = new PfpConvert();
			var entry = new StoredEntry { site = "example.com", name = "user", password = "secret" };
			var list = new List<BaseEntry> { entry, entry };

			Assert.Throws<PfpConvert.PfpConvertException>(() => pfp.Save(stream, "password", list));
		}

		[TestCase("secret", ExpectedResult = "secret")]
		public string TestGetPasswordStored(string password)
		{
			return Fn("password")(new StoredEntry { password = password });
		}

		[TestCase(ExpectedResult = "h2z}+tFz;8s^+uYk")]
		public string TestGetPasswordGeneratedPfp()
		{
			return Fn("password")(new GeneratedEntry { site = "example.com", name = "user" });
		}

		[TestCase(ExpectedResult = "r4wC!qMGg0zyaBS#")]
		public string TestGetPasswordGeneratedAep()
		{
			return Fn("password")(new GeneratedEntry { site = "example.com", name = "user", type = PfpConvert.GENERATED_AEP_TYPE });
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
