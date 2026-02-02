using System.Collections.Generic;

namespace Adrium.KeepassPfpConverter.Algo
{
	public class StringifierV1
	{
		private const string LOWERCASE = "abcdefghjkmnpqrstuvwxyz";
		private const string UPPERCASE = "ABCDEFGHJKMNPQRSTUVWXYZ";
		private const string NUMBER = "23456789";
		private const string SYMBOL = "!#$%&()*+,-./:;<=>?@[]^_{|}~";

		public string Stringify(string bytes, bool lower, bool upper, bool number, bool symbol)
		{
			var charsets = new List<StringifierUniversal.Settings>();

			if (lower)
				charsets.Add(new StringifierUniversal.Settings(LOWERCASE, 1));
			if (upper)
				charsets.Add(new StringifierUniversal.Settings(UPPERCASE, 1));
			if (number)
				charsets.Add(new StringifierUniversal.Settings(NUMBER, 1));
			if (symbol)
				charsets.Add(new StringifierUniversal.Settings(SYMBOL, 1));

			return new StringifierUniversal(charsets).Stringify(bytes);
		}
	}
}
