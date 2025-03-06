namespace PLOS.Gui.Core.Base
{
    partial class FormComboListSubstitution
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
            this.LstComboSelect = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // LstComboSelect
            // 
            this.LstComboSelect.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstComboSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstComboSelect.FormattingEnabled = true;
            this.LstComboSelect.ItemHeight = 12;
            this.LstComboSelect.Location = new System.Drawing.Point(0, 0);
            this.LstComboSelect.Name = "LstComboSelect";
            this.LstComboSelect.Size = new System.Drawing.Size(148, 198);
            this.LstComboSelect.TabIndex = 0;
            // 
            // FrmComboListSubstitution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(148, 198);
            this.ControlBox = false;
            this.Controls.Add(this.LstComboSelect);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmComboListSubstitution";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.ListBox LstComboSelect;

    }
}