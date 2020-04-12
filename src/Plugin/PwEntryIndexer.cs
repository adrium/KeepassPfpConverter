using KeePassLib;
using KeePassLib.Security;

namespace Adrium.KeepassPfpConverter
{
	public class PwEntryIndexer
	{
		private PwEntry entry;
		private MemoryProtectionConfig protect;

		public PwEntryIndexer(PwEntry entry, MemoryProtectionConfig protect)
		{
			this.entry = entry;
			this.protect = protect;
		}

		public string this[string k]
		{
			get {
				return entry.Strings.Get(k)?.ReadString();
			}
			set {
				entry.Strings.Set(k, new ProtectedString(protect.GetProtection(k), value));
			}
		}
	}
}
