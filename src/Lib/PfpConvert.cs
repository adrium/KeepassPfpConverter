using System;
using System.Collections.Generic;
using Adrium.KeepassPfpConverter.Algo;
using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter
{
	public partial class PfpConvert
	{
		private const string SALT_KEY = "salt";
		private const string HMAC_KEY = "hmac-secret";
		private const string STORAGE_PREFIX = "site:";
		private const string PFP_APPLICATION = "pfp";

		private delegate string Cipher(string data);

		public GetPassword GetPasswordGetter(string password)
		{
			var hasher = new HasherV2(password);
			var getgenpw = new PasswordGenerator(hasher.Hash, new StringifierV1().Stringify);

			return entry => {
				if (entry is StoredEntry stored)
					return stored.password;

				if (entry is GeneratedEntry gen)
					if (GENERATEDV2_TYPE.Equals(gen.type))
						return getgenpw.Get(gen);

				throw new ArgumentException($"Unsupported entry for {entry.name}@{entry.site}");
			};
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
