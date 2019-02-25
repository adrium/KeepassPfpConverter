using System;
namespace Adrium.KeepassPfpConverter
{
	public class EntryObject
	{
		public string type;
		public string site;
		public string name;
		public string password;
		public string revision;
		public string notes;

		public int length;
		public bool lower;
		public bool upper;
		public bool number;
		public bool symbol;

		public static class Type
		{
			public const string Stored = "stored";
			public const string Generated1 = "generated";
			public const string Generated2 = "generated2";
		}
	}
}
