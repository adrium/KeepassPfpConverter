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
			Assert.IsFalse(resultidx.entry.Strings.Get(PwDefs.UserNameField).IsProtected);
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

		[TestCase(PwDefs.UrlField, "pfp.invalid")]
		[TestCase(PwDefs.UserNameField, "(none)")]
		[TestCase(PwDefs.PasswordField, "x")]
		public void TestGetKeepassEntryEmptyFields(string field, string value)
		{
			var entry = GetPfpEntryObject();
			if (field == PwDefs.UrlField) entry.site = value;
			if (field == PwDefs.UserNameField) entry.name = value;
			if (field == PwDefs.PasswordField) entry.password = value;

			var result = Util.GetKeepassEntry(entry, Fn(), new List<string>());

			Assert.IsEmpty(PwEntryIndexer.GetString(result, field));
		}

		[TestCase("", "", "", "", "", "")]
		[TestCase("", "Notes", "", "Notes", "", "")]
		[TestCase("rev", "Test: OK\nFoo: Bar\nNotes", "rev", "Notes", "OK", "Bar")]
		public void TestGetKeepassEntryExtraFields(string revin, string notein, string revexp, string noteexp, string testexp, string fooexp)
		{
			var entry = GetPfpEntryObject();
			entry.revision = revin;
			entry.notes = notein;

			var result = Util.GetKeepassEntry(entry, Fn(), new List<string> { });

			Assert.AreEqual(revexp, PwEntryIndexer.GetString(result, Util.RevisionField));
			Assert.AreEqual(noteexp, PwEntryIndexer.GetString(result, PwDefs.NotesField));
			Assert.AreEqual(testexp, PwEntryIndexer.GetString(result, "Test"));
			Assert.AreEqual(fooexp, PwEntryIndexer.GetString(result, "Foo"));

			if (testexp == "")
				Assert.IsFalse(result.Strings.Exists("Test"));
		}

		[Test]
		public void TestGetKeepassEntryAmbiguousFields()
		{
			var entry = new StoredEntry();

			entry.notes = string.Format("{0}: Tag1\n{1}: Ignore\n{2}: Ignore\n{3}: Ignore\n{4}: One Note\n{5}: Ignore\nMore Notes",
				Util.TagsField, PwDefs.UrlField, PwDefs.UserNameField, PwDefs.PasswordField, PwDefs.NotesField, Util.RevisionField);

			var resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { }), null);

			Assert.IsFalse(resultidx.entry.Strings.Exists(PwDefs.UrlField));
			Assert.IsFalse(resultidx.entry.Strings.Exists(PwDefs.UserNameField));
			Assert.IsFalse(resultidx.entry.Strings.Exists(PwDefs.PasswordField));
			Assert.IsFalse(resultidx.entry.Strings.Exists(Util.RevisionField));
			Assert.AreEqual(resultidx[PwDefs.NotesField], "One Note\r\nMore Notes");
			CollectionAssert.AreEqual(new string[] { "Tag1" }, resultidx.entry.Tags);

			entry.name = "user";
			resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { }), null);
			Assert.AreEqual("user", resultidx[PwDefs.UserNameField]);

			entry.revision = "new";
			resultidx = new PwEntryIndexer(Util.GetKeepassEntry(entry, Fn(), new List<string> { }), null);
			Assert.AreEqual("new", resultidx[Util.RevisionField]);
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

		[TestCase("example.com/test", ExpectedResult = "example.com")]
		[TestCase("http://example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://www.example.com/test", ExpectedResult = "example.com")]
		[TestCase("https://host.example.com/test", ExpectedResult = "host.example.com")]
		[TestCase("https://www.host.example.com/test", ExpectedResult = "host.example.com")]
		[TestCase("https://sub.host.example.com/test", ExpectedResult = "sub.host.example.com")]
		[TestCase("ftp://example.com/test", ExpectedResult = "example.com")]
		public string TestGetPfpEntryUrlField(string value)
		{
			var entry = GetKeepassEntryObject();
			entry[PwDefs.UrlField] = value;

			var result = Util.GetPfpEntry(entry.entry);

			return result.site;
		}

		[TestCase(null, null, "OK", "Bar", "", "Foo: Bar\nTest: OK")]
		[TestCase(null, "Hello: World\r\nTest: Fail\r\nNotes", "OK", "Bar", "", "Foo: Bar\nHello: World\nTest: OK\n\nNotes")]
		[TestCase(null, "Revision: rev", "", "", "rev", null)]
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

		[Test]
		public void TestGetPfpEntryAmbiguousFields()
		{
			var empty = new StoredEntry();
			var entry = GetKeepassEntryObject();
			entry.entry.Strings.Clear();

			entry[PwDefs.NotesField] = string.Format("{0}: Tag1\n{1}: Ignore\n{2}: Ignore\n{3}: Ignore\n{4}: One Note\n{5}: old\nMore Notes",
				Util.TagsField, PwDefs.UrlField, PwDefs.UserNameField, PwDefs.PasswordField, PwDefs.NotesField, Util.RevisionField);

			var result = Util.GetPfpEntry(entry.entry) as StoredEntry;

			Assert.AreEqual(empty.site, result.site);
			Assert.AreEqual(empty.name, result.name);
			Assert.AreEqual(empty.password, result.password);
			Assert.AreEqual("old", result.revision);
			Assert.AreEqual("Tags: Tag1\n\nOne Note\nMore Notes", result.notes);

			entry[PwDefs.UserNameField] = "user";
			result = Util.GetPfpEntry(entry.entry) as StoredEntry;
			Assert.AreEqual("user", result.name);

			entry[Util.RevisionField] = "new";
			result = Util.GetPfpEntry(entry.entry) as StoredEntry;
			Assert.AreEqual("new", result.revision);

			entry.entry.Tags.Add("Tag2");
			result = Util.GetPfpEntry(entry.entry) as StoredEntry;
			Assert.AreEqual("Tags: Tag1, Tag2\n\nOne Note\nMore Notes", result.notes);
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

		[TestCase("", "", new string[0], new string[0])]
		[TestCase(": Colon", "<SAME>", new string[0], new string[0])]
		[TestCase("Hello World", "<SAME>", new string[0], new string[0])]
		[TestCase("Relevant Hint: No dict for keys with spaces", "<SAME>", new string[0], new string[0])]
		[TestCase(" Key : Value : Pair  ", "", new string[] { "Key", "Value : Pair" }, new string[0])]
		[TestCase("Key: Old\nKey:Value \n\nNote", "Note", new string[] { "Key", "Value" }, new string[0])]
		[TestCase("Note1\nKey: Value\nEmpty:  \n\nNote2", "Note1\nNote2", new string[] { "Key", "Value" }, new string[0])]
		[TestCase("Note1\nKey: Value\nNotes: Note2\n\nNote3", "Note1\nNote2\nNote3", new string[] { "Key", "Value" }, new string[0])]
		[TestCase("Key: Value\n\nForce: Note", "Force: Note", new string[] { "Key", "Value" }, new string[0])]
		[TestCase("Key: Value\nTags: T1 , T2\n\nNote", "Note", new string[] { "Key", "Value" }, new string[] { "T1", "T2" })]
		public void TestParseNotes(string notein, string noteexp, string[] dictexpa, string[] listexpa)
		{
			var dictexp = ArrayToDict(dictexpa);
			var listexp = ArrayToList(listexpa);

			var dict = new Dictionary<string, string>();
			var list = new List<string>();

			var note = Util.ParseNotes(notein, dict, list);

			Assert.AreEqual(noteexp == "<SAME>" ? notein : noteexp, note);
			CollectionAssert.AreEquivalent(dictexp, dict);
			CollectionAssert.AreEquivalent(listexp, list);
		}

		[TestCase(new string[0], new string[0], "")]
		[TestCase(new string[] { "Key", "" }, new string[0], "")]
		[TestCase(new string[] { "Key ", "Value " }, new string[0], "Key: Value")]
		[TestCase(new string[] { "Hello World", "Foo\r\nBar\r\n" }, new string[0], "HelloWorld: Foo Bar")]
		[TestCase(new string[] { "Hello World", "Foo\r\nBar", PwDefs.NotesField, "This is\r\na note." }, new string[] { "T1", "T2" }, "Tags: T1, T2\nHelloWorld: Foo Bar\n\nThis is\na note.")]
		public void TestBuildNotes(string[] dictina, string[] listina, string noteexp)
		{
			var dictin = ArrayToDict(dictina);
			var listin = ArrayToList(listina);

			var note = Util.BuildNotes(dictin, listin);

			Assert.AreEqual(noteexp, note);
		}

		private StoredEntry GetPfpEntryObject()
		{
			return new StoredEntry
			{
				site = "example.com",
				name = "user",
				password = "secret",
				revision = ""
			};
		}

		private PwEntryIndexer GetKeepassEntryObject()
		{
			return new PwEntryIndexer(new PwEntry(true, true), new List<string> { }) {
				[PwDefs.UrlField] = "example.com",
				[PwDefs.UserNameField] = "user",
				[PwDefs.PasswordField] = "secret",
			};
		}

		private IDictionary<T, T> ArrayToDict<T>(T[] a)
		{
			var result = new Dictionary<T, T>();
			var i = 0;
			while (i < a.Length)
				result.Add(a[i++], a[i++]);
			return result;
		}

		private IList<T> ArrayToList<T>(T[] a)
		{
			return new List<T>(a);
		}

		private GetPassword Fn()
		{
			return entry => (entry as StoredEntry).password;
		}
	}
}
