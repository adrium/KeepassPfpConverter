using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class StringifierVaep
	{
		// private const string SYMBOL = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
		// No quotes, slashes, parens, dots
		// Only Android LatinIME primary symbols, truncate to length 10
		private const string LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
		private const string UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string NUMBER = "0123456789";
		private const string SYMBOL = "!#$%&*+-?@";

		public string Stringify(string bytes, bool lower, bool upper, bool number, bool symbol)
		{
			var charsets = new List<StringifierUniversal.Settings>();

			if (lower)
				charsets.Add(new StringifierUniversal.Settings(LOWERCASE, 2));
			if (upper)
				charsets.Add(new StringifierUniversal.Settings(UPPERCASE, 2));
			if (number)
				charsets.Add(new StringifierUniversal.Settings(NUMBER, 2));
			if (symbol)
				charsets.Add(new StringifierUniversal.Settings(SYMBOL, 2));

			return new StringifierUniversal(charsets).Stringify(bytes);
		}
	}
}
