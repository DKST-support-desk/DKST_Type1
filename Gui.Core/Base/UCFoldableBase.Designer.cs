namespace PLOS.Gui.Core.Base
{
	partial class UCFoldableBase
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCFoldableBase));
			this.pnlFold = new System.Windows.Forms.Panel();
			this.tlblTitle = new PLOS.Gui.Core.CustumContol.ToggleLabel(this.components);
			this.SuspendLayout();
			// 
			// pnlFold
			// 
			this.pnlFold.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlFold.Location = new System.Drawing.Point(0, 24);
			this.pnlFold.Name = "pnlFold";
			this.pnlFold.Size = new System.Drawing.Size(150, 126);
			this.pnlFold.TabIndex = 10;
			// 
			// tlblTitle
			// 
			this.tlblTitle.BaseColor = System.Drawing.Color.CadetBlue;
			this.tlblTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.tlblTitle.FalseImage = ((System.Drawing.Image)(resources.GetObject("tlblTitle.FalseImage")));
			this.tlblTitle.ForeColor = System.Drawing.Color.White;
			this.tlblTitle.LabelRotateAngle = 0F;
			this.tlblTitle.LabelText = "_ToggleTitle";
			this.tlblTitle.LabelTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tlblTitle.LabelTextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			this.tlblTitle.LabelTranslateX = 0F;
			this.tlblTitle.LabelTranslateY = 0F;
			this.tlblTitle.Location = new System.Drawing.Point(0, 0);
			this.tlblTitle.Name = "tlblTitle";
			this.tlblTitle.ReadOnly = true;
			this.tlblTitle.Size = new System.Drawing.Size(150, 24);
			this.tlblTitle.TabIndex = 0;
			this.tlblTitle.TabStop = true;
			this.tlblTitle.TogglePadding = new System.Windows.Forms.Padding(3, 1, 3, 1);
			this.tlblTitle.ToggleSizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.tlblTitle.ToggleStatus = true;
			this.tlblTitle.ToggleWidth = 20;
			this.tlblTitle.TrueImage = ((System.Drawing.Image)(resources.GetObject("tlblTitle.TrueImage")));
			// 
			// UCFoldableBase
			// 
			this.Controls.Add(this.pnlFold);
			this.Controls.Add(this.tlblTitle);
			this.Name = "UCFoldableBase";
			this.ResumeLayout(false);

		}

		#endregion

		private CustumContol.ToggleLabel tlblTitle;
		public System.Windows.Forms.Panel pnlFold;
	}
}
