using Adrium.KeepassPfpConverter.Objects;
using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Adrium.KeepassPfpConverter.Plugin
{
	partial class FormatProvider : FileFormatProvider
	{
		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return false;

			slLogger.SetText("Collecting entries...", LogStatusType.Info);

			var entries = ConvertGroup(pwExportInfo.DataGroup, slLogger);

			slLogger.SetText("Encrypting backup...", LogStatusType.Info);

			var pfp = new PfpConvert();
			pfp.Save(sOutput, form.MasterPassword, entries);

			slLogger.SetText(string.Format("Exported {0} entries.", entries.Count), LogStatusType.Info);

			return true;
		}

		private IList<BaseEntry> ConvertGroup(PwGroup pwGroup, IStatusLogger slLogger)
		{
			var result = new List<BaseEntry>();

			foreach (var pwEntry in pwGroup.Entries) {
				var entry = Util.GetPfpEntry(pwEntry);
				result.Add(entry);
			}

			foreach (var pwSubGroup in pwGroup.Groups) {
				var subEntries = ConvertGroup(pwSubGroup, slLogger);
				result.AddRange(subEntries);
			}

			return result;
		}
	}
}
