namespace PLOS.Gui.Core.Base
{
	partial class FormItemSelectionBase
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlMain = new System.Windows.Forms.Panel();
			this.lstbxSelection = new System.Windows.Forms.ListBox();
			this.pnlBtm.SuspendLayout();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.lstbxSelection);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(8);
			this.pnlMain.Size = new System.Drawing.Size(394, 243);
			this.pnlMain.TabIndex = 1;
			// 
			// lstbxSelection
			// 
			this.lstbxSelection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstbxSelection.FormattingEnabled = true;
			this.lstbxSelection.ItemHeight = 12;
			this.lstbxSelection.Location = new System.Drawing.Point(8, 8);
			this.lstbxSelection.Name = "lstbxSelection";
			this.lstbxSelection.Size = new System.Drawing.Size(378, 227);
			this.lstbxSelection.TabIndex = 2;
			this.lstbxSelection.DoubleClick += new System.EventHandler(this.OnSelectionListDoubleClick);
			// 
			// FormItemSelectionBase
			// 
			this.ClientSize = new System.Drawing.Size(394, 275);
			this.Controls.Add(this.pnlMain);
			this.Name = "FormItemSelectionBase";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.Controls.SetChildIndex(this.pnlBtm, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.pnlBtm.ResumeLayout(false);
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel pnlMain;
		protected System.Windows.Forms.ListBox lstbxSelection;
	}
}
