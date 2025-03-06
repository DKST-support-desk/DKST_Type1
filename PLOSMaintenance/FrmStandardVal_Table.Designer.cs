
namespace PLOSMaintenance
{
	partial class FrmStandardVal_Table
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
            this.tabStdVal = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabStdVal
            // 
            this.tabStdVal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabStdVal.Location = new System.Drawing.Point(0, 0);
            this.tabStdVal.Margin = new System.Windows.Forms.Padding(2);
            this.tabStdVal.Multiline = true;
            this.tabStdVal.Name = "tabStdVal";
            this.tabStdVal.SelectedIndex = 0;
            this.tabStdVal.Size = new System.Drawing.Size(600, 360);
            this.tabStdVal.TabIndex = 0;
            // 
            // FrmStandardVal_Table
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 360);
            this.Controls.Add(this.tabStdVal);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmStandardVal_Table";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "標準値マスタ";
            this.Load += new System.EventHandler(this.FrmStandardVal_Table_Load);
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.TabControl tabStdVal;
    }
}