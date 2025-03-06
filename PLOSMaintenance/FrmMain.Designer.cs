
namespace PLOSMaintenance
{
	partial class FrmMain
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

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageOperation = new System.Windows.Forms.TabPage();
            this.ucOperating_Shift_Regist = new PLOSMaintenance.UCOperating_Shift_Regist();
            this.tabPageLine = new System.Windows.Forms.TabPage();
            this.ucLineInfo = new PLOSMaintenance.UCLineInfo();
            this.tabPageProductType = new System.Windows.Forms.TabPage();
            this.ucProductType = new PLOSMaintenance.UCProductType();
            this.tabPageH_W = new System.Windows.Forms.TabPage();
            this.ucCamera = new PLOSMaintenance.UC_Device_Camera();
            this.tabPageDI = new System.Windows.Forms.TabPage();
            this.uC_Device_DI = new PLOSMaintenance.UC_Device_DI();
            this.tabPageDO = new System.Windows.Forms.TabPage();
            this.uC_Device_DO = new PLOSMaintenance.UC_Device_DO();
            this.tabPageQR = new System.Windows.Forms.TabPage();
            this.uC_Device_QR = new PLOSMaintenance.UC_Device_QR();
            this.tabPageComposition = new System.Windows.Forms.TabPage();
            this.ucComposition = new PLOSMaintenance.UCComposition();
            this.pnlMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPageOperation.SuspendLayout();
            this.tabPageLine.SuspendLayout();
            this.tabPageProductType.SuspendLayout();
            this.tabPageH_W.SuspendLayout();
            this.tabPageDI.SuspendLayout();
            this.tabPageDO.SuspendLayout();
            this.tabPageQR.SuspendLayout();
            this.tabPageComposition.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tabMain);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(2);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1336, 682);
            this.pnlMain.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageOperation);
            this.tabMain.Controls.Add(this.tabPageLine);
            this.tabMain.Controls.Add(this.tabPageProductType);
            this.tabMain.Controls.Add(this.tabPageH_W);
            this.tabMain.Controls.Add(this.tabPageDI);
            this.tabMain.Controls.Add(this.tabPageDO);
            this.tabMain.Controls.Add(this.tabPageQR);
            this.tabMain.Controls.Add(this.tabPageComposition);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Margin = new System.Windows.Forms.Padding(2);
            this.tabMain.Multiline = true;
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1336, 682);
            this.tabMain.TabIndex = 0;
            this.tabMain.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabMain_Selecting);
            // 
            // tabPageOperation
            // 
            this.tabPageOperation.Controls.Add(this.ucOperating_Shift_Regist);
            this.tabPageOperation.Location = new System.Drawing.Point(4, 26);
            this.tabPageOperation.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageOperation.Name = "tabPageOperation";
            this.tabPageOperation.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageOperation.Size = new System.Drawing.Size(1328, 652);
            this.tabPageOperation.TabIndex = 0;
            this.tabPageOperation.Text = "⓪稼働登録※毎日入力　";
            this.tabPageOperation.UseVisualStyleBackColor = true;
            // 
            // ucOperating_Shift_Regist
            // 
            this.ucOperating_Shift_Regist.AutoScroll = true;
            this.ucOperating_Shift_Regist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucOperating_Shift_Regist.Location = new System.Drawing.Point(2, 2);
            this.ucOperating_Shift_Regist.MinimumSize = new System.Drawing.Size(938, 0);
            this.ucOperating_Shift_Regist.Name = "ucOperating_Shift_Regist";
            this.ucOperating_Shift_Regist.Size = new System.Drawing.Size(1324, 648);
            this.ucOperating_Shift_Regist.TabIndex = 0;
            // 
            // tabPageLine
            // 
            this.tabPageLine.Controls.Add(this.ucLineInfo);
            this.tabPageLine.Location = new System.Drawing.Point(4, 48);
            this.tabPageLine.Name = "tabPageLine";
            this.tabPageLine.Size = new System.Drawing.Size(192, 48);
            this.tabPageLine.TabIndex = 6;
            this.tabPageLine.Text = "①ライン情報登録";
            this.tabPageLine.UseVisualStyleBackColor = true;
            // 
            // ucLineInfo
            // 
            this.ucLineInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucLineInfo.Location = new System.Drawing.Point(0, 0);
            this.ucLineInfo.Margin = new System.Windows.Forms.Padding(4);
            this.ucLineInfo.Name = "ucLineInfo";
            this.ucLineInfo.Size = new System.Drawing.Size(192, 48);
            this.ucLineInfo.TabIndex = 0;
            // 
            // tabPageProductType
            // 
            this.tabPageProductType.Controls.Add(this.ucProductType);
            this.tabPageProductType.Location = new System.Drawing.Point(4, 70);
            this.tabPageProductType.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageProductType.Name = "tabPageProductType";
            this.tabPageProductType.Size = new System.Drawing.Size(192, 26);
            this.tabPageProductType.TabIndex = 2;
            this.tabPageProductType.Text = "②品番タイプ登録　";
            this.tabPageProductType.UseVisualStyleBackColor = true;
            // 
            // ucProductType
            // 
            this.ucProductType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucProductType.Location = new System.Drawing.Point(0, 0);
            this.ucProductType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucProductType.Name = "ucProductType";
            this.ucProductType.Padding = new System.Windows.Forms.Padding(4);
            this.ucProductType.Size = new System.Drawing.Size(192, 26);
            this.ucProductType.TabIndex = 0;
            // 
            // tabPageH_W
            // 
            this.tabPageH_W.Controls.Add(this.ucCamera);
            this.tabPageH_W.Location = new System.Drawing.Point(4, 92);
            this.tabPageH_W.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageH_W.Name = "tabPageH_W";
            this.tabPageH_W.Size = new System.Drawing.Size(192, 4);
            this.tabPageH_W.TabIndex = 5;
            this.tabPageH_W.Text = "③カメラ登録";
            this.tabPageH_W.UseVisualStyleBackColor = true;
            // 
            // ucCamera
            // 
            this.ucCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCamera.Location = new System.Drawing.Point(0, 0);
            this.ucCamera.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.ucCamera.Name = "ucCamera";
            this.ucCamera.Padding = new System.Windows.Forms.Padding(4);
            this.ucCamera.Size = new System.Drawing.Size(192, 4);
            this.ucCamera.TabIndex = 0;
            // 
            // tabPageDI
            // 
            this.tabPageDI.Controls.Add(this.uC_Device_DI);
            this.tabPageDI.Location = new System.Drawing.Point(4, 114);
            this.tabPageDI.Name = "tabPageDI";
            this.tabPageDI.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDI.Size = new System.Drawing.Size(192, 0);
            this.tabPageDI.TabIndex = 7;
            this.tabPageDI.Text = "④センサ機器登録";
            this.tabPageDI.UseVisualStyleBackColor = true;
            // 
            // uC_Device_DI
            // 
            this.uC_Device_DI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uC_Device_DI.Location = new System.Drawing.Point(3, 3);
            this.uC_Device_DI.Margin = new System.Windows.Forms.Padding(4);
            this.uC_Device_DI.Name = "uC_Device_DI";
            this.uC_Device_DI.Size = new System.Drawing.Size(186, 0);
            this.uC_Device_DI.TabIndex = 0;
            // 
            // tabPageDO
            // 
            this.tabPageDO.Controls.Add(this.uC_Device_DO);
            this.tabPageDO.Location = new System.Drawing.Point(4, 136);
            this.tabPageDO.Name = "tabPageDO";
            this.tabPageDO.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDO.Size = new System.Drawing.Size(192, 0);
            this.tabPageDO.TabIndex = 8;
            this.tabPageDO.Text = "⑤ブザー機器登録";
            this.tabPageDO.UseVisualStyleBackColor = true;
            // 
            // uC_Device_DO
            // 
            this.uC_Device_DO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uC_Device_DO.Location = new System.Drawing.Point(3, 3);
            this.uC_Device_DO.Margin = new System.Windows.Forms.Padding(4);
            this.uC_Device_DO.Name = "uC_Device_DO";
            this.uC_Device_DO.Size = new System.Drawing.Size(186, 0);
            this.uC_Device_DO.TabIndex = 0;
            // 
            // tabPageQR
            // 
            this.tabPageQR.Controls.Add(this.uC_Device_QR);
            this.tabPageQR.Location = new System.Drawing.Point(4, 158);
            this.tabPageQR.Name = "tabPageQR";
            this.tabPageQR.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQR.Size = new System.Drawing.Size(192, 0);
            this.tabPageQR.TabIndex = 9;
            this.tabPageQR.Text = "⑥QRリーダ登録";
            this.tabPageQR.UseVisualStyleBackColor = true;
            // 
            // uC_Device_QR
            // 
            this.uC_Device_QR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uC_Device_QR.Location = new System.Drawing.Point(3, 3);
            this.uC_Device_QR.Margin = new System.Windows.Forms.Padding(4);
            this.uC_Device_QR.Name = "uC_Device_QR";
            this.uC_Device_QR.Size = new System.Drawing.Size(186, 0);
            this.uC_Device_QR.TabIndex = 0;
            // 
            // tabPageComposition
            // 
            this.tabPageComposition.Controls.Add(this.ucComposition);
            this.tabPageComposition.Location = new System.Drawing.Point(4, 180);
            this.tabPageComposition.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageComposition.Name = "tabPageComposition";
            this.tabPageComposition.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageComposition.Size = new System.Drawing.Size(192, 0);
            this.tabPageComposition.TabIndex = 1;
            this.tabPageComposition.Text = "⑦編成情報登録　";
            this.tabPageComposition.UseVisualStyleBackColor = true;
            // 
            // ucComposition
            // 
            this.ucComposition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucComposition.Location = new System.Drawing.Point(2, 2);
            this.ucComposition.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.ucComposition.Name = "ucComposition";
            this.ucComposition.Padding = new System.Windows.Forms.Padding(4);
            this.ucComposition.Size = new System.Drawing.Size(188, 0);
            this.ucComposition.TabIndex = 0;
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1336, 682);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(1054, 648);
            this.Name = "FrmMain";
            this.Text = "DKST手組版 マスタメンテナンス画面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.pnlMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabPageOperation.ResumeLayout(false);
            this.tabPageLine.ResumeLayout(false);
            this.tabPageProductType.ResumeLayout(false);
            this.tabPageH_W.ResumeLayout(false);
            this.tabPageDI.ResumeLayout(false);
            this.tabPageDO.ResumeLayout(false);
            this.tabPageQR.ResumeLayout(false);
            this.tabPageComposition.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageOperation;
        private UCOperating_Shift_Regist ucOperating_Shift_Regist;
        private System.Windows.Forms.TabPage tabPageLine;
        private UCLineInfo ucLineInfo;
        private System.Windows.Forms.TabPage tabPageProductType;
        private UCProductType ucProductType;
        private System.Windows.Forms.TabPage tabPageH_W;
        private UC_Device_Camera ucCamera;
        private System.Windows.Forms.TabPage tabPageDI;
        private System.Windows.Forms.TabPage tabPageDO;
        private System.Windows.Forms.TabPage tabPageQR;
        private System.Windows.Forms.TabPage tabPageComposition;
        private UCComposition ucComposition;
        private UC_Device_DI uC_Device_DI;
        private UC_Device_DO uC_Device_DO;
        private UC_Device_QR uC_Device_QR;
    }
}

