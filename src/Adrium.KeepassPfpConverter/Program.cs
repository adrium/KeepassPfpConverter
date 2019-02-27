using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Adrium.KeepassPfpConverter
{
	class Program
	{
		public static void Main(string[] args)
		{
			var commands = new List<Command>();
			commands.Add(new Command { Name = "decrypt", Args = 3, Usage = "<file> <masterPassword> <output>", Cmd = DecryptCommand });
			commands.Add(new Command { Name = "form", Args = 0, Usage = "", Cmd = ShowFormCommand });

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
			try {
				DecryptCommandImpl(args);
			} catch (PfpConvert.ReaderException e) {
				Console.WriteLine(e.Message);
			}
		}

		private static void ShowFormCommand(string[] args)
		{
			var form = new OptionForm();
			if (form.ShowDialog() == DialogResult.OK) {
				Console.WriteLine("Master password = {0}", form.MasterPassword);
			}
		}

		private static void DecryptCommandImpl(string[] args)
		{
			var jsonsettings = new JsonSerializerSettings {
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore,
			};

			using (var input = File.OpenRead(args[0]))
			using (var output = new StreamWriter(args[2]))
			{
				var crypto = new Crypto();
				crypto.SetMasterPassword(args[1]);

				var entries = PfpConvert.Load(crypto, input);
				var outputjson = JsonConvert.SerializeObject(entries, jsonsettings);
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
