namespace PLOS.Gui.Core.Base
{
    partial class FormFixedDialogBase
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFixedDialogBase));
			this.pnlBtm = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.pnlBtm.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBtm
			// 
			resources.ApplyResources(this.pnlBtm, "pnlBtm");
			this.pnlBtm.Controls.Add(this.btnCancel);
			this.pnlBtm.Controls.Add(this.btnOk);
			this.pnlBtm.Name = "pnlBtm";
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// FormFixedDialogBase
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.pnlBtm);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFixedDialogBase";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.pnlBtm.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnOk;
        protected System.Windows.Forms.Panel pnlBtm;

    }
}