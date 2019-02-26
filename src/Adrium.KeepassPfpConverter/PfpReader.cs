using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter
{
	public class PfpReader
	{
		private readonly string masterPassword;

		public PfpReader(string masterPassword)
		{
			this.masterPassword = masterPassword;
		}

		public IList<EntryObject> GetEntries(string backupJson)
		{
			var backup = JsonConvert.DeserializeObject<BackupObject>(backupJson);

			if (!backup.application.Equals("pfp"))
				throw new ReaderException("Unsupported format");

			if (backup.format != 2)
				throw new ReaderException("Unsupported version");

			var crypto = new Crypto(masterPassword, backup.data["salt"]);
			var password = new Password(crypto);

			var result = new List<EntryObject>();

			foreach (var item in backup.data) {
				if (!item.Key.StartsWith("site:"))
					continue;

				try {
					var json = crypto.Decrypt(item.Value);
					var entry = JsonConvert.DeserializeObject<EntryObject>(json);

					if (entry.type != null)
						entry.password = password.GetPassword(entry);

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
	}
}
