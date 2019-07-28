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
			var crypto = new Crypto();
			crypto.SetMasterPassword("password");

			var result = PfpConvert.Load(crypto, file);
			AssertEntries(result);
		}

		[Test]
		public void TestSave()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("password");

			var entry1 = Data.SiteEntry;
			var entry2 = Data.AliasEntry;
			var entry3 = Data.StoredEntry;
			var entry4 = Data.Generated2Entry;

			var list = new List<BaseEntry> { entry1, entry2, entry3, entry4 };
			var stream = new MemoryStream();

			PfpConvert.Save(crypto, stream, list);
			stream.Position = 0;

			var result = PfpConvert.Load(crypto, stream);
			AssertEntries(result);
		}

		[Test]
		public void TestDeserializeObjectContainingEntries()
		{
			var json = "{'site':'example.com'}".Replace("'", "\"");
			var result = PfpConvert.DeserializeObjectContainingEntries<BaseEntry>(json) as SiteEntry;
			Assert.AreEqual("example.com", result.site);
		}

		[Test]
		public void TestGenerateSiteEntries()
		{
			var entry1 = Data.StoredEntry;
			var entry2 = Data.Generated2Entry;

			var list = new List<BaseEntry> { entry1, entry2 };

			var result = PfpConvert.GenerateSiteEntries(list);

			Assert.AreSame(entry1, result.First(x => x is StoredEntry));
			Assert.AreSame(entry2, result.First(x => x is GeneratedEntry));

			var entry3 = result.First(x => x is SiteEntry) as SiteEntry;
		}

		[Test]
		public void TestGetAliases()
		{
			var entry1 = Data.StoredEntry;
			var entry2 = Data.AliasEntry;
			var entry3 = Data.SiteEntry;
			entry3.site = "unrelated.local";

			var list = new List<BaseEntry> { entry1, entry2, entry3 };
			var expected = new Dictionary<string, IList<string>> {
				["example.com"] = new List<string> { "example.net" }
			};

			var result = PfpConvert.GetAliases(list);

			Assert.AreEqual(expected, result);
		}

		private void AssertEntries(IList<BaseEntry> list)
		{
			var expected1 = Data.SiteEntry;
			var entry1 = list.First(x => x is SiteEntry e && e.alias == null) as SiteEntry;
			Assert.AreEqual(expected1.ToDict(), entry1.ToDict());

			var expected2 = Data.AliasEntry;
			var entry2 = list.First(x => x is SiteEntry e && e.alias != null) as SiteEntry;
			Assert.AreEqual(expected2.ToDict(), entry2.ToDict());

			var expected3 = Data.StoredEntry;
			var entry3 = list.First(x => x is StoredEntry) as StoredEntry;
			Assert.AreEqual(expected3.ToDict(), entry3.ToDict());

			var expected4 = Data.Generated2Entry;
			var entry4 = list.First(x => x is GeneratedEntry) as GeneratedEntry;
			Assert.AreEqual(expected4.ToDict(), entry4.ToDict());
		}
	}
}
