using Adrium.KeepassPfpConverter.Objects;
using Adrium.KeepassPfpConverter.Plugin;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class PluginDedupListTest
	{
		[Test]
		public void TestDeduplicate()
		{
			var entry1 = new StoredEntry { site = "example.com", name = "user" };
			var entry2 = new StoredEntry { site = "example.com", name = "user" };
			var entry3 = new StoredEntry { site = "example.com", name = "user" };
			var entry4 = new StoredEntry { site = "example.com", name = "user" };
			var entrynew1 = new StoredEntry { site = "example.com", name = "user", revision = "new" };
			var entrynew2 = new StoredEntry { site = "example.com", name = "user", revision = "new" };
			var entrynew3 = new StoredEntry { site = "example.com", name = "user", revision = "new" };

			var dedup = new DedupList();

			dedup.Deduplicate(entry1, "Title1");

			Assert.AreEqual("", entry1.revision);

			dedup.Deduplicate(entrynew1, "Title2");

			Assert.AreEqual("", entry1.revision);
			Assert.AreEqual("new", entrynew1.revision);

			dedup.Deduplicate(entry2, "Title3");

			Assert.AreEqual("Title1", entry1.revision);
			Assert.AreEqual("Title3", entry2.revision);
			Assert.AreEqual("new", entrynew1.revision);

			dedup.Deduplicate(entrynew2, "Title4");

			Assert.AreEqual("Title1", entry1.revision);
			Assert.AreEqual("Title3", entry2.revision);
			Assert.AreEqual("new in Title2", entrynew1.revision);
			Assert.AreEqual("new in Title4", entrynew2.revision);

			dedup.Deduplicate(entry3, "Title3");

			Assert.AreEqual("Title1", entry1.revision);
			Assert.AreEqual("Title3", entry2.revision);
			Assert.AreEqual("copy of Title3", entry3.revision);
			Assert.AreEqual("new in Title2", entrynew1.revision);
			Assert.AreEqual("new in Title4", entrynew2.revision);

			dedup.Deduplicate(entry4, "Title3");

			Assert.AreEqual("Title1", entry1.revision);
			Assert.AreEqual("Title3", entry2.revision);
			Assert.AreEqual("copy of Title3", entry3.revision);
			Assert.AreEqual("copy of copy of Title3", entry4.revision);
			Assert.AreEqual("new in Title2", entrynew1.revision);
			Assert.AreEqual("new in Title4", entrynew2.revision);

			dedup.Deduplicate(entrynew3, "Title4");

			Assert.AreEqual("Title1", entry1.revision);
			Assert.AreEqual("Title3", entry2.revision);
			Assert.AreEqual("copy of Title3", entry3.revision);
			Assert.AreEqual("copy of copy of Title3", entry4.revision);
			Assert.AreEqual("new in Title2", entrynew1.revision);
			Assert.AreEqual("new in Title4", entrynew2.revision);
			Assert.AreEqual("copy of new in Title4", entrynew3.revision);
		}
	}
}
