using System;
using System.Windows.Forms;

namespace Adrium.KeepassPfpConverter.Plugin
{
	public partial class OptionForm : Form
	{
		public string MasterPassword { get; private set; }

		public OptionForm()
		{
			InitializeComponent();
			ActiveControl = passGroupText;
		}

		private void formButtonOk_Click(object sender, EventArgs e)
		{
			if (passGroupText.Text.Equals("")) {
				MessageBox.Show("Master password required");
				DialogResult = DialogResult.None;
			} else {
				MasterPassword = passGroupText.Text;
				DialogResult = DialogResult.OK;
			}
		}

		private void fromButtonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
