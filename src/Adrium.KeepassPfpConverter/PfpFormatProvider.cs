using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;
using System.IO;

namespace Adrium.KeepassPfpConverter
{
	class PfpFormatProvider : FileFormatProvider
	{
		public override bool SupportsImport {
			get { return true; }
		}

		public override bool SupportsExport {
			get { return false; }
		}

		public override string FormatName {
			get { return "Pain-free Passwords backup"; }
		}

		public override string ApplicationGroup {
			get { return KeePass.Resources.KPRes.PasswordManagers; }
		}

		public override string DefaultExtension {
			get { return "json"; }
		}

		public override bool ImportAppendsToRootGroupOnly {
			get { return true; }
		}

		public override bool RequiresFile {
			get { return true; }
		}

		public override void Import(PwDatabase pwDatabase, Stream input, IStatusLogger statusLogger)
		{
			statusLogger.StartLogging("PfP import...", false);
			statusLogger.SetText("Sleeping...", LogStatusType.Info);

			System.Threading.Thread.Sleep(2000);

			statusLogger.SetText("Finished import.", LogStatusType.Info);
			statusLogger.EndLogging();
		}
	}
}
