
namespace PLOSMaintenance
{
	partial class UCOperating_Shift_Pattern
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.pnlOperatingShuftPattern = new System.Windows.Forms.Panel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlOperatingShuftPattern.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlOperatingShuftPattern
			// 
			this.pnlOperatingShuftPattern.AutoScroll = true;
			this.pnlOperatingShuftPattern.Controls.Add(this.tableLayoutPanel3);
			this.pnlOperatingShuftPattern.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlOperatingShuftPattern.Location = new System.Drawing.Point(0, 0);
			this.pnlOperatingShuftPattern.Name = "pnlOperatingShuftPattern";
			this.pnlOperatingShuftPattern.Size = new System.Drawing.Size(350, 350);
			this.pnlOperatingShuftPattern.TabIndex = 2;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 10;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(329, 600);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAdd,
            this.menuRemove});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(211, 80);
			// 
			// menuAdd
			// 
			this.menuAdd.Name = "menuAdd";
			this.menuAdd.Size = new System.Drawing.Size(210, 24);
			this.menuAdd.Text = "現状画⾯を登録";
			this.menuAdd.Click += new System.EventHandler(this.OnAddOperatingPatternClicked);
			// 
			// menuRemove
			// 
			this.menuRemove.Name = "menuRemove";
			this.menuRemove.Size = new System.Drawing.Size(210, 24);
			this.menuRemove.Text = "登録パターンをクリア";
			this.menuRemove.Click += new System.EventHandler(this.OnClearOperatingPatternClicked);
			// 
			// UCOperating_Shift_Pattern
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlOperatingShuftPattern);
			this.Name = "UCOperating_Shift_Pattern";
			this.Size = new System.Drawing.Size(350, 350);
			this.pnlOperatingShuftPattern.ResumeLayout(false);
			this.pnlOperatingShuftPattern.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlOperatingShuftPattern;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuAdd;
		private System.Windows.Forms.ToolStripMenuItem menuRemove;
	}
}
