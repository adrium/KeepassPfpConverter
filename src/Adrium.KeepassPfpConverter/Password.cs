using System;
using System.Collections.Generic;
using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter
{
	public static class Password
	{
		private const string GENERATED_TYPE_2 = "generated2";

		private const string LOWERCASE = "abcdefghjkmnpqrstuvwxyz";
		private const string UPPERCASE = "ABCDEFGHJKMNPQRSTUVWXYZ";
		private const string NUMBER = "23456789";
		private const string SYMBOL = "!#$%&()*+,-./:;<=>?@[]^_{|}~";

		public static string ToPassword(string bytes, bool lower, bool upper, bool number, bool symbol)
		{
			var array = Convert.FromBase64String(bytes);
			var charsets = new List<string>();

			if (lower)
				charsets.Add(LOWERCASE);
			if (upper)
				charsets.Add(UPPERCASE);
			if (number)
				charsets.Add(NUMBER);
			if (symbol)
				charsets.Add(SYMBOL);

			var numChars = 0;
			foreach (var chars in charsets)
				numChars += chars.Length;

			var seen = new List<string>();

			var result = new char[array.Length];

			for (var i = 0; i < array.Length; i++) {
				if (charsets.Count - seen.Count >= array.Length - i) {
					foreach (var value in seen) {
						charsets.Remove(value);
					}
					seen.Clear();
					numChars = 0;
					foreach (var chars in charsets)
						numChars += chars.Length;
				}

				var index = array[i] % numChars;
				foreach (var charset in charsets) {
					if (index < charset.Length) {
						result[i] = charset[index];
						if (!seen.Contains(charset))
							seen.Add(charset);
						break;
					}
					index -= charset.Length;
				}
			}
			return new string(result);
		}

		public static string GetPassword(Crypto crypto, PassEntry entry)
		{
			var result = "⚠ Unsupported password type";

			if (entry is StoredEntry stored)
				result = stored.password;

			if (entry is GeneratedEntry generated)
				if (entry.type.Equals(GENERATED_TYPE_2))
					result = GeneratePassword2(crypto, generated);

			return result;
		}

		public static string GeneratePassword2(Crypto crypto, GeneratedEntry entry)
		{
			var salt = entry.site + "\0" + entry.name;
			if (!string.IsNullOrEmpty(entry.revision))
				salt += "\0" + entry.revision;

			var hash = crypto.Hash(salt, entry.length);
			var result = ToPassword(hash, entry.lower, entry.upper, entry.number, entry.symbol);

			return result;
		}
	}
}
