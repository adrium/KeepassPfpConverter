using Adrium.KeepassPfpConverter.Objects;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassLib.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Adrium.KeepassPfpConverter
{
	public static class Util
	{
		private const string EmptyUrl = "pfp.invalid";
		private const string EmptyUsername = "(none)";
		private const string EmptyPassword = "X";
		private const string RevisionKey = "Revision";

		private delegate string StringGetter(string key);
		private delegate void StringSetter(string key, string value);

		public static PwEntry GetPwEntry(Crypto crypto, PassEntry entry, MemoryProtectionConfig protect)
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
				setter(PwDefs.UrlField, $"https://{entry.site}/");
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

		public static PassEntry GetPassEntry(Crypto crypto, ProtectedStringDictionary strings)
		{
			var result = new StoredEntry();
			var fields = new Dictionary<string, string>();

			result.type = "stored";
			result.name = EmptyUsername;
			result.password = EmptyPassword;
			result.site = EmptyUrl;
			result.revision = "";

			StringGetter getter = key =>
				fields.ContainsKey(key) ? fields[key].Equals("") ? null : fields[key] : null;

			foreach (var field in strings)
				fields.Add(field.Key, field.Value.ReadString());

			var value = "";
			if ((value = getter(PwDefs.UserNameField)) != null)
				result.name = value;

			if ((value = getter(PwDefs.PasswordField)) != null)
				result.password = value;

			if ((value = getter(PwDefs.UrlField)) != null) {
				var url = value;
				url = url.Replace("https://", "");
				url = url.Replace("http://", "");
				url = url.Replace("www.", "");
				if (url.IndexOf("/") >= 0)
					url = url.Substring(0, url.IndexOf("/"));
				if (!url.Equals(""))
					result.site = url;
			}

			var notes = "";
			if ((value = getter(PwDefs.NotesField)) != null)
				notes = ParseNotes(value, fields);

			if (fields.ContainsKey(RevisionKey))
				result.revision = fields[RevisionKey];

			fields.Remove(PwDefs.UserNameField);
			fields.Remove(PwDefs.PasswordField);
			fields.Remove(PwDefs.TitleField);
			fields.Remove(PwDefs.UrlField);
			fields.Remove(PwDefs.NotesField);
			fields.Remove(RevisionKey);

			if (fields.Count > 0) {
				foreach (var field in fields) {
					value = field.Value;
					value = StrUtil.NormalizeNewLines(value, false);
					value = value.Replace('\n', ' ');
					notes = $"{field.Key}: {value}\n{notes}";
				}
			}

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
	}
}
