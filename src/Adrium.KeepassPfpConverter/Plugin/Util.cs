using Adrium.KeepassPfpConverter.Objects;
using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public static class Util
	{
		private const string EmptyUrl = "pfp.invalid";
		private const string EmptyUsername = "(none)";
		private const string EmptyPassword = "X";
		private const string RevisionKey = "Revision";

		private delegate string StringGetter(string key);
		private delegate void StringSetter(string key, string value);

		public static PwEntry GetKeepassEntry(Crypto crypto, PassEntry entry, MemoryProtectionConfig protect)
		{
			var result = new PwEntry(true, true);
			var strings = result.Strings;

			StringSetter setter = (key, value) =>
				strings.Set(key, new ProtectedString(protect.GetProtection(key), value));

			var pw = Password.GetPassword(crypto, entry);
			if (!pw.Equals(EmptyPassword))
				setter(PwDefs.PasswordField, pw);

			if (!entry.name.Equals(EmptyUsername))
				setter(PwDefs.UserNameField, entry.name);

			if (entry.site.Equals(EmptyUrl)) {
				setter(PwDefs.TitleField, entry.name);
			} else {
				setter(PwDefs.TitleField, entry.site);
				setter(PwDefs.UrlField, entry.site);
			}

			var fields = new Dictionary<string, string>();
			var notes = ParseNotes(entry.notes ?? "", fields);

			if (!string.IsNullOrEmpty(entry.revision))
				notes = $"{RevisionKey}: {entry.revision}\n{notes}";

			notes = notes.Trim();
			notes = StrUtil.NormalizeNewLines(notes, true);

			if (!notes.Equals(""))
				setter(PwDefs.NotesField, notes);

			foreach (var field in fields)
				setter(field.Key, field.Value);

			return result;
		}

		public static PassEntry GetPfpEntry(Crypto crypto, PwEntry pwEntry)
		{
			var result = new StoredEntry();
			var fields = new Dictionary<string, string>();
			var value = "";

			foreach (var field in pwEntry.Strings) {
				var key = field.Key;
				value = field.Value.ReadString();

				if (key.Equals(PwDefs.UrlField))
					value = GetSitePart(value);

				if (key.Equals(PwDefs.NotesField))
					value = ParseNotes(value, fields);

				if (value.Equals(""))
					continue;

				fields.Add(key, value);
			}

			StringGetter getter = key =>
				fields.ContainsKey(key) ? fields[key] : null;

			var notes = "";
			var textnotes = getter(PwDefs.NotesField) ?? "";

			result.type = "stored";
			result.name = getter(PwDefs.UserNameField) ?? EmptyUsername;
			result.password = getter(PwDefs.PasswordField) ?? EmptyPassword;
			result.site = getter(PwDefs.UrlField) ?? EmptyUrl;
			result.revision = getter(RevisionKey) ?? "";

			fields.Remove(PwDefs.TitleField);
			fields.Remove(PwDefs.UserNameField);
			fields.Remove(PwDefs.PasswordField);
			fields.Remove(PwDefs.UrlField);
			fields.Remove(PwDefs.NotesField);
			fields.Remove(RevisionKey);

			foreach (var field in fields) {
				value = field.Value;
				value = StrUtil.NormalizeNewLines(value, false);
				value = value.Replace('\n', ' ');
				notes += $"{field.Key}: {value}\n";
			}

			notes += "\n" + textnotes;
			notes = notes.Trim();
			notes = StrUtil.NormalizeNewLines(notes, false);

			if (!notes.Equals(""))
				result.notes = notes;

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
					if (match.Success) {
						dict.Add(match.Groups[1].Value, match.Groups[2].Value);
					} else {
						parsing = false;
					}
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
