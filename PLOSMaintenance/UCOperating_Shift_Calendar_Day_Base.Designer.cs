
namespace PLOSMaintenance
{
	partial class UCOperating_Shift_Calendar_Day_Base
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
			this.pnlMain = new System.Windows.Forms.Panel();
			this.lblDay = new System.Windows.Forms.Label();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.lblDay);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(4);
			this.pnlMain.Size = new System.Drawing.Size(200, 150);
			this.pnlMain.TabIndex = 0;
			this.pnlMain.Click += new System.EventHandler(this.OnPanelClicked);
			this.pnlMain.Resize += new System.EventHandler(this.OnPanelResized);
			// 
			// lblDay
			// 
			this.lblDay.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDay.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lblDay.Location = new System.Drawing.Point(4, 4);
			this.lblDay.Margin = new System.Windows.Forms.Padding(0);
			this.lblDay.Name = "lblDay";
			this.lblDay.Size = new System.Drawing.Size(192, 69);
			this.lblDay.TabIndex = 0;
			this.lblDay.Text = "label1";
			this.lblDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblDay.Click += new System.EventHandler(this.OnPanelClicked);
			// 
			// UCOperating_Shift_Calendar_Day
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlMain);
			this.Name = "UCOperating_Shift_Calendar_Day";
			this.Size = new System.Drawing.Size(200, 150);
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel pnlMain;
		protected System.Windows.Forms.Label lblDay;
	}
}
