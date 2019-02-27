using Adrium.KeepassPfpConverter.Objects;
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
		private const string NoUrlSite = "pfp.invalid";

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

			slLogger.StartLogging("PfP import (this may take a while)...", false);

			var crypto = new Crypto();
			crypto.SetMasterPassword(form.MasterPassword);

			var entries = PfpConvert.Load(crypto, sInput);

			var protect = pwStorage.MemoryProtection;
			var i = 0;

			foreach (var baseentry in entries) {
				if (!(baseentry is PassEntry entry))
					continue;

				var pwEntry = new PwEntry(true, true);
				var strings = pwEntry.Strings;

				strings.Set(PwDefs.TitleField, new ProtectedString(protect.ProtectTitle, entry.site));
				strings.Set(PwDefs.UserNameField, new ProtectedString(protect.ProtectUserName, entry.name));
				strings.Set(PwDefs.PasswordField, new ProtectedString(protect.ProtectPassword, Password.GetPassword(crypto, entry)));
				strings.Set(PwDefs.UrlField, new ProtectedString(protect.ProtectUrl, GetSiteForKeepass(entry)));

				var notes = GetNotesForKeepass(entry);
				if (notes != null)
					strings.Set(PwDefs.NotesField, new ProtectedString(protect.ProtectNotes, notes));

				pwStorage.RootGroup.AddEntry(pwEntry, true);
				i++;
			}

			slLogger.SetText(string.Format("Imported {0} entries.", i), LogStatusType.Info);
			slLogger.EndLogging();
		}

		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput, IStatusLogger slLogger)
		{
			var form = new OptionForm();
			if (form.ShowDialog() != DialogResult.OK)
				return false;

			slLogger.StartLogging("PfP export...", false);

			var crypto = new Crypto();
			crypto.SetMasterPassword(form.MasterPassword);

			var entries = ConvertGroup(pwExportInfo.DataGroup, slLogger);

			crypto.GenerateSalt();
			crypto.GenerateHmacSecret();

			PfpConvert.Save(crypto, sOutput, entries);

			slLogger.SetText("Export finished.", LogStatusType.Info);
			slLogger.EndLogging();

			return true;
		}

		private IList<BaseEntry> ConvertGroup(PwGroup pwGroup, IStatusLogger slLogger)
		{
			var result = new List<BaseEntry>();

			foreach (var pwEntry in pwGroup.Entries) {
				var entry = new StoredEntry();

				entry.type = "stored";
				entry.name = pwEntry.Strings.Get(PwDefs.UserNameField)?.ReadString();
				entry.password = pwEntry.Strings.Get(PwDefs.PasswordField)?.ReadString();
				entry.site = GetSiteForPfp(pwEntry.Strings.Get(PwDefs.UrlField)?.ReadString());
				entry.notes = GetNotesForPfp(pwEntry.Strings.Get(PwDefs.NotesField)?.ReadString());

				result.Add(entry);
			}

			foreach (var pwSubGroup in pwGroup.Groups) {
				var subEntries = ConvertGroup(pwSubGroup, slLogger);
				result.AddRange(subEntries);
			}

			return result;
		}

		private static string GetSiteForKeepass(PassEntry entry)
		{
			var result = string.Format("https://{0}/", entry.site);
			return result;
		}

		private static string GetNotesForKeepass(PassEntry entry)
		{
			var result = "";

			if (!string.IsNullOrEmpty(entry.notes))
				result = entry.notes;

			if (!string.IsNullOrEmpty(entry.revision))
				result = string.Format("Revision: {0}\n\n{1}", entry.revision, result);

			result = StrUtil.NormalizeNewLines(result, true);

			if (result.Equals(""))
				result = null;

			return result;
		}

		private static string GetSiteForPfp(string v)
		{
			if (v == null)
				return NoUrlSite;

			var result = v;
			result = result.Replace("https://", "");
			result = result.Replace("http://", "");
			result = result.Replace("www.", "");
			if (result.IndexOf("/") >= 0)
				result = result.Substring(0, result.IndexOf("/"));

			if (result.Equals(""))
				result = NoUrlSite;

			return result;
		}

		private static string GetNotesForPfp(string v)
		{
			if (v == null)
				return null;

			var result = StrUtil.NormalizeNewLines(v, false);
			if (result.Equals(""))
				result = null;
			return result;
		}
	}
}
