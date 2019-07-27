using Adrium.KeepassPfpConverter.Objects;
using Adrium.KeepassPfpConverter.Plugin;
using KeePassLib;
using NUnit.Framework;

namespace Adrium.KeepassPfpConverter.Test
{
	[TestFixture]
	public class PluginUtilTest
	{
		[Test]
		public void TestGetKeepassEntryStored()
		{
			var crypto = new Crypto();
			var expected = Data.StoredEntry;
			var protect = new MemoryProtectionConfig();
			var result = Util.GetKeepassEntry(crypto, expected, protect);
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual(expected.site, resultidx[PwDefs.UrlField]);
			Assert.AreEqual(expected.name, resultidx[PwDefs.UserNameField]);
			Assert.AreEqual(expected.password, resultidx[PwDefs.PasswordField]);
			Assert.IsTrue(result.Strings.Get(PwDefs.PasswordField).IsProtected);
		}

		[Test]
		public void TestGetKeepassEntryGenerated2()
		{
			var crypto = new Crypto();
			crypto.SetMasterPassword("password");

			var expected = Data.Generated2Entry;
			var protect = new MemoryProtectionConfig();
			var result = Util.GetKeepassEntry(crypto, expected, protect);
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual(expected.site, resultidx[PwDefs.UrlField]);
			Assert.AreEqual(expected.name, resultidx[PwDefs.UserNameField]);
			Assert.AreEqual(Data.Generated2Password, resultidx[PwDefs.PasswordField]);
			Assert.IsTrue(result.Strings.Get(PwDefs.PasswordField).IsProtected);
		}

		[Test]
		public void TestGetPfpEntryStored()
		{
			var expected = Data.StoredEntry;
			var entry = new PwEntry(true, true);
			var entryidx = new PwEntryIndexer(entry, new MemoryProtectionConfig()) {
				[PwDefs.UserNameField] = expected.name,
				[PwDefs.PasswordField] = expected.password,
				[PwDefs.UrlField] = $"https://www.{expected.site}/login?secure=yes"
			};

			var result = Util.GetPfpEntry(new Crypto(), entry) as StoredEntry;

			Assert.AreEqual(expected.site, result.site);
			Assert.AreEqual(expected.name, result.name);
			Assert.AreEqual(expected.password, result.password);
		}

		[Test]
		public void TestGetKeepassNotesNone()
		{
			var expected = Data.StoredEntry;
			expected.revision = "";
			expected.notes = null;

			var result = Util.GetKeepassEntry(new Crypto(), expected, new MemoryProtectionConfig());

			Assert.IsFalse(result.Strings.Exists(PwDefs.NotesField));
		}

		[Test]
		public void TestGetKeepassNotesString()
		{
			var expected = Data.StoredEntry;
			expected.revision = "";
			expected.notes = "message";

			var result = Util.GetKeepassEntry(new Crypto(), expected, new MemoryProtectionConfig());
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual("message", resultidx[PwDefs.NotesField]);
		}

		[Test]
		public void TestGetKeepassNotesRevision()
		{
			var expected = Data.StoredEntry;
			expected.revision = "test";
			expected.notes = null;

			var result = Util.GetKeepassEntry(new Crypto(), expected, new MemoryProtectionConfig());
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual("Revision: test", resultidx[PwDefs.NotesField]);
		}

		[Test]
		public void TestGetKeepassNotesMixed()
		{
			var expected = Data.StoredEntry;
			expected.revision = "test";
			expected.notes = "message";

			var result = Util.GetKeepassEntry(new Crypto(), expected, new MemoryProtectionConfig());
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual("Revision: test\r\nmessage", resultidx[PwDefs.NotesField]);
		}

		[Test]
		public void TestGetKeepassNotesFields()
		{
			var expected = Data.StoredEntry;
			expected.revision = "test";
			expected.notes = "Foo: Bar: Baz\r\nTest: OK\nmessage\n";

			var result = Util.GetKeepassEntry(new Crypto(), expected, new MemoryProtectionConfig());
			var resultidx = new PwEntryIndexer(result, null);

			Assert.AreEqual("Revision: test\r\nmessage", resultidx[PwDefs.NotesField]);
			Assert.AreEqual("Bar: Baz", resultidx["Foo"]);
			Assert.AreEqual("OK", resultidx["Test"]);
		}

		[Test]
		public void TestGetPfpEntryEmpty()
		{
			var entry = new PwEntry(true, true);

			var result = Util.GetPfpEntry(new Crypto(), entry);

			Assert.AreEqual("pfp.invalid", result.site);
			Assert.AreEqual("", result.revision);
			Assert.AreEqual(null, result.notes);
		}

		[Test]
		public void TestGetPfpNotesString()
		{
			var entry = new PwEntry(true, true);
			var entryidx = new PwEntryIndexer(entry, new MemoryProtectionConfig()) {
				[PwDefs.NotesField] = "message"
			};

			var result = Util.GetPfpEntry(new Crypto(), entry);

			Assert.AreEqual("", result.revision);
			Assert.AreEqual("message", result.notes);
		}

		[Test]
		public void TestGetPfpNotesRevision()
		{
			var entry = new PwEntry(true, true);
			var entryidx = new PwEntryIndexer(entry, new MemoryProtectionConfig()) {
				[PwDefs.NotesField] = "Revision: test"
			};

			var result = Util.GetPfpEntry(new Crypto(), entry);

			Assert.AreEqual("test", result.revision);
			Assert.AreEqual(null, result.notes);
		}

		[Test]
		public void TestGetPfpNotesMixed()
		{
			var entry = new PwEntry(true, true);
			var entryidx = new PwEntryIndexer(entry, new MemoryProtectionConfig()) {
				[PwDefs.NotesField] = "Revision: test\r\nmessage"
			};

			var result = Util.GetPfpEntry(new Crypto(), entry);

			Assert.AreEqual("test", result.revision);
			Assert.AreEqual("message", result.notes);
		}

		[Test]
		public void TestGetPfpNotesFields()
		{
			var entry = new PwEntry(true, true);
			var entryidx = new PwEntryIndexer(entry, new MemoryProtectionConfig()) {
				[PwDefs.NotesField] = "Revision: test\r\nSecurity: High\r\nmessage\r\n",
				["Test"] = "OK",
				["Foo"] = "Bar\r\nBaz"
			};

			var result = Util.GetPfpEntry(new Crypto(), entry);

			Assert.AreEqual("test", result.revision);
			Assert.AreEqual("Foo: Bar Baz\nSecurity: High\nTest: OK\nmessage", result.notes);
		}
	}
}
