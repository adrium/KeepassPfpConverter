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
			var commands = new List<Command>();
			commands.Add(new Command { Name = "decrypt", Args = 3, Usage = "<file> <masterPassword> <output>", Cmd = DecryptCommand });

			var cmdname = args.Length < 1 ? "" : args[0];
			var command = commands.Find(x => x.Name.Equals(cmdname));

			if (command == default(Command)) {
				PrintHelp(commands);
				return;
			}

			var cmdargs = new string[args.Length - 1];
			Array.Copy(args, 1, cmdargs, 0, cmdargs.Length);

			if (cmdargs.Length < command.Args) {
				PrintHelp(command);
				return;
			}

			command.Cmd(cmdargs);
		}

		private static void DecryptCommand(string[] args)
		{
			string error = null;
			Exception e = null;

			try {
				DecryptCommandImpl(args);
			} catch (Org.BouncyCastle.Crypto.InvalidCipherTextException ex) {
				e = ex;
				error = "Decryption not successful. Check master password.";
			}

			if (e != null) {
				Console.WriteLine("ERROR: {0}", error);
				Console.WriteLine("Exception: {0}", e.Message);
			}
		}

		private static void DecryptCommandImpl(string[] args)
		{
			using (var input = new StreamReader(args[0]))
			using (var output = new StreamWriter(args[2]))
			{
				var masterPassword = args[1];
				var str = input.ReadToEnd();

				var pfpreader = new PfpReader(masterPassword);
				var entries = pfpreader.GetEntries(str);
				var outputjson = JsonConvert.SerializeObject(entries, Formatting.Indented);
				output.WriteLine(outputjson);
			}
		}

		private static void PrintHelp(IList<Command> commands)
		{
			var exe = GetExe();
			Console.WriteLine("USAGE:");
			foreach (var command in commands)
				Console.WriteLine("{0} {1} {2}", exe, command.Name, command.Usage);
		}

		private static void PrintHelp(Command command)
		{
			var exe = GetExe();
			Console.WriteLine("USAGE:");
			Console.WriteLine("{0} {1} {2}", exe, command.Name, command.Usage);
		}

		private static string GetExe()
		{
			var path = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.Location;
			var result = System.IO.Path.GetFileName(path);
			return result;
		}

		private class Command
		{
			public string Name;
			public int Args;
			public Action<string[]> Cmd;
			public string Usage;
		}
	}
}
