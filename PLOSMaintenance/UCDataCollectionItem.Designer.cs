
namespace PLOSMaintenance
{
	partial class UCDataCollectionItem
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
			this.txtProcessName = new System.Windows.Forms.TextBox();
			this.cmbTriggerType = new System.Windows.Forms.ComboBox();
			this.cmbSensorDevice_1 = new System.Windows.Forms.ComboBox();
			this.cmbSensorDevice_2 = new System.Windows.Forms.ComboBox();
			this.lblSensorFillterUnit_1 = new System.Windows.Forms.Label();
			this.NmSensorFilter_1 = new System.Windows.Forms.NumericUpDown();
			this.NmSensorFilter_2 = new System.Windows.Forms.NumericUpDown();
			this.lblSensorFillterUnit_2 = new System.Windows.Forms.Label();
			this.dgvCamera = new System.Windows.Forms.DataGridView();
			this.cmbDODevice = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblSensorTitle = new System.Windows.Forms.Label();
			this.lblSensorFillterTitle = new System.Windows.Forms.Label();
			this.lblDO = new System.Windows.Forms.Label();
			this.picSensor = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.NmSensorFilter_1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NmSensorFilter_2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCamera)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picSensor)).BeginInit();
			this.SuspendLayout();
			// 
			// txtProcessName
			// 
			this.txtProcessName.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtProcessName.Location = new System.Drawing.Point(3, 22);
			this.txtProcessName.Name = "txtProcessName";
			this.txtProcessName.ReadOnly = true;
			this.txtProcessName.Size = new System.Drawing.Size(140, 25);
			this.txtProcessName.TabIndex = 0;
			// 
			// cmbTriggerType
			// 
			this.cmbTriggerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTriggerType.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbTriggerType.FormattingEnabled = true;
			this.cmbTriggerType.Location = new System.Drawing.Point(149, 21);
			this.cmbTriggerType.Name = "cmbTriggerType";
			this.cmbTriggerType.Size = new System.Drawing.Size(110, 26);
			this.cmbTriggerType.TabIndex = 1;
			// 
			// cmbSensorDevice_1
			// 
			this.cmbSensorDevice_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSensorDevice_1.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbSensorDevice_1.FormattingEnabled = true;
			this.cmbSensorDevice_1.Location = new System.Drawing.Point(149, 79);
			this.cmbSensorDevice_1.Name = "cmbSensorDevice_1";
			this.cmbSensorDevice_1.Size = new System.Drawing.Size(171, 26);
			this.cmbSensorDevice_1.TabIndex = 2;
			// 
			// cmbSensorDevice_2
			// 
			this.cmbSensorDevice_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSensorDevice_2.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbSensorDevice_2.FormattingEnabled = true;
			this.cmbSensorDevice_2.Location = new System.Drawing.Point(149, 111);
			this.cmbSensorDevice_2.Name = "cmbSensorDevice_2";
			this.cmbSensorDevice_2.Size = new System.Drawing.Size(171, 26);
			this.cmbSensorDevice_2.TabIndex = 2;
			this.cmbSensorDevice_2.Visible = false;
			// 
			// lblSensorFillterUnit_1
			// 
			this.lblSensorFillterUnit_1.AutoSize = true;
			this.lblSensorFillterUnit_1.Font = new System.Drawing.Font("MS UI Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lblSensorFillterUnit_1.Location = new System.Drawing.Point(395, 85);
			this.lblSensorFillterUnit_1.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.lblSensorFillterUnit_1.Name = "lblSensorFillterUnit_1";
			this.lblSensorFillterUnit_1.Size = new System.Drawing.Size(15, 14);
			this.lblSensorFillterUnit_1.TabIndex = 7;
			this.lblSensorFillterUnit_1.Text = "S";
			this.lblSensorFillterUnit_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// NmSensorFilter_1
			// 
			this.NmSensorFilter_1.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.NmSensorFilter_1.Location = new System.Drawing.Point(327, 80);
			this.NmSensorFilter_1.Margin = new System.Windows.Forms.Padding(4);
			this.NmSensorFilter_1.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.NmSensorFilter_1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.NmSensorFilter_1.Name = "NmSensorFilter_1";
			this.NmSensorFilter_1.Size = new System.Drawing.Size(60, 25);
			this.NmSensorFilter_1.TabIndex = 6;
			this.NmSensorFilter_1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// NmSensorFilter_2
			// 
			this.NmSensorFilter_2.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.NmSensorFilter_2.Location = new System.Drawing.Point(327, 112);
			this.NmSensorFilter_2.Margin = new System.Windows.Forms.Padding(4);
			this.NmSensorFilter_2.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this.NmSensorFilter_2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.NmSensorFilter_2.Name = "NmSensorFilter_2";
			this.NmSensorFilter_2.Size = new System.Drawing.Size(60, 25);
			this.NmSensorFilter_2.TabIndex = 6;
			this.NmSensorFilter_2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.NmSensorFilter_2.Visible = false;
			// 
			// lblSensorFillterUnit_2
			// 
			this.lblSensorFillterUnit_2.AutoSize = true;
			this.lblSensorFillterUnit_2.Font = new System.Drawing.Font("MS UI Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lblSensorFillterUnit_2.Location = new System.Drawing.Point(395, 117);
			this.lblSensorFillterUnit_2.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.lblSensorFillterUnit_2.Name = "lblSensorFillterUnit_2";
			this.lblSensorFillterUnit_2.Size = new System.Drawing.Size(15, 14);
			this.lblSensorFillterUnit_2.TabIndex = 7;
			this.lblSensorFillterUnit_2.Text = "S";
			this.lblSensorFillterUnit_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblSensorFillterUnit_2.Visible = false;
			// 
			// dgvCamera
			// 
			this.dgvCamera.AllowUserToAddRows = false;
			this.dgvCamera.AllowUserToDeleteRows = false;
			this.dgvCamera.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCamera.Location = new System.Drawing.Point(612, 3);
			this.dgvCamera.MultiSelect = false;
			this.dgvCamera.Name = "dgvCamera";
			this.dgvCamera.RowHeadersVisible = false;
			this.dgvCamera.RowHeadersWidth = 51;
			this.dgvCamera.RowTemplate.Height = 24;
			this.dgvCamera.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvCamera.Size = new System.Drawing.Size(350, 132);
			this.dgvCamera.TabIndex = 9;
			// 
			// cmbDODevice
			// 
			this.cmbDODevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDODevice.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbDODevice.FormattingEnabled = true;
			this.cmbDODevice.Location = new System.Drawing.Point(982, 21);
			this.cmbDODevice.Name = "cmbDODevice";
			this.cmbDODevice.Size = new System.Drawing.Size(171, 26);
			this.cmbDODevice.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 15);
			this.label1.TabIndex = 10;
			this.label1.Text = "工程名称";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(146, 3);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 15);
			this.label2.TabIndex = 10;
			this.label2.Text = "トリガ動作";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSensorTitle
			// 
			this.lblSensorTitle.AutoSize = true;
			this.lblSensorTitle.Location = new System.Drawing.Point(146, 61);
			this.lblSensorTitle.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.lblSensorTitle.Name = "lblSensorTitle";
			this.lblSensorTitle.Size = new System.Drawing.Size(54, 15);
			this.lblSensorTitle.TabIndex = 10;
			this.lblSensorTitle.Text = "センサ";
			this.lblSensorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblSensorFillterTitle
			// 
			this.lblSensorFillterTitle.AutoSize = true;
			this.lblSensorFillterTitle.Location = new System.Drawing.Point(324, 61);
			this.lblSensorFillterTitle.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.lblSensorFillterTitle.Name = "lblSensorFillterTitle";
			this.lblSensorFillterTitle.Size = new System.Drawing.Size(59, 15);
			this.lblSensorFillterTitle.TabIndex = 10;
			this.lblSensorFillterTitle.Text = "フィルター";
			this.lblSensorFillterTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblDO
			// 
			this.lblDO.AutoSize = true;
			this.lblDO.Location = new System.Drawing.Point(979, 3);
			this.lblDO.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
			this.lblDO.Name = "lblDO";
			this.lblDO.Size = new System.Drawing.Size(42, 15);
			this.lblDO.TabIndex = 11;
			this.lblDO.Text = "ブザー";
			this.lblDO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// picSensor
			// 
			this.picSensor.Location = new System.Drawing.Point(417, 4);
			this.picSensor.Name = "picSensor";
			this.picSensor.Size = new System.Drawing.Size(155, 131);
			this.picSensor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picSensor.TabIndex = 8;
			this.picSensor.TabStop = false;
			// 
			// UCDataCollectionItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.lblDO);
			this.Controls.Add(this.lblSensorFillterTitle);
			this.Controls.Add(this.lblSensorTitle);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dgvCamera);
			this.Controls.Add(this.picSensor);
			this.Controls.Add(this.lblSensorFillterUnit_2);
			this.Controls.Add(this.lblSensorFillterUnit_1);
			this.Controls.Add(this.NmSensorFilter_2);
			this.Controls.Add(this.NmSensorFilter_1);
			this.Controls.Add(this.cmbSensorDevice_2);
			this.Controls.Add(this.cmbDODevice);
			this.Controls.Add(this.cmbSensorDevice_1);
			this.Controls.Add(this.cmbTriggerType);
			this.Controls.Add(this.txtProcessName);
			this.Name = "UCDataCollectionItem";
			this.Size = new System.Drawing.Size(1183, 140);
			((System.ComponentModel.ISupportInitialize)(this.NmSensorFilter_1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NmSensorFilter_2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCamera)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picSensor)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtProcessName;
		private System.Windows.Forms.ComboBox cmbTriggerType;
		private System.Windows.Forms.ComboBox cmbSensorDevice_1;
		private System.Windows.Forms.ComboBox cmbSensorDevice_2;
		private System.Windows.Forms.Label lblSensorFillterUnit_1;
		private System.Windows.Forms.NumericUpDown NmSensorFilter_1;
		private System.Windows.Forms.NumericUpDown NmSensorFilter_2;
		private System.Windows.Forms.Label lblSensorFillterUnit_2;
		private System.Windows.Forms.PictureBox picSensor;
		private System.Windows.Forms.DataGridView dgvCamera;
		private System.Windows.Forms.ComboBox cmbDODevice;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblSensorTitle;
		private System.Windows.Forms.Label lblSensorFillterTitle;
		private System.Windows.Forms.Label lblDO;
	}
}
