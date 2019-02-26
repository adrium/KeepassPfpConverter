using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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

		public override void Import(PwDatabase pwStorage, Stream sInput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return;

			slLogger.StartLogging("PfP import (this may take a while)...", false);

			IList<EntryObject> entries = new List<EntryObject>();

			using (var reader = new StreamReader(sInput)) {
				var pfpreader = new PfpReader(form.MasterPassword);
				entries = pfpreader.GetEntries(reader.ReadToEnd());
			}

			var protect = pwStorage.MemoryProtection;
			var i = 0;

			foreach (var entry in entries) {
				if (entry.type == null)
					continue;

				var pwEntry = new PwEntry(true, true);
				var strings = pwEntry.Strings;

				strings.Set(PwDefs.TitleField, new ProtectedString(protect.ProtectTitle, entry.site));
				strings.Set(PwDefs.UserNameField, new ProtectedString(protect.ProtectUserName, entry.name));
				strings.Set(PwDefs.PasswordField, new ProtectedString(protect.ProtectPassword, entry.password));
				strings.Set(PwDefs.UrlField, new ProtectedString(protect.ProtectUrl, string.Format("https://{0}/", entry.site)));

				if (entry.notes == null)
					entry.notes = "";

				if (!string.IsNullOrEmpty(entry.revision))
					entry.notes = string.Format("Revision: {0}\n{1}\n", entry.revision, entry.notes);

				entry.notes = StrUtil.NormalizeNewLines(entry.notes, true);
				strings.Set(PwDefs.NotesField, new ProtectedString(protect.ProtectNotes, entry.notes));

				pwStorage.RootGroup.AddEntry(pwEntry, true);
				i++;
			}

			slLogger.SetText(string.Format("Imported {0} entries.", i), LogStatusType.Info);
			slLogger.EndLogging();
		}
	}
}
