using Adrium.KeepassPfpConverter.Objects;
using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Adrium.KeepassPfpConverter.Plugin
{
	class PfpFormatProvider : FileFormatProvider
	{
		public override bool SupportsImport {
			get { return true; }
		}

		public override bool SupportsExport {
			get { return true; }
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

		public override void Import(PwDatabase pwStorage, Stream sInput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return;

			slLogger.SetText("Decrypting backup...", LogStatusType.Info);

			var crypto = new Crypto();
			crypto.SetMasterPassword(form.MasterPassword);

			var protect = pwStorage.MemoryProtection;
			var entries = PfpConvert.Load(crypto, sInput);
			var aliases = PfpConvert.GetAliases(entries);
			var i = 0;

			foreach (var baseentry in entries) {
				if (!(baseentry is PassEntry entry))
					continue;

				slLogger.SetText($"Importing {entry.name}@{entry.site}...", LogStatusType.Info);

				var pwEntry = Util.GetKeepassEntry(crypto, entry, protect);

				pwStorage.RootGroup.AddEntry(pwEntry, true);
				i++;
			}

			slLogger.SetText($"Imported {i} entries.", LogStatusType.Info);
		}

		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return false;

			slLogger.SetText("Collecting entries...", LogStatusType.Info);

			var crypto = new Crypto();
			crypto.SetMasterPassword(form.MasterPassword);

			var entries = ConvertGroup(crypto, pwExportInfo.DataGroup, slLogger);

			slLogger.SetText("Encrypting backup...", LogStatusType.Info);

			PfpConvert.Save(crypto, sOutput, entries);

			slLogger.SetText($"Exported {entries.Count} entries.", LogStatusType.Info);

			return true;
		}

		private IList<BaseEntry> ConvertGroup(Crypto crypto, PwGroup pwGroup, IStatusLogger slLogger)
		{
			var result = new List<BaseEntry>();

			foreach (var pwEntry in pwGroup.Entries) {
				var entry = Util.GetPfpEntry(crypto, pwEntry);
				result.Add(entry);
			}

			foreach (var pwSubGroup in pwGroup.Groups) {
				var subEntries = ConvertGroup(crypto, pwSubGroup, slLogger);
				result.AddRange(subEntries);
			}

			return result;
		}
	}
}
