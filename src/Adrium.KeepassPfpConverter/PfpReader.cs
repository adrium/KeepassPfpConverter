using Newtonsoft.Json;
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

			var crypto = new Crypto(masterPassword, backup.data["salt"]);
			var password = new Password(crypto);

			var result = new List<EntryObject>();

			foreach (var item in backup.data) {
				if (!item.Key.StartsWith("site:"))
					continue;

				var json = crypto.Decrypt(item.Value);
				var entry = JsonConvert.DeserializeObject<EntryObject>(json);

				if (entry.type != null)
					entry.password = password.GetPassword(entry);

				result.Add(entry);
			}

			return result;
		}
	}
}
