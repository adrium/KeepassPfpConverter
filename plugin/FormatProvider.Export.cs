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

			var entries = new List<BaseEntry>();
			ConvertGroup(pwExportInfo.DataGroup, slLogger, entries, new DedupList(), "");

			slLogger.SetText("Encrypting backup...", LogStatusType.Info);

			var pfp = new PfpConvert();
			pfp.Save(sOutput, form.MasterPassword, entries);

			slLogger.SetText(string.Format("Exported {0} entries.", entries.Count), LogStatusType.Info);

			return true;
		}

		private void ConvertGroup(PwGroup pwGroup, IStatusLogger slLogger, IList<BaseEntry> list, DedupList dup, string prefix)
		{
			prefix = prefix + pwGroup.Name + "/";
			foreach (var pwEntry in pwGroup.Entries) {
				var title = prefix + PwEntryIndexer.GetString(pwEntry, PwDefs.TitleField);
				var entry = Util.GetPfpEntry(pwEntry);
				dup.Deduplicate(entry, title);
				list.Add(entry);
			}

			foreach (var pwSubGroup in pwGroup.Groups)
				ConvertGroup(pwSubGroup, slLogger, list, dup, prefix);
		}
	}
}
