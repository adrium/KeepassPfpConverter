using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Security;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public class PwEntryIndexer
	{
		public readonly PwEntry entry;
		public readonly ICollection<string> protect;

		public PwEntryIndexer(PwEntry entry, ICollection<string> protect = null)
		{
			if (protect == null)
				protect = new List<string> { PwDefs.PasswordField };
			this.entry = entry;
			this.protect = protect;
		}

		public string this[string k] {
			get => entry.Strings.Get(k)?.ReadString();
			set => entry.Strings.Set(k, new ProtectedString(protect.Contains(k), value));
		}
	}
}
