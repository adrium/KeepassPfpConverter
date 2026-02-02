using System.IO;
using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;

namespace Adrium.KeepassPfpConverter.Plugin
{
	partial class FormatProvider : FileFormatProvider
	{
		public override bool SupportsImport => true;
		public override bool SupportsExport => true;
		public override bool ImportAppendsToRootGroupOnly => true;
		public override bool RequiresFile => true;
		public override string FormatName => "Pain-free Passwords backup";
		public override string DefaultExtension => "json";
		public override string ApplicationGroup => KeePass.Resources.KPRes.PasswordManagers;
	}
}
