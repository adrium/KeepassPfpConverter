using KeePassLib;
using KeePassLib.Security;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public class PwEntryIndexer
	{
		public readonly PwEntry entry;
		public readonly MemoryProtectionConfig protect;

		public PwEntryIndexer(PwEntry entry) : this(entry, new MemoryProtectionConfig()) { }

		public PwEntryIndexer(PwEntry entry, MemoryProtectionConfig protect)
		{
			this.entry = entry;
			this.protect = protect;
		}

		public string this[string k] {
			get => entry.Strings.Get(k)?.ReadString();
			set => entry.Strings.Set(k, new ProtectedString(protect.GetProtection(k), value));
		}
	}
}
