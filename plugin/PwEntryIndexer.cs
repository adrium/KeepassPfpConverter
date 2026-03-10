using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Security;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public class PwEntryIndexer
	{
		public readonly PwEntry entry;
		public readonly ICollection<string> protect;

		public PwEntryIndexer(PwEntry entry, ICollection<string> protect)
		{
			this.entry = entry;
			this.protect = protect;
		}

		public string this[string k] {
			get { return GetString(entry, k); }
			set { entry.Strings.Set(k, new ProtectedString(protect.Contains(k), value)); }
		}

		public static string GetString(PwEntry entry, string k)
		{
			return (entry.Strings.Get(k) ?? ProtectedString.Empty).ReadString();
		}
	}
}
