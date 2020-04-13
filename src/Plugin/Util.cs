using Adrium.KeepassPfpConverter.Objects;
using KeePassLib;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public static class Util
	{
		public const string RevisionField = "Revision";

		private const string EmptyUrl = "pfp.invalid";
		private const string EmptyUsername = "(none)";
		private const string EmptyPassword = "X";

		public static PwEntry GetKeepassEntry(PassEntry entry, GetPassword getPassword, ICollection<string> protect)
		{
			var resultidx = new PwEntryIndexer(new PwEntry(true, true), protect);
			var fields = new Dictionary<string, string>();
			var notes = ParseNotes(entry.notes ?? "", fields);

			foreach (var field in fields)
				resultidx[field.Key] = field.Value;

			var pw = getPassword(entry);

			if (!pw.Equals(EmptyPassword))
				resultidx[PwDefs.PasswordField] = pw;

			if (!entry.name.Equals(EmptyUsername))
				resultidx[PwDefs.UserNameField] = entry.name;

			if (!entry.revision.Equals(""))
				resultidx[RevisionField] = entry.revision;

			if (entry.site.Equals(EmptyUrl)) {
				resultidx[PwDefs.TitleField] = entry.name;
			} else {
				resultidx[PwDefs.TitleField] = entry.site;
				resultidx[PwDefs.UrlField] = entry.site;
			}

			notes = notes.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n").Trim();

			if (!notes.Equals(""))
				resultidx[PwDefs.NotesField] = notes;

			return resultidx.entry;
		}

		public static PassEntry GetPfpEntry(PwEntry pwEntry)
		{
			var entry = new PwEntryIndexer(pwEntry);
			var result = new StoredEntry();
			var fields = new SortedDictionary<string, string> {
				{ PwDefs.UserNameField, EmptyUsername },
				{ PwDefs.PasswordField, EmptyPassword },
				{ PwDefs.UrlField, EmptyUrl },
				{ PwDefs.NotesField, "" },
				{ RevisionField, "" }
			};

			result.notes = "%fields%";
			result.notes += ParseNotes(entry[PwDefs.NotesField] ?? "", fields)
				.Replace("\r\n", "\n").Replace("\r", "\n");

			foreach (var field in pwEntry.Strings)
				if (!entry[field.Key].Equals(""))
					fields[field.Key] = entry[field.Key];

			result.site = fields[PwDefs.UrlField];
			result.name = fields[PwDefs.UserNameField];
			result.password = fields[PwDefs.PasswordField];
			result.revision = fields[RevisionField];

			fields.Remove(PwDefs.TitleField);
			fields.Remove(PwDefs.UserNameField);
			fields.Remove(PwDefs.PasswordField);
			fields.Remove(PwDefs.UrlField);
			fields.Remove(PwDefs.NotesField);
			fields.Remove(RevisionField);

			foreach (var field in fields) {
				var value = field.Value.Replace("\r\n", " ").Replace("\n", " ");
				result.notes = result.notes.Replace("%fields%", $"{field.Key}: {value}\n%fields%");
			}

			result.site = GetSitePart(result.site);
			result.notes = result.notes.Replace("%fields%", "\n").Trim();

			if (result.notes.Equals(""))
				result.notes = null;

			return result;
		}

		public static ICollection<string> GetMemoryProtection(MemoryProtectionConfig protect)
		{
			var result = new List<string>();

			if (protect.ProtectNotes) result.Add(PwDefs.NotesField);
			if (protect.ProtectPassword) result.Add(PwDefs.PasswordField);
			if (protect.ProtectTitle) result.Add(PwDefs.TitleField);
			if (protect.ProtectUrl) result.Add(PwDefs.UrlField);
			if (protect.ProtectUserName) result.Add(PwDefs.UserNameField);

			return result;
		}

		private static string ParseNotes(string str, IDictionary<string, string> dict)
		{
			var parsing = true;
			var reader = new StringReader(str);
			var matcher = new Regex("([^:]+): (.+)");
			var result = "";

			string line;
			while ((line = reader.ReadLine()) != null) {
				if (parsing) {
					var match = matcher.Match(line);
					if (match.Success)
						dict[match.Groups[1].Value] = match.Groups[2].Value;
					else
						parsing = false;
				}

				if (!parsing)
					result += line + "\n";
			}

			return result;
		}

		private static string GetSitePart(string url)
		{
			var result = url;

			result = result.Replace("https://", "");
			result = result.Replace("http://", "");
			result = result.Replace("www.", "");

			if (result.IndexOf("/") >= 0)
				result = result.Substring(0, result.IndexOf("/"));

			return result;
		}
	}
}
