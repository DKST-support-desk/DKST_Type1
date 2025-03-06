
namespace PLOSMaintenance
{
	partial class UCProductTypeRow
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
			this.tableLayoutPanelRow = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.tableLayoutPanelRow.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanelRow
			// 
			this.tableLayoutPanelRow.ColumnCount = 2;
			this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanelRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this.tableLayoutPanelRow.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanelRow.Controls.Add(this.textBox1, 1, 0);
			this.tableLayoutPanelRow.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanelRow.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanelRow.Name = "tableLayoutPanelRow";
			this.tableLayoutPanelRow.RowCount = 1;
			this.tableLayoutPanelRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanelRow.Size = new System.Drawing.Size(600, 100);
			this.tableLayoutPanelRow.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.BackgroundImage = global::PLOSMaintenance.Properties.Resources.削除アイコン;
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(54, 94);
			this.panel1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(63, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 25);
			this.textBox1.TabIndex = 1;
			// 
			// UCProductTypeRow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanelRow);
			this.Name = "UCProductTypeRow";
			this.Size = new System.Drawing.Size(600, 150);
			this.tableLayoutPanelRow.ResumeLayout(false);
			this.tableLayoutPanelRow.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRow;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBox1;
	}
}
