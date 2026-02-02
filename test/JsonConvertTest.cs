using Adrium.KeepassPfpConverter.Objects;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class JsonConvertTest
	{
		[Test]
		public void TestDeserializeSiteEntry()
		{
			var json = new JsonConvert();
			var result = json.Deserialize<BaseEntry>("{'site':'example.net','alias':'example.com'}".Replace('\'', '"')) as SiteEntry;
			Assert.AreEqual("example.net", result.site);
			Assert.AreEqual("example.com", result.alias);
		}

		[Test]
		public void TestDeserializeStoredEntry()
		{
			var json = new JsonConvert();
			var result = json.Deserialize<BaseEntry>("{'type':'stored','password':'secret'}".Replace('\'', '"')) as StoredEntry;
			Assert.AreEqual("secret", result.password);
		}

		[Test]
		public void TestDeserializeGeneratedEntry()
		{
			var json = new JsonConvert();
			var result = json.Deserialize<BaseEntry>("{'type':'gen','length':2}".Replace('\'', '"')) as GeneratedEntry;
			Assert.AreEqual("gen", result.type);
			Assert.AreEqual(2, result.length);
		}

		[Test]
		public void TestSerializeSiteEntry()
		{
			var json = new JsonConvert();
			var result = json.Serialize(new SiteEntry { site = "example.net" });
			Assert.AreEqual("{'site':'example.net'}".Replace('\'', '"'), result);
		}

		[Test]
		public void TestSerializeStoredEntry()
		{
			var json = new JsonConvert();
			var result = json.Serialize(new StoredEntry { password = "secret" });
			Assert.AreEqual("{'type':'stored','password':'secret'}".Replace('\'', '"'), result);
		}

		[Test]
		public void TestSerializeGeneratedEntry()
		{
			var json = new JsonConvert();
			var result = json.Serialize(new GeneratedEntry { type = "gen", length = 2 });
			StringAssert.Contains("'type':'gen','length':2,".Replace('\'', '"'), result);
		}
	}
}
