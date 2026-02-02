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
			protect.Add("PUK");
			protect.Add("CVV");

			var entries = pfp.Load(sInput, form.MasterPassword);
			var pw = pfp.GetPasswordGetter(form.MasterPassword);

			var i = 0;

			foreach (var baseentry in entries) {
				var entry = baseentry as PassEntry;

				if (entry == null)
					continue;

				slLogger.SetText(string.Format("Importing {0} - {1} - {2}...", i, entry.site, entry.name), LogStatusType.Info);

				var pwEntry = Util.GetKeepassEntry(entry, pw, protect);

				pwStorage.RootGroup.AddEntry(pwEntry, true);
				i++;
			}

			slLogger.SetText(string.Format("Imported {0} entries.", i), LogStatusType.Info);
		}
	}
}
