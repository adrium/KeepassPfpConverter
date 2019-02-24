using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Adrium.KeepassPfpConverter
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length < 3) {
				PrintHelp();
				return;
			}

			var cmdargs = new string[args.Length - 1];
			Array.Copy(args, 1, cmdargs, 0, cmdargs.Length);

			try {
				DecryptCommand(cmdargs);
			} catch (Org.BouncyCastle.Crypto.InvalidCipherTextException e) {
				LogError("Decryption not successful. Check master password.\nException: " + e.Message);
			}
		}

		private static void DecryptCommand(string[] args)
		{
			using (var input = new StreamReader(args[0]))
			using (var output = new StreamWriter(args[2]))
			{
				var str = input.ReadToEnd();
				var backup = JsonConvert.DeserializeObject<BackupObject>(str);

				var crypto = new Crypto(args[1], backup.data["salt"]);

				var decrypted = new Dictionary<string, EntryObject>();

				foreach (var item in backup.data) {
					if (!item.Key.StartsWith("site:"))
						continue;

					var json = crypto.Decrypt(item.Value);
					var entry = JsonConvert.DeserializeObject<EntryObject>(json);

					decrypted.Add(item.Key, entry);
				}

				var outputjson = JsonConvert.SerializeObject(decrypted, Formatting.Indented);
				output.WriteLine(outputjson);
			}
		}

		private static void PrintHelp()
		{
			var path = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.Location;
			var exe = System.IO.Path.GetFileName(path);

			Console.WriteLine("{0} decrypt <file> <masterPassword> <output>", exe);
		}

		private static void LogError(string msg)
		{
			Console.WriteLine("\u001b[31m" + msg + "\u001b[0m");
		}
	}
}
