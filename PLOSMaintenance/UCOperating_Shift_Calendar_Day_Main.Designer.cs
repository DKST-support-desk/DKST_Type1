
namespace PLOSMaintenance
{
	partial class UCOperating_Shift_Calendar_Day_Main
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCOperating_Shift_Calendar_Day_Main));
			this.picProduction = new System.Windows.Forms.PictureBox();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picProduction)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.picProduction);
			this.pnlMain.Controls.SetChildIndex(this.lblDay, 0);
			this.pnlMain.Controls.SetChildIndex(this.picProduction, 0);
			// 
			// picProduction
			// 
			this.picProduction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.picProduction.BackColor = System.Drawing.Color.Transparent;
			this.picProduction.InitialImage = ((System.Drawing.Image)(resources.GetObject("picProduction.InitialImage")));
			this.picProduction.Location = new System.Drawing.Point(170, 120);
			this.picProduction.Name = "picProduction";
			this.picProduction.Size = new System.Drawing.Size(30, 30);
			this.picProduction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picProduction.TabIndex = 1;
			this.picProduction.TabStop = false;
			// 
			// UCOperating_Shift_Calendar_Day_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "UCOperating_Shift_Calendar_Day_Main";
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picProduction)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picProduction;
	}
}
