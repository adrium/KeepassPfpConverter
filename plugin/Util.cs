using Adrium.KeepassPfpConverter.Objects;
using KeePassLib;
using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public static class Util
	{
		public const string RevisionField = "Revision";
		public const string TagsField = "Tags";

		public static PwEntry GetKeepassEntry(PassEntry entry, GetPassword getPassword, ICollection<string> protect)
		{
			var empty = new StoredEntry();
			var resultidx = new PwEntryIndexer(new PwEntry(true, true), protect);
			var fields = new Dictionary<string, string>();
			var tags = new List<string>();

			var notes = ParseNotes(entry.notes ?? "", fields, tags);
			var pw = getPassword(entry);

			fields[Util.RevisionField] = entry.revision.ToLower() == empty.revision ? "" : entry.revision;
			fields[PwDefs.UrlField] = entry.site.ToLower() == empty.site ? "" : entry.site;
			fields[PwDefs.UserNameField] = entry.name.ToLower() == empty.name ? "" : entry.name;
			fields[PwDefs.PasswordField] = pw.ToLower() == empty.password ? "" : pw;
			fields[PwDefs.NotesField] = notes == "" ? "" : notes.Replace("\n", "\r\n");
			fields[PwDefs.TitleField] = GetTitle(entry);

			foreach (var field in fields)
				if (field.Value != "")
					resultidx[field.Key] = field.Value;

			resultidx.entry.Tags.AddRange(tags);

			return resultidx.entry;
		}

		public static StoredEntry GetPfpEntry(PwEntry pwEntry)
		{
			var entry = new PwEntryIndexer(pwEntry, null);
			var empty = new StoredEntry();
			var result = new StoredEntry();

			var fields = new SortedDictionary<string, string>();
			var tags = new List<string>();

			fields.Add(RevisionField, "");

			var notes = ParseNotes(entry[PwDefs.NotesField], fields, tags);

			tags.AddRange(pwEntry.Tags);

			foreach (var field in pwEntry.Strings)
				fields[field.Key] = entry[field.Key];

			fields[PwDefs.NotesField] = notes;
			fields[PwDefs.UrlField] = entry[PwDefs.UrlField];
			fields[PwDefs.UserNameField] = entry[PwDefs.UserNameField];
			fields[PwDefs.PasswordField] = entry[PwDefs.PasswordField];

			result.site = fields[PwDefs.UrlField] == "" ? empty.site : GetSitePart(fields[PwDefs.UrlField]);
			result.name = fields[PwDefs.UserNameField] == "" ? empty.name : fields[PwDefs.UserNameField];
			result.password = fields[PwDefs.PasswordField] == "" ? result.password : fields[PwDefs.PasswordField];
			result.revision = fields[RevisionField] == "" ? empty.revision : fields[RevisionField];

			fields.Remove(PwDefs.TitleField);
			fields.Remove(PwDefs.UserNameField);
			fields.Remove(PwDefs.PasswordField);
			fields.Remove(PwDefs.UrlField);
			fields.Remove(RevisionField);

			notes = BuildNotes(fields, tags);
			result.notes = notes == "" ? empty.notes : notes;

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

		public static string ParseNotes(string notes, IDictionary<string, string> dict, IList<string> tags)
		{
			var result = "";
			var sect = notes.Replace("\r\n", "\n").Replace("\r", "\n")
				.Split(new string[] { "\n\n" }, 2, System.StringSplitOptions.None);

			foreach (var line in sect[0].Split('\n')) {
				var kv = line.Split(new char[] { ':' }, 2);
				var x = kv.Length == 1 ? "" : kv[0].Trim();
				var k = x.Contains(" ") ? "" : x;
				var v = (k == "" ? line : kv[1]).Trim();
				if (k == "" || k == PwDefs.NotesField)
					result += v + "\n";
				else if (k == TagsField)
					foreach (var tag in v.Split(','))
						tags.Add(tag.Trim());
				else if (v != "")
					dict[k] = v;
			}

			if (sect.Length == 2)
				result += sect[1];

			result = result.Trim();
			return result;
		}

		public static string BuildNotes(IDictionary<string, string> dict, IList<string> tags)
		{
			var notes = "";
			var kvstr = "";

			if (tags.Count > 0)
				kvstr += string.Format("{0}: {1}\n", TagsField, string.Join(", ", new List<string>(tags).ToArray()));

			if (dict.Count > 0) {
				foreach (var pair in dict) {
					var k = pair.Key.Replace(" ", "").Trim();
					var v = pair.Value.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
					if (k == PwDefs.NotesField)
						notes = v;
					else if (v != "")
						kvstr += string.Format("{0}: {1}\n", k, v.Replace("\n", " "));
				}
			}

			var result = (kvstr + "\n" + notes).Trim();
			return result;
		}

		private static string GetTitle(PassEntry entry)
		{
			var empty = new StoredEntry();
			var result = entry.name;
			if (entry.site != empty.site)
				result = string.Format("{0} - {1}", entry.site, result);
			if (entry.revision != empty.revision)
				result = string.Format("{0} #{1}", result, entry.revision);
			return result;
		}

		private static string GetSitePart(string url)
		{
			var result = url;
			var split = url.Split(new string[] { "://" }, System.StringSplitOptions.None);

			result = split.Length == 1 ? split[0] : split[1];
			result = result.Split('/')[0];
			result = result.Replace("www.", "");

			return result;
		}
	}
}
