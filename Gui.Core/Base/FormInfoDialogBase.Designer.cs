namespace PLOS.Gui.Core.Base
{
    partial class FormInfoDialogBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfoDialogBase));
			this.pnlBtm = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.pnlBtm.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBtm
			// 
			resources.ApplyResources(this.pnlBtm, "pnlBtm");
			this.pnlBtm.Controls.Add(this.btnClose);
			this.pnlBtm.Name = "pnlBtm";
			// 
			// btnClose
			// 
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// FormInfoDialogBase
			// 
			this.AcceptButton = this.btnClose;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnClose;
			this.ControlBox = false;
			this.Controls.Add(this.pnlBtm);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInfoDialogBase";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.pnlBtm.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button btnClose;
        protected System.Windows.Forms.Panel pnlBtm;

    }
}