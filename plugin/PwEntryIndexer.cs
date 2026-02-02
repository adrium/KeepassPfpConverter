using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Security;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public class PwEntryIndexer
	{
		public readonly ProtectedString empty;
		public readonly PwEntry entry;
		public readonly ICollection<string> protect;

		public PwEntryIndexer(PwEntry entry, ICollection<string> protect)
		{
			this.empty = new ProtectedString(false, "");
			this.entry = entry;
			this.protect = protect;
		}

		public string this[string k] {
			get { return (entry.Strings.Get(k) ?? empty).ReadString(); }
			set { entry.Strings.Set(k, new ProtectedString(protect.Contains(k), value)); }
		}
	}
}
