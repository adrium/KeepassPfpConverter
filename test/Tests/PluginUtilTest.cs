#if PLUGIN
using System.Collections.Generic;
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
		public void TestGetKeepassEntrySimple()
		{
			var entry = GetPfpEntryObject();

			var resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { PwDefs.PasswordField }), null);

			StringAssert.Contains(entry.site, resultidx[PwDefs.TitleField]);
			Assert.AreEqual(entry.site, resultidx[PwDefs.UrlField]);
			Assert.AreEqual(entry.name, resultidx[PwDefs.UserNameField]);
			Assert.AreEqual(entry.password, resultidx[PwDefs.PasswordField]);
			Assert.IsTrue(resultidx.entry.Strings.Get(PwDefs.PasswordField).IsProtected);
		}

		[Test]
		public void TestGetPfpEntrySimple()
		{
			var entry = GetKeepassEntryObject();

			var result = Util.GetPfpEntry(entry.entry) as StoredEntry;

			Assert.AreEqual(entry[PwDefs.UrlField], result.site);
			Assert.AreEqual(entry[PwDefs.UserNameField], result.name);
			Assert.AreEqual(entry[PwDefs.PasswordField], result.password);
			Assert.AreEqual("", result.revision);
		}

		// TODO null and ""
		[TestCase(PwDefs.UrlField, "pfp.invalid")]
		[TestCase(PwDefs.UserNameField, "(none)")]
		[TestCase(PwDefs.PasswordField, "x")]
		public void TestGetKeepassEntryEmptyFields(string field, string value)
		{
			var entry = GetPfpEntryObject();
			if (field == PwDefs.UrlField) entry.site = value;
			if (field == PwDefs.UserNameField) entry.name = value;
			if (field == PwDefs.PasswordField) entry.password = value;

			var resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { }), null);

			Assert.IsNull(resultidx[field]);
		}

		// TODO [TestCase(null, null, null, null, null, null)]
		[TestCase("", null, null, null, null, null)]
		[TestCase("", "Notes", null, "Notes", null, null)]
		[TestCase("rev", "Notes", "rev", "Notes", null, null)]
		[TestCase("", "Test: OK", null, null, "OK", null)]
		[TestCase("", "Test: Foo: Bar", null, null, "Foo: Bar", null)]
		[TestCase("", "Test: OK\nNotes", null, "Notes", "OK", null)]
		[TestCase("", "Test: OK\nFoo: Bar\nNotes", null, "Notes", "OK", "Bar")]
		[TestCase("rev", "Test: OK\nFoo: Bar\nNotes", "rev", "Notes", "OK", "Bar")]
		[TestCase("new", "Revision: old", "new", null, null, null)]
		public void TestGetKeepassEntryExtraFields(string revin, string notein, string revexp, string noteexp, string testexp, string fooexp)
		{
			var entry = GetPfpEntryObject();
			entry.revision = revin;
			entry.notes = notein;

			var resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { }), null);

			Assert.AreEqual(revexp, resultidx[Util.RevisionField]);
			Assert.AreEqual(noteexp, resultidx[PwDefs.NotesField]);
			Assert.AreEqual(testexp, resultidx["Test"]);
			Assert.AreEqual(fooexp, resultidx["Foo"]);
		}

		[Test]
		public void TestGetPfpEntryEmpty()
		{
			var entry = new PwEntry(true, true);

			var result = Util.GetPfpEntry(entry) as StoredEntry;

			Assert.AreEqual("pfp.invalid", result.site);
			Assert.AreEqual("(none)", result.name);
			Assert.AreEqual("x", result.password);
			Assert.AreEqual("", result.revision);
			Assert.AreEqual(null, result.notes);
		}

		[TestCase("example.com", ExpectedResult = "example.com")]
		[TestCase("http://example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://www.example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://host.example.com/test", ExpectedResult = "host.example.com")]
		[TestCase("https://www.host.example.com/test", ExpectedResult = "host.example.com")]
		[TestCase("https://sub.host.example.com/test", ExpectedResult = "sub.host.example.com")]
		// TODO [TestCase("ftp://example.com/test", ExpectedResult = "example.com")]
		public string TestGetPfpEntryUrlField(string value)
		{
			var entry = GetKeepassEntryObject();
			entry[PwDefs.UrlField] = value;

			var result = Util.GetPfpEntry(entry.entry);

			return result.site;
		}

		[TestCase(null, null, null, null, "", null)]
		[TestCase(null, null, "OK", null, "", "Test: OK")]
		[TestCase(null, null, "OK", "Bar", "", "Foo: Bar\nTest: OK")]
		[TestCase(null, "Notes\r\nNewline", "OK", "Bar", "", "Foo: Bar\nTest: OK\n\nNotes\nNewline")]
		[TestCase(null, "Hello: World\r\nNotes\r\nNewline", "OK", "Bar", "", "Foo: Bar\nHello: World\nTest: OK\n\nNotes\nNewline")]
		[TestCase(null, "Notes\r\nHello: World\r\nNewline", "OK", "Bar", "", "Foo: Bar\nTest: OK\n\nNotes\nHello: World\nNewline")]
		[TestCase("rev", "Notes", "OK", null, "rev", "Test: OK\n\nNotes")]
		[TestCase("rev", "Test: Conflict\nNotes", "OK", null, "rev", "Test: OK\n\nNotes")]
		[TestCase("new", "Revision: old\nNotes", "OK", null, "new", "Test: OK\n\nNotes")]
		public void TestGetPfpEntryNotesField(string revin, string notein, string testin, string fooin, string revexp, string noteexp)
		{
			var entry = GetKeepassEntryObject();
			if (revin != null) entry[Util.RevisionField] = revin;
			if (notein != null) entry[PwDefs.NotesField] = notein;
			if (testin != null) entry["Test"] = testin;
			if (fooin != null) entry["Foo"] = fooin;

			var result = Util.GetPfpEntry(entry.entry);

			Assert.AreEqual(revexp, result.revision);
			Assert.AreEqual(noteexp, result.notes);
		}

		private StoredEntry GetPfpEntryObject()
		{
			return new StoredEntry {
				site = "example.com",
				name = "user",
				password = "secret",
				revision = ""
			};
		}

		[Test]
		public void TestGetMemoryProtection()
		{
			var protect = new MemoryProtectionConfig();
			protect.ProtectUserName = true;
			var result = Util.GetMemoryProtection(protect);

			CollectionAssert.Contains(result, PwDefs.PasswordField);
			CollectionAssert.Contains(result, PwDefs.UserNameField);
			CollectionAssert.DoesNotContain(result, PwDefs.NotesField);
			CollectionAssert.DoesNotContain(result, PwDefs.TitleField);
			CollectionAssert.DoesNotContain(result, PwDefs.UrlField);
		}

		private PwEntryIndexer GetKeepassEntryObject()
		{
			return new PwEntryIndexer(new PwEntry(true, true), new List<string> { }) {
				[PwDefs.UrlField] = "example.com",
				[PwDefs.UserNameField] = "user",
				[PwDefs.PasswordField] = "secret",
			};
		}

		private GetPassword Fn()
		{
			return entry => (entry as StoredEntry).password;
		}
	}
}
#endif
