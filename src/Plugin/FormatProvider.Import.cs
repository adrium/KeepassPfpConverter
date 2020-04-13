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
		public override void Import(PwDatabase pwStorage, Stream sInput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return;

			slLogger.SetText("Decrypting backup...", LogStatusType.Info);

			var pfp = new PfpConvert();
			var protect = Util.GetMemoryProtection(pwStorage.MemoryProtection);
			protect.Add("PIN");
			protect.Add("CVV");

			var entries = pfp.Load(sInput, form.MasterPassword);
			var pw = pfp.GetPasswordGetter(form.MasterPassword);

			var i = 0;

			foreach (var baseentry in entries) {
				if (!(baseentry is PassEntry entry))
					continue;

				slLogger.SetText($"Importing {entry.name}@{entry.site}...", LogStatusType.Info);

				var pwEntry = Util.GetKeepassEntry(entry, pw, protect);

				pwStorage.RootGroup.AddEntry(pwEntry, true);
				i++;
			}

			slLogger.SetText($"Imported {i} entries.", LogStatusType.Info);
		}
	}
}
