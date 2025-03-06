namespace PLOS.Gui.Core.Base
{
	partial class UCExpansionPictureBox
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
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.txtIndex = new PLOS.Gui.Core.CustumContol.AntiAliasLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(1, 1);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(78, 58);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.pictureBox.Click += new System.EventHandler(this.OnClick);
			// 
			// txtIndex
			// 
			this.txtIndex.AutoSize = true;
			this.txtIndex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(100)))));
			//this.txtIndex.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtIndex.ForeColor = System.Drawing.Color.White;
			this.txtIndex.Location = new System.Drawing.Point(1, 1);
			this.txtIndex.Name = "txtIndex";
			this.txtIndex.RotateAngle = 0F;
			this.txtIndex.Size = new System.Drawing.Size(25, 14);
			this.txtIndex.TabIndex = 1;
			this.txtIndex.Text = "000";
			this.txtIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.txtIndex.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			this.txtIndex.TranslateX = 0F;
			this.txtIndex.TranslateY = 0F;
			// 
			// UCExpansionPictureBox
			// 
			this.Controls.Add(this.txtIndex);
			this.Controls.Add(this.pictureBox);
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			this.Name = "UCExpansionPictureBox";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Size = new System.Drawing.Size(80, 60);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox;
		private CustumContol.AntiAliasLabel txtIndex;
	}
}
