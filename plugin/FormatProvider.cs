using System.IO;
using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;

namespace Adrium.KeepassPfpConverter.Plugin
{
	partial class FormatProvider : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return true; } }
		public override bool ImportAppendsToRootGroupOnly { get { return true; } }
		public override bool RequiresFile { get { return true; } }
		public override string FormatName { get { return "Pain-free Passwords backup"; } }
		public override string DefaultExtension { get { return "json"; } }
		public override string ApplicationGroup { get { return KeePass.Resources.KPRes.PasswordManagers; } }
	}
}
