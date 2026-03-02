using System.IO;

namespace Adrium.KeepassPfpConverter.Test
{
	public static class Data
	{
		public static Stream Invalid => typeof(Data).Assembly.GetManifestResourceStream("invalid.json");
		public static Stream Json => typeof(Data).Assembly.GetManifestResourceStream("passwords.json");
	}
}
