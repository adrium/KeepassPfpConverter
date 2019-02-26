using System;
using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter
{
	public class Password
	{
		private const string LOWERCASE = "abcdefghjkmnpqrstuvwxyz";
		private const string UPPERCASE = "ABCDEFGHJKMNPQRSTUVWXYZ";
		private const string NUMBER = "23456789";
		private const string SYMBOL = "!#$%&()*+,-./:;<=>?@[]^_{|}~";

		private Crypto crypto;

		public Password(Crypto crypto)
		{
			this.crypto = crypto;
		}

		public static string ToPassword(byte[] array, bool lower, bool upper, bool number, bool symbol)
		{
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

		public string GetPassword(EntryObject entry)
		{
			string result;
			if (false) {
				/* switch */
			} else if (entry.type.Equals(EntryObject.Type.Stored)) {
				result = entry.password;
			} else if (entry.type.Equals(EntryObject.Type.Generated1)) {
				result = "⚠ Legacy entries not supported";
			} else if (entry.type.Equals(EntryObject.Type.Generated2)) {
				result = DerivePassword(entry);
			} else {
				result = "⚠ Unknown password type";
			}

			return result;
		}

		private string DerivePassword(EntryObject entry)
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
