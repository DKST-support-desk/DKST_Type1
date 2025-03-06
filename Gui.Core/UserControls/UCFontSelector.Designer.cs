namespace PLOS.Gui.Core.UserControls
{
	partial class UCFontSelector
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
			this.lblFontName = new PLOS.Gui.Core.CustumContol.AntiAliasLabel();
			this.lblSize = new PLOS.Gui.Core.CustumContol.AntiAliasLabel();
			this.btnFontDialog = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblFontName
			// 
			this.lblFontName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFontName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblFontName.Location = new System.Drawing.Point(2, 2);
			this.lblFontName.Name = "lblFontName";
			this.lblFontName.RotateAngle = 0F;
			this.lblFontName.Size = new System.Drawing.Size(266, 36);
			this.lblFontName.TabIndex = 0;
			this.lblFontName.Text = "FontName";
			this.lblFontName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblFontName.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			this.lblFontName.TranslateX = 0F;
			this.lblFontName.TranslateY = 0F;
			// 
			// lblSize
			// 
			this.lblSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblSize.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblSize.Location = new System.Drawing.Point(268, 2);
			this.lblSize.Name = "lblSize";
			this.lblSize.RotateAngle = 0F;
			this.lblSize.Size = new System.Drawing.Size(80, 36);
			this.lblSize.TabIndex = 3;
			this.lblSize.Text = "0 Point";
			this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblSize.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			this.lblSize.TranslateX = 0F;
			this.lblSize.TranslateY = 0F;
			// 
			// btnFontDialog
			// 
			this.btnFontDialog.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnFontDialog.Location = new System.Drawing.Point(348, 2);
			this.btnFontDialog.Name = "btnFontDialog";
			this.btnFontDialog.Size = new System.Drawing.Size(100, 36);
			this.btnFontDialog.TabIndex = 4;
			this.btnFontDialog.Text = "Font";
			this.btnFontDialog.UseVisualStyleBackColor = true;
			this.btnFontDialog.Click += new System.EventHandler(this.OnFontDialogButtonClicked);
			// 
			// UCFontSelector
			// 
			this.Controls.Add(this.lblFontName);
			this.Controls.Add(this.lblSize);
			this.Controls.Add(this.btnFontDialog);
			this.Name = "UCFontSelector";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.Size = new System.Drawing.Size(450, 40);
			this.ResumeLayout(false);

		}

		#endregion

		private CustumContol.AntiAliasLabel lblFontName;
		private CustumContol.AntiAliasLabel lblSize;
		private System.Windows.Forms.Button btnFontDialog;
	}
}
