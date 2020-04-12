using System.IO;
using Adrium.KeepassPfpConverter.Objects;
using KeePassLib;

namespace Adrium.KeepassPfpConverter.Test
{
	public static class Data
	{
		public static Stream Json => typeof(Data).Assembly.GetManifestResourceStream("data.passwords.json");

		public static SiteEntry SiteEntry => new SiteEntry {
			site = "example.com"
		};

		public static SiteEntry AliasEntry => new SiteEntry {
			site = "example.net",
			alias = "example.com"
		};

		public static StoredEntry StoredEntry => new StoredEntry {
			type = "stored",
			name = "stored",
			site = "example.com",
			revision = "old",
			password = "secret",
			notes = "foo: bar\n\nmessage"
		};

		public static string Generated2Password => "h2z}+tFz;8s^+uYk";

		public static GeneratedEntry Generated2Entry => new GeneratedEntry {
			type = "generated2",
			name = "user",
			site = "example.com",
			revision = "",
			length = 16,
			lower = true,
			upper = true,
			number = true,
			symbol = true
		};
	}
}
