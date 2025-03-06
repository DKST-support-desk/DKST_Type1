namespace PLOS.Gui.Core.Base
{
	partial class UCRichMessagePanel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCRichMessagePanel));
			this.lblMessage = new PLOS.Gui.Core.CustumContol.AntiAliasLabel();
			this.pnlOption = new System.Windows.Forms.Panel();
			this.picIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// lblMessage
			// 
			resources.ApplyResources(this.lblMessage, "lblMessage");
			this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.RotateAngle = 0F;
			this.lblMessage.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			this.lblMessage.TranslateX = 0F;
			this.lblMessage.TranslateY = 0F;
			this.lblMessage.Click += new System.EventHandler(this.OnControlClick);
			// 
			// pnlOption
			// 
			resources.ApplyResources(this.pnlOption, "pnlOption");
			this.pnlOption.Name = "pnlOption";
			// 
			// picIcon
			// 
			resources.ApplyResources(this.picIcon, "picIcon");
			this.picIcon.Name = "picIcon";
			this.picIcon.TabStop = false;
			// 
			// UCRichMessagePanel
			// 
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.picIcon);
			this.Controls.Add(this.pnlOption);
			this.MinimumSize = new System.Drawing.Size(300, 60);
			this.Name = "UCRichMessagePanel";
			resources.ApplyResources(this, "$this");
			this.Click += new System.EventHandler(this.OnControlClick);
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		protected PLOS.Gui.Core.CustumContol.AntiAliasLabel	lblMessage;
		private System.Windows.Forms.Panel pnlOption;
		private System.Windows.Forms.PictureBox picIcon;

	}
}
