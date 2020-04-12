using System;
using System.Collections.Generic;
using System.IO;
using Adrium.KeepassPfpConverter.Objects;
using Newtonsoft.Json;

namespace Adrium.KeepassPfpConverter
{
	class Program
	{
		public static void Main(string[] args)
		{
			var commands = new List<Command>();
			commands.Add(new Command { Name = "decrypt", Args = 3, Usage = "<file> <masterPassword> <output>", Cmd = DecryptCommand });
			commands.Add(new Command { Name = "encrypt", Args = 3, Usage = "<file> <masterPassword> <output>", Cmd = EncryptCommand });

			var cmdname = args.Length < 1 ? "" : args[0];
			var command = commands.Find(x => x.Name.Equals(cmdname));

			if (command == default(Command)) {
				PrintHelp(commands);
				return;
			}

			var cmdargs = new string[args.Length - 1];
			Array.Copy(args, 1, cmdargs, 0, cmdargs.Length);

			if (cmdargs.Length < command.Args) {
				PrintHelp(commands);
				return;
			}

			try {
				command.Cmd(cmdargs);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		private static void EncryptCommand(string[] args)
		{
			var json = new JsonConvert();
			var pfp = new PfpConvert();

			using (var input = new StreamReader(args[0]))
			using (var output = File.OpenWrite(args[2])) {
				var entries = json.Deserialize<BaseEntry[]>(input.ReadToEnd());
				pfp.Save(output, args[1], entries);
			}
		}

		private static void DecryptCommand(string[] args)
		{
			var json = new JsonConvert(new JsonSerializerSettings { Formatting = Formatting.Indented });
			var pfp = new PfpConvert();

			using (var input = File.OpenRead(args[0]))
			using (var output = new StreamWriter(args[2])) {
				var entries = pfp.Load(input, args[1]);
				output.WriteLine(json.Serialize(entries));
			}
		}

		private static void PrintHelp(IList<Command> commands)
		{
			var exe = GetExe();
			Console.WriteLine("USAGE:");
			foreach (var command in commands)
				Console.WriteLine("{0} {1} {2}", exe, command.Name, command.Usage);
		}

		private static string GetExe()
		{
			var path = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.Location;
			var result = Path.GetFileName(path);
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
