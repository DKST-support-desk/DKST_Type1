namespace PLOSDeviceConnectionManager
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
            this.components = new System.ComponentModel.Container();
            this.grbDioInput = new System.Windows.Forms.GroupBox();
            this.ucBitViewInput = new PLOSDeviceConnectionManager.UcBitView();
            this.grbDioOutput = new System.Windows.Forms.GroupBox();
            this.uclBitViewOutput = new PLOSDeviceConnectionManager.UcBitView();
            this.dgvCameraStatus = new System.Windows.Forms.DataGridView();
            this.clmCameraName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCameraStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCameraViewIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvQrStatus = new System.Windows.Forms.DataGridView();
            this.clmQrName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmQrStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrDeviceStatus = new System.Windows.Forms.Timer(this.components);
            this.lblOperating = new System.Windows.Forms.Label();
            this.lblClock = new System.Windows.Forms.Label();
            this.btnUpdateShift = new System.Windows.Forms.Button();
            this.pnlSelectInner = new System.Windows.Forms.Panel();
            this.btnSelectInnerProcessReset = new System.Windows.Forms.Button();
            this.cmbSelectInnerProcess = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ucBitViewInnerSelect = new PLOSDeviceConnectionManager.UcBitView();
            this.lblStatusDio = new System.Windows.Forms.Label();
            this.cmbCamIndex = new System.Windows.Forms.ComboBox();
            this.pnlStreamViewer = new System.Windows.Forms.Panel();
            this.pcbStreamView = new System.Windows.Forms.PictureBox();
            this.grbDioInput.SuspendLayout();
            this.grbDioOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameraStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQrStatus)).BeginInit();
            this.pnlSelectInner.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlStreamViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbStreamView)).BeginInit();
            this.SuspendLayout();
            // 
            // grbDioInput
            // 
            this.grbDioInput.Controls.Add(this.ucBitViewInput);
            this.grbDioInput.Location = new System.Drawing.Point(19, 140);
            this.grbDioInput.Margin = new System.Windows.Forms.Padding(10);
            this.grbDioInput.Name = "grbDioInput";
            this.grbDioInput.Size = new System.Drawing.Size(415, 120);
            this.grbDioInput.TabIndex = 0;
            this.grbDioInput.TabStop = false;
            this.grbDioInput.Text = "入力ポート";
            // 
            // ucBitViewInput
            // 
            this.ucBitViewInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBitViewInput.Location = new System.Drawing.Point(3, 15);
            this.ucBitViewInput.Name = "ucBitViewInput";
            this.ucBitViewInput.Size = new System.Drawing.Size(409, 102);
            this.ucBitViewInput.TabIndex = 0;
            // 
            // grbDioOutput
            // 
            this.grbDioOutput.Controls.Add(this.uclBitViewOutput);
            this.grbDioOutput.Location = new System.Drawing.Point(447, 140);
            this.grbDioOutput.Name = "grbDioOutput";
            this.grbDioOutput.Padding = new System.Windows.Forms.Padding(10);
            this.grbDioOutput.Size = new System.Drawing.Size(415, 120);
            this.grbDioOutput.TabIndex = 0;
            this.grbDioOutput.TabStop = false;
            this.grbDioOutput.Text = "出力ポート";
            // 
            // uclBitViewOutput
            // 
            this.uclBitViewOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uclBitViewOutput.Location = new System.Drawing.Point(10, 22);
            this.uclBitViewOutput.Name = "uclBitViewOutput";
            this.uclBitViewOutput.Size = new System.Drawing.Size(395, 88);
            this.uclBitViewOutput.TabIndex = 0;
            // 
            // dgvCameraStatus
            // 
            this.dgvCameraStatus.AllowUserToAddRows = false;
            this.dgvCameraStatus.AllowUserToDeleteRows = false;
            this.dgvCameraStatus.AllowUserToResizeColumns = false;
            this.dgvCameraStatus.AllowUserToResizeRows = false;
            this.dgvCameraStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCameraStatus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmCameraName,
            this.clmCameraStatus,
            this.clmCameraViewIndex});
            this.dgvCameraStatus.Location = new System.Drawing.Point(19, 273);
            this.dgvCameraStatus.Name = "dgvCameraStatus";
            this.dgvCameraStatus.ReadOnly = true;
            this.dgvCameraStatus.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCameraStatus.RowTemplate.Height = 21;
            this.dgvCameraStatus.Size = new System.Drawing.Size(415, 255);
            this.dgvCameraStatus.TabIndex = 1;
            // 
            // clmCameraName
            // 
            this.clmCameraName.DataPropertyName = "CameraName";
            this.clmCameraName.HeaderText = "カメラ名";
            this.clmCameraName.Name = "clmCameraName";
            this.clmCameraName.ReadOnly = true;
            this.clmCameraName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmCameraName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmCameraName.Width = 180;
            // 
            // clmCameraStatus
            // 
            this.clmCameraStatus.DataPropertyName = "CameraStatus";
            this.clmCameraStatus.FillWeight = 80F;
            this.clmCameraStatus.HeaderText = "接続状態";
            this.clmCameraStatus.Name = "clmCameraStatus";
            this.clmCameraStatus.ReadOnly = true;
            this.clmCameraStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmCameraStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmCameraStatus.Width = 80;
            // 
            // clmCameraViewIndex
            // 
            this.clmCameraViewIndex.DataPropertyName = "CameraViewIndex";
            this.clmCameraViewIndex.FillWeight = 80F;
            this.clmCameraViewIndex.HeaderText = "表示番号";
            this.clmCameraViewIndex.Name = "clmCameraViewIndex";
            this.clmCameraViewIndex.ReadOnly = true;
            this.clmCameraViewIndex.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmCameraViewIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmCameraViewIndex.Width = 80;
            // 
            // dgvQrStatus
            // 
            this.dgvQrStatus.AllowUserToAddRows = false;
            this.dgvQrStatus.AllowUserToDeleteRows = false;
            this.dgvQrStatus.AllowUserToResizeColumns = false;
            this.dgvQrStatus.AllowUserToResizeRows = false;
            this.dgvQrStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvQrStatus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmQrName,
            this.clmQrStatus});
            this.dgvQrStatus.Location = new System.Drawing.Point(447, 273);
            this.dgvQrStatus.Name = "dgvQrStatus";
            this.dgvQrStatus.ReadOnly = true;
            this.dgvQrStatus.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvQrStatus.RowTemplate.Height = 21;
            this.dgvQrStatus.Size = new System.Drawing.Size(415, 255);
            this.dgvQrStatus.TabIndex = 1;
            // 
            // clmQrName
            // 
            this.clmQrName.DataPropertyName = "ReaderName";
            this.clmQrName.HeaderText = "QRリーダ名";
            this.clmQrName.Name = "clmQrName";
            this.clmQrName.ReadOnly = true;
            this.clmQrName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmQrName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmQrName.Width = 200;
            // 
            // clmQrStatus
            // 
            this.clmQrStatus.DataPropertyName = "ReaderStatus";
            this.clmQrStatus.HeaderText = "接続状態";
            this.clmQrStatus.Name = "clmQrStatus";
            this.clmQrStatus.ReadOnly = true;
            this.clmQrStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmQrStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tmrDeviceStatus
            // 
            this.tmrDeviceStatus.Interval = 50;
            this.tmrDeviceStatus.Tick += new System.EventHandler(this.TmrDeviceStatus_Tick);
            // 
            // lblOperating
            // 
            this.lblOperating.AutoSize = true;
            this.lblOperating.BackColor = System.Drawing.Color.Lime;
            this.lblOperating.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblOperating.Location = new System.Drawing.Point(18, 62);
            this.lblOperating.Name = "lblOperating";
            this.lblOperating.Size = new System.Drawing.Size(139, 19);
            this.lblOperating.TabIndex = 3;
            this.lblOperating.Text = "直状態信号：ON";
            // 
            // lblClock
            // 
            this.lblClock.AutoSize = true;
            this.lblClock.BackColor = System.Drawing.SystemColors.Control;
            this.lblClock.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblClock.Location = new System.Drawing.Point(163, 62);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(47, 19);
            this.lblClock.TabIndex = 3;
            this.lblClock.Text = "時計";
            // 
            // btnUpdateShift
            // 
            this.btnUpdateShift.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUpdateShift.Location = new System.Drawing.Point(19, 534);
            this.btnUpdateShift.Name = "btnUpdateShift";
            this.btnUpdateShift.Size = new System.Drawing.Size(191, 44);
            this.btnUpdateShift.TabIndex = 4;
            this.btnUpdateShift.Text = "計画稼働シフト更新";
            this.btnUpdateShift.UseVisualStyleBackColor = true;
            this.btnUpdateShift.Click += new System.EventHandler(this.btnUpdateShift_Click);
            // 
            // pnlSelectInner
            // 
            this.pnlSelectInner.Controls.Add(this.btnSelectInnerProcessReset);
            this.pnlSelectInner.Controls.Add(this.cmbSelectInnerProcess);
            this.pnlSelectInner.Controls.Add(this.groupBox1);
            this.pnlSelectInner.Location = new System.Drawing.Point(878, 140);
            this.pnlSelectInner.Name = "pnlSelectInner";
            this.pnlSelectInner.Size = new System.Drawing.Size(373, 119);
            this.pnlSelectInner.TabIndex = 6;
            // 
            // btnSelectInnerProcessReset
            // 
            this.btnSelectInnerProcessReset.Location = new System.Drawing.Point(160, 15);
            this.btnSelectInnerProcessReset.Name = "btnSelectInnerProcessReset";
            this.btnSelectInnerProcessReset.Size = new System.Drawing.Size(75, 20);
            this.btnSelectInnerProcessReset.TabIndex = 2;
            this.btnSelectInnerProcessReset.Text = "Reset";
            this.btnSelectInnerProcessReset.UseVisualStyleBackColor = true;
            this.btnSelectInnerProcessReset.Click += new System.EventHandler(this.btnSelectInnerProcessReset_Click);
            // 
            // cmbSelectInnerProcess
            // 
            this.cmbSelectInnerProcess.FormattingEnabled = true;
            this.cmbSelectInnerProcess.Location = new System.Drawing.Point(241, 15);
            this.cmbSelectInnerProcess.Name = "cmbSelectInnerProcess";
            this.cmbSelectInnerProcess.Size = new System.Drawing.Size(121, 20);
            this.cmbSelectInnerProcess.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ucBitViewInnerSelect);
            this.groupBox1.Location = new System.Drawing.Point(3, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(359, 94);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "選択内部工程";
            // 
            // ucBitViewInnerSelect
            // 
            this.ucBitViewInnerSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBitViewInnerSelect.Location = new System.Drawing.Point(10, 22);
            this.ucBitViewInnerSelect.Name = "ucBitViewInnerSelect";
            this.ucBitViewInnerSelect.Size = new System.Drawing.Size(339, 62);
            this.ucBitViewInnerSelect.TabIndex = 0;
            // 
            // lblStatusDio
            // 
            this.lblStatusDio.AutoSize = true;
            this.lblStatusDio.BackColor = System.Drawing.Color.Lime;
            this.lblStatusDio.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatusDio.Location = new System.Drawing.Point(18, 94);
            this.lblStatusDio.Name = "lblStatusDio";
            this.lblStatusDio.Size = new System.Drawing.Size(163, 19);
            this.lblStatusDio.TabIndex = 3;
            this.lblStatusDio.Text = "DIO接続状態：正常";
            // 
            // cmbCamIndex
            // 
            this.cmbCamIndex.FormattingEnabled = true;
            this.cmbCamIndex.Location = new System.Drawing.Point(10, 3);
            this.cmbCamIndex.Name = "cmbCamIndex";
            this.cmbCamIndex.Size = new System.Drawing.Size(121, 20);
            this.cmbCamIndex.TabIndex = 8;
            this.cmbCamIndex.SelectedIndexChanged += new System.EventHandler(this.cmbCamIndex_SelectedIndexChanged);
            // 
            // pnlStreamViewer
            // 
            this.pnlStreamViewer.Controls.Add(this.pcbStreamView);
            this.pnlStreamViewer.Controls.Add(this.cmbCamIndex);
            this.pnlStreamViewer.Location = new System.Drawing.Point(868, 273);
            this.pnlStreamViewer.Name = "pnlStreamViewer";
            this.pnlStreamViewer.Size = new System.Drawing.Size(391, 255);
            this.pnlStreamViewer.TabIndex = 9;
            // 
            // pcbStreamView
            // 
            this.pcbStreamView.Location = new System.Drawing.Point(10, 29);
            this.pcbStreamView.Name = "pcbStreamView";
            this.pcbStreamView.Size = new System.Drawing.Size(374, 218);
            this.pcbStreamView.TabIndex = 8;
            this.pcbStreamView.TabStop = false;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.pnlStreamViewer);
            this.Controls.Add(this.pnlSelectInner);
            this.Controls.Add(this.btnUpdateShift);
            this.Controls.Add(this.lblClock);
            this.Controls.Add(this.lblStatusDio);
            this.Controls.Add(this.lblOperating);
            this.Controls.Add(this.dgvQrStatus);
            this.Controls.Add(this.dgvCameraStatus);
            this.Controls.Add(this.grbDioOutput);
            this.Controls.Add(this.grbDioInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "デバイス通信コントローラ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grbDioInput.ResumeLayout(false);
            this.grbDioOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameraStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQrStatus)).EndInit();
            this.pnlSelectInner.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.pnlStreamViewer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbStreamView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbDioInput;
        private UcBitView ucBitViewInput;
        private System.Windows.Forms.GroupBox grbDioOutput;
        private UcBitView uclBitViewOutput;
        private System.Windows.Forms.DataGridView dgvCameraStatus;
        private System.Windows.Forms.DataGridView dgvQrStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmQrName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmQrStatus;
        private System.Windows.Forms.Timer tmrDeviceStatus;
        private System.Windows.Forms.Label lblOperating;
        private System.Windows.Forms.Label lblClock;
        private System.Windows.Forms.Button btnUpdateShift;
        private System.Windows.Forms.Panel pnlSelectInner;
        private System.Windows.Forms.ComboBox cmbSelectInnerProcess;
        private System.Windows.Forms.GroupBox groupBox1;
        private UcBitView ucBitViewInnerSelect;
        private System.Windows.Forms.Button btnSelectInnerProcessReset;
        private System.Windows.Forms.Label lblStatusDio;
        private System.Windows.Forms.ComboBox cmbCamIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCameraName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCameraStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCameraViewIndex;
        private System.Windows.Forms.Panel pnlStreamViewer;
        private System.Windows.Forms.PictureBox pcbStreamView;
    }
}

