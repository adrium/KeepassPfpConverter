using System.Collections.Generic;
using System.IO;
using Adrium.KeepassPfpConverter.Algo;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace Adrium.KeepassPfpConverter
{
	public partial class PfpConvert
	{
		private const string MAC_MESSAGE = "mac check in GCM failed";

		public IList<BaseEntry> Load(Stream stream, string password)
		{
			DeserializedBackup backup;

			var json = new JsonConvert();
			var reader = new StreamReader(stream);

			try {
				backup = json.Deserialize<DeserializedBackup>(reader.ReadToEnd());
			} catch (JsonSerializationException) {
				throw new PfpConvertException("Not a PFP file");
			}

			if (! PFP_APPLICATION.Equals(backup.application))
				throw new PfpConvertException($"Unsupported application {backup.application}");

			if (! (backup.format == 2 || backup.format == 3))
				throw new PfpConvertException($"Unsupported format version {backup.format}");

			if (backup.data == null || !backup.data.ContainsKey(SALT_KEY) || !backup.data.ContainsKey(HMAC_KEY))
				throw new PfpConvertException($"Invalid format");

			string str;

			var hasher = new HasherV2(password);
			var cipher = new CipherV1(hasher.Hash, backup.data[SALT_KEY]);
			var decrypt = GetDecryptFn(cipher);

			try {
				str = decrypt(backup.data[HMAC_KEY]);
			} catch (InvalidCipherTextException e) {
				if (e.Message.Equals(MAC_MESSAGE))
					throw new PfpConvertException("Wrong master password");
				else
					throw;
			}

			backup.data.Remove(SALT_KEY);
			backup.data.Remove(HMAC_KEY);

			var result = new List<BaseEntry>();

			foreach (var item in backup.data) {
				if (!item.Key.StartsWith(STORAGE_PREFIX))
					continue;

				str = decrypt(item.Value);
				result.Add(json.Deserialize<BaseEntry>(str));
			}

			return result;
		}

		private Cipher GetDecryptFn(CipherV1 cipher)
		{
			return data => {
				var arr = data.Split('_');
				return cipher.Process(false, arr[0], arr[1]);
			};
		}
	}
}
