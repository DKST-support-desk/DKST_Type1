
namespace PLOSMaintenance
{
	partial class UC_Device_DI
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRegist = new System.Windows.Forms.Button();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvSensorDeviceMst = new System.Windows.Forms.DataGridView();
            this.dgvClmDeleteImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.SensorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputNo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DeviceRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensorDeviceMst)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.tableLayoutPanel1);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 528);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(600, 72);
            this.pnlBottom.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.Controls.Add(this.btnRegist, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 72);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnRegist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRegist.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRegist.Location = new System.Drawing.Point(437, 2);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(161, 69);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "登録";
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.OnRegistButtonClicked);
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.btnAdd);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControl.Location = new System.Drawing.Point(0, 488);
            this.pnlControl.Margin = new System.Windows.Forms.Padding(2);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Padding = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.pnlControl.Size = new System.Drawing.Size(600, 40);
            this.pnlControl.TabIndex = 9;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAdd.Location = new System.Drawing.Point(7, 8);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(586, 24);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.OnAddButtonClicked);
            // 
            // dgvSensorDeviceMst
            // 
            this.dgvSensorDeviceMst.AllowUserToAddRows = false;
            this.dgvSensorDeviceMst.AllowUserToDeleteRows = false;
            this.dgvSensorDeviceMst.AllowUserToResizeColumns = false;
            this.dgvSensorDeviceMst.AllowUserToResizeRows = false;
            this.dgvSensorDeviceMst.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSensorDeviceMst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSensorDeviceMst.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvClmDeleteImage,
            this.SensorName,
            this.InputNo,
            this.DeviceRemark});
            this.dgvSensorDeviceMst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSensorDeviceMst.Location = new System.Drawing.Point(0, 0);
            this.dgvSensorDeviceMst.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSensorDeviceMst.Name = "dgvSensorDeviceMst";
            this.dgvSensorDeviceMst.RowHeadersVisible = false;
            this.dgvSensorDeviceMst.RowHeadersWidth = 62;
            this.dgvSensorDeviceMst.RowTemplate.Height = 27;
            this.dgvSensorDeviceMst.Size = new System.Drawing.Size(600, 488);
            this.dgvSensorDeviceMst.TabIndex = 7;
            this.dgvSensorDeviceMst.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellContentClicked);
            this.dgvSensorDeviceMst.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dgvSensorDeviceMst.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSensorDeviceMst_CellFormatting);
            // 
            // dgvClmDeleteImage
            // 
            this.dgvClmDeleteImage.FillWeight = 15F;
            this.dgvClmDeleteImage.HeaderText = "";
            this.dgvClmDeleteImage.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
            this.dgvClmDeleteImage.Name = "dgvClmDeleteImage";
            this.dgvClmDeleteImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SensorName
            // 
            this.SensorName.DataPropertyName = "SensorName";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.SensorName.DefaultCellStyle = dataGridViewCellStyle1;
            this.SensorName.FillWeight = 50F;
            this.SensorName.HeaderText = "センサ名";
            this.SensorName.Name = "SensorName";
            this.SensorName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InputNo
            // 
            this.InputNo.DataPropertyName = "InputNo";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.InputNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.InputNo.FillWeight = 40F;
            this.InputNo.HeaderText = "接点番号";
            this.InputNo.Name = "InputNo";
            this.InputNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DeviceRemark
            // 
            this.DeviceRemark.DataPropertyName = "DeviceRemark";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.DeviceRemark.DefaultCellStyle = dataGridViewCellStyle3;
            this.DeviceRemark.HeaderText = "機器説明";
            this.DeviceRemark.Name = "DeviceRemark";
            this.DeviceRemark.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeviceRemark.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // UC_Device_DI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvSensorDeviceMst);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.pnlBottom);
            this.Name = "UC_Device_DI";
            this.Size = new System.Drawing.Size(600, 600);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensorDeviceMst)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlControl;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.DataGridView dgvSensorDeviceMst;
        private System.Windows.Forms.DataGridViewImageColumn dgvClmDeleteImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn SensorName;
        private System.Windows.Forms.DataGridViewComboBoxColumn InputNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceRemark;
    }
}
