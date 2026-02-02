using System.Collections.Generic;
using System.IO;
using Adrium.KeepassPfpConverter.Algo;
using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter
{
	public partial class PfpConvert
	{
		private const int IV_LENGTH = 12;
		private const int HMAC_LENGTH = 32;
		private const int SALT_LENGTH = 16;

		public void Save(Stream stream, string password, IList<BaseEntry> entries)
		{
			var backup = new DeserializedBackup {
				application = PFP_APPLICATION,
				format = 2,
				data = new Dictionary<string, string>()
			};

			entries = Util.GenerateSiteEntries(entries);

			var salt = Util.GenerateRandom(SALT_LENGTH);
			var hmac = Util.GenerateRandom(HMAC_LENGTH);

			var json = new JsonConvert();
			var hasher = new HasherV2(password);
			var digest = new DigesterV1(hmac);
			var cipher = new CipherV1(hasher.Hash, salt);
			var encrypt = GetEncryptFn(cipher);

			backup.data.Add(SALT_KEY, salt);
			backup.data.Add(HMAC_KEY, encrypt(json.Serialize(hmac)));

			foreach (var entry in entries) {
				var data = encrypt(json.Serialize(entry));

				var key = STORAGE_PREFIX + digest.Digest(entry.site);

				if (entry is PassEntry pass)
					key += ":" + digest.Digest(pass.site + "\0" + pass.name + "\0" + pass.revision);

				backup.data.Add(key, data);
			}

			var writer = new StreamWriter(stream);
			var str = json.Serialize(backup);
			writer.Write(str);
			writer.Flush();
		}

		private Cipher GetEncryptFn(CipherV1 cipher)
		{
			return data => {
				var iv = Util.GenerateRandom(IV_LENGTH);
				var str = cipher.Process(true, iv, data);
				return iv + "_" + str;
			};
		}
	}
}
