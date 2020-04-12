namespace Adrium.KeepassPfpConverter.Plugin
{
	public partial class OptionForm
	{
		private void InitializeComponent()
		{
			this.formButtonOk = new System.Windows.Forms.Button();
			this.fromButtonCancel = new System.Windows.Forms.Button();
			this.passGroup = new System.Windows.Forms.GroupBox();
			this.passGroupText = new System.Windows.Forms.TextBox();
			this.passGroup.SuspendLayout();

			this.formButtonOk.Text = "OK";
			this.formButtonOk.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.formButtonOk.Location = new System.Drawing.Point(128, 200);
			this.formButtonOk.Size = new System.Drawing.Size(80, 24);
			this.formButtonOk.TabIndex = 100;
			this.formButtonOk.Click += new System.EventHandler(this.formButtonOk_Click);

			this.fromButtonCancel.Text = "Cancel";
			this.fromButtonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.fromButtonCancel.Location = new System.Drawing.Point(224, 200);
			this.fromButtonCancel.Size = new System.Drawing.Size(80, 24);
			this.fromButtonCancel.TabIndex = 101;
			this.fromButtonCancel.Click += new System.EventHandler(this.fromButtonCancel_Click);

			this.passGroup.Text = "Master password";
			this.passGroup.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right);
			this.passGroup.Controls.Add(this.passGroupText);
			this.passGroup.Location = new System.Drawing.Point(16, 16);
			this.passGroup.Size = new System.Drawing.Size(288, 64);

			this.passGroupText.Text = "";
			this.passGroupText.UseSystemPasswordChar = true;
			this.passGroupText.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right);
			this.passGroupText.Location = new System.Drawing.Point(16, 24);
			this.passGroupText.Name = "passGroupText";
			this.passGroupText.Size = new System.Drawing.Size(256, 22);
			this.passGroupText.TabIndex = 1;

			this.AcceptButton = formButtonOk;
			this.ClientSize = new System.Drawing.Size(320, 240);
			this.Text = "Options";
			this.Location = new System.Drawing.Point(0, 0);
			this.Controls.Add(this.formButtonOk);
			this.Controls.Add(this.fromButtonCancel);
			this.Controls.Add(this.passGroup);
			this.passGroup.ResumeLayout(false);
		}

		private System.Windows.Forms.Button formButtonOk;
		private System.Windows.Forms.Button fromButtonCancel;
		private System.Windows.Forms.GroupBox passGroup;
		private System.Windows.Forms.TextBox passGroupText;
	}
}
