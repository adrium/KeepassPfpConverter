using Adrium.KeepassPfpConverter.Objects;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class PasswordGenerator
	{
		private readonly Hash Hash;
		private readonly Stringify Stringify;

		public PasswordGenerator(Hash hash, Stringify stringify)
		{
			Hash = hash;
			Stringify = stringify;
		}

		public string Get(GeneratedEntry entry)
		{
			var salt = entry.site + "\0" + entry.name;

			if (!string.IsNullOrEmpty(entry.revision))
				salt += "\0" + entry.revision;

			var hash = Hash(salt, entry.length);
			var result = Stringify(hash, entry.lower, entry.upper, entry.number, entry.symbol);
			return result;
		}
	}
}
