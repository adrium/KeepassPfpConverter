using System;
using System.Collections.Generic;
using System.IO;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace Adrium.KeepassPfpConverter
{
	public static partial class PfpConvert
	{
		private const string APPLICATION = "pfp";
		private const int FORMAT = 2;

		private const string SALT_KEY = "salt";
		private const string HMAC_KEY = "hmac-secret";
		private const string STORAGE_PREFIX = "site:";

		public static IList<BaseEntry> Load(Crypto crypto, Stream stream)
		{
			DeserializedBackup backup;
			using (var reader = new StreamReader(stream)) {
				var str = reader.ReadToEnd();
				backup = JsonConvert.DeserializeObject<DeserializedBackup>(str);
			}

			if (!backup.application.Equals(APPLICATION))
				throw new ReaderException("Unsupported format");

			if (backup.format != FORMAT)
				throw new ReaderException("Unsupported version");

			var result = new List<BaseEntry>();

			string json;
			BaseEntry entry;

			var converter = new JsonConverter();
			crypto.SetSalt(backup.data[SALT_KEY]);

			foreach (var item in backup.data) {
				if (item.Key.Equals(SALT_KEY)) {
					continue; // ignore
				}

				if (item.Key.Equals(HMAC_KEY)) {
					json = crypto.Decrypt(item.Value);
					json = JsonConvert.DeserializeObject<string>(json);
					crypto.SetHmacSecret(json);
					continue;
				}

				try {
					json = crypto.Decrypt(item.Value);
					entry = JsonConvert.DeserializeObject<BaseEntry>(json, converter);
					result.Add(entry);
				} catch (InvalidCipherTextException e) {
					if (e.Message.Equals("mac check in GCM failed"))
						throw new ReaderException("Wrong master password");
					else
						throw e;
				}
			}

			return result;

		}

		public class ReaderException : Exception
		{
			public ReaderException(string message) : base(message) { }
		}

		private class DeserializedBackup
		{
			#pragma warning disable 649
			public string application;
			public int format;
			public IDictionary<string, string> data;
			#pragma warning restore 649
		}
	}
}
