namespace PLOS.Gui.Core.Base
{
	partial class FormExpansionImage
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
			this.picBoxExpansionImage = new PLOS.Gui.Core.CustumContol.PreviewPictureBox();
			this.pnlBtm.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picBoxExpansionImage)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(332, 4);
			this.btnCancel.Visible = false;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(332, 4);
			this.btnOk.Click += new System.EventHandler(this.OnClick);
			// 
			// pnlBtm
			// 
			this.pnlBtm.Location = new System.Drawing.Point(8, 304);
			this.pnlBtm.Size = new System.Drawing.Size(400, 28);
			// 
			// picBoxExpansionImage
			// 
			this.picBoxExpansionImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picBoxExpansionImage.Location = new System.Drawing.Point(8, 4);
			this.picBoxExpansionImage.Name = "picBoxExpansionImage";
			this.picBoxExpansionImage.NextImage = global::PLOS.Gui.Core.Properties.Resources.ImageNext;
			this.picBoxExpansionImage.PrevImage = global::PLOS.Gui.Core.Properties.Resources.ImagePrev;
			this.picBoxExpansionImage.Size = new System.Drawing.Size(400, 328);
			this.picBoxExpansionImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picBoxExpansionImage.TabIndex = 101;
			this.picBoxExpansionImage.TabStop = false;
			this.picBoxExpansionImage.ImageChange += new System.EventHandler<PLOS.Gui.Core.CustumContol.ImageChangeEventArgs>(this.OnPictureBoxImageChange);
			this.picBoxExpansionImage.ImageNext += new System.EventHandler(this.OnPictureBoxImageNext);
			this.picBoxExpansionImage.ImagePrev += new System.EventHandler(this.OnPictureBoxImagePrev);
			// 
			// FormExpansionImage
			// 
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(416, 336);
			this.ControlBox = true;
			this.Controls.Add(this.picBoxExpansionImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.MinimumSize = new System.Drawing.Size(300, 300);
			this.Name = "FormExpansionImage";
			this.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
			this.Click += new System.EventHandler(this.OnClick);
			this.Controls.SetChildIndex(this.picBoxExpansionImage, 0);
			this.Controls.SetChildIndex(this.pnlBtm, 0);
			this.pnlBtm.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picBoxExpansionImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

        private CustumContol.PreviewPictureBox picBoxExpansionImage;

    }
}
