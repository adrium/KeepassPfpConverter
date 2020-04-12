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

		public static PwEntry GetKeepassEntry(Crypto crypto, PassEntry entry, MemoryProtectionConfig protect)
		{
			var result = new PwEntry(true, true);
			var resultidx = new PwEntryIndexer(result, protect);

			var pw = Password.GetPassword(crypto, entry);

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

			var fields = new Dictionary<string, string>();
			var notes = ParseNotes(entry.notes ?? "", fields);

			notes = notes.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n").Trim();

			if (!notes.Equals(""))
				resultidx[PwDefs.NotesField] = notes;

			foreach (var field in fields)
				resultidx[field.Key] = field.Value;

			return result;
		}

		public static PassEntry GetPfpEntry(Crypto crypto, PwEntry pwEntry)
		{
			var result = new StoredEntry();
			var fields = new SortedDictionary<string, string> {
				{ PwDefs.UserNameField, EmptyUsername },
				{ PwDefs.PasswordField, EmptyPassword },
				{ PwDefs.UrlField, EmptyUrl },
				{ PwDefs.NotesField, "" },
				{ RevisionField, "" }
			};

			foreach (var field in pwEntry.Strings)
				if (!field.Value.ReadString().Equals(""))
					fields[field.Key] = field.Value.ReadString();

			result.notes = "%fields%";
			result.notes += ParseNotes(fields[PwDefs.NotesField], fields)
				.Replace("\r\n", "\n").Replace("\r", "\n");

			result.type = "stored";
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
			result.notes = result.notes.Replace("%fields%", "").Trim();

			if (result.notes.Equals(""))
				result.notes = null;

			return result;
		}

		public static string ParseNotes(string str, IDictionary<string, string> dict)
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

		public static string GetSitePart(string url)
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
