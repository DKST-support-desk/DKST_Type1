
namespace PLOSMaintenance
{
	partial class UCOperating_Shift_Exclusion_Row
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.chkbkEndNextDay = new System.Windows.Forms.CheckBox();
            this.StartTime = new System.Windows.Forms.DateTimePicker();
            this.chkbkStartNextDay = new System.Windows.Forms.CheckBox();
            this.EndTime = new System.Windows.Forms.DateTimePicker();
            this.cmbExclusionClass = new System.Windows.Forms.ComboBox();
            this.txtExclusionRemark = new System.Windows.Forms.TextBox();
            this.chkExclusionType = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "翌";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkbkEndNextDay, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.StartTime, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkbkStartNextDay, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.EndTime, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbExclusionClass, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtExclusionRemark, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkExclusionType, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(394, 45);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(141, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "翌";
            // 
            // chkbkEndNextDay
            // 
            this.chkbkEndNextDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkbkEndNextDay.AutoSize = true;
            this.chkbkEndNextDay.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkbkEndNextDay.Location = new System.Drawing.Point(141, 18);
            this.chkbkEndNextDay.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chkbkEndNextDay.Name = "chkbkEndNextDay";
            this.chkbkEndNextDay.Size = new System.Drawing.Size(19, 14);
            this.chkbkEndNextDay.TabIndex = 8;
            this.chkbkEndNextDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkbkEndNextDay.UseVisualStyleBackColor = true;
            // 
            // StartTime
            // 
            this.StartTime.CustomFormat = "HH : mm";
            this.StartTime.Enabled = false;
            this.StartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.StartTime.Location = new System.Drawing.Point(53, 15);
            this.StartTime.Name = "StartTime";
            this.StartTime.ShowUpDown = true;
            this.StartTime.Size = new System.Drawing.Size(82, 19);
            this.StartTime.TabIndex = 4;
            // 
            // chkbkStartNextDay
            // 
            this.chkbkStartNextDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkbkStartNextDay.AutoSize = true;
            this.chkbkStartNextDay.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkbkStartNextDay.Location = new System.Drawing.Point(28, 18);
            this.chkbkStartNextDay.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chkbkStartNextDay.Name = "chkbkStartNextDay";
            this.chkbkStartNextDay.Size = new System.Drawing.Size(19, 14);
            this.chkbkStartNextDay.TabIndex = 0;
            this.chkbkStartNextDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkbkStartNextDay.UseVisualStyleBackColor = true;
            // 
            // EndTime
            // 
            this.EndTime.CustomFormat = " HH : mm";
            this.EndTime.Enabled = false;
            this.EndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.EndTime.Location = new System.Drawing.Point(166, 15);
            this.EndTime.Name = "EndTime";
            this.EndTime.ShowUpDown = true;
            this.EndTime.Size = new System.Drawing.Size(82, 19);
            this.EndTime.TabIndex = 4;
            // 
            // cmbExclusionClass
            // 
            this.cmbExclusionClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExclusionClass.Enabled = false;
            this.cmbExclusionClass.FormattingEnabled = true;
            this.cmbExclusionClass.Location = new System.Drawing.Point(254, 14);
            this.cmbExclusionClass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.cmbExclusionClass.Name = "cmbExclusionClass";
            this.cmbExclusionClass.Size = new System.Drawing.Size(90, 20);
            this.cmbExclusionClass.TabIndex = 5;
            // 
            // txtExclusionRemark
            // 
            this.txtExclusionRemark.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExclusionRemark.Enabled = false;
            this.txtExclusionRemark.Location = new System.Drawing.Point(350, 15);
            this.txtExclusionRemark.Name = "txtExclusionRemark";
            this.txtExclusionRemark.Size = new System.Drawing.Size(41, 19);
            this.txtExclusionRemark.TabIndex = 6;
            // 
            // chkExclusionType
            // 
            this.chkExclusionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkExclusionType.AutoSize = true;
            this.chkExclusionType.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkExclusionType.Location = new System.Drawing.Point(3, 18);
            this.chkExclusionType.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.chkExclusionType.Name = "chkExclusionType";
            this.chkExclusionType.Size = new System.Drawing.Size(19, 14);
            this.chkExclusionType.TabIndex = 7;
            this.chkExclusionType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkExclusionType.UseVisualStyleBackColor = true;
            this.chkExclusionType.CheckedChanged += new System.EventHandler(this.OnExclusionTypeCheckedChanged);
            // 
            // UCOperating_Shift_Exclusion_Row
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UCOperating_Shift_Exclusion_Row";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.Size = new System.Drawing.Size(400, 45);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox chkbkEndNextDay;
        private System.Windows.Forms.DateTimePicker StartTime;
        private System.Windows.Forms.CheckBox chkbkStartNextDay;
        private System.Windows.Forms.DateTimePicker EndTime;
        public System.Windows.Forms.ComboBox cmbExclusionClass;
        private System.Windows.Forms.TextBox txtExclusionRemark;
        private System.Windows.Forms.CheckBox chkExclusionType;
        private System.Windows.Forms.Label label2;
    }
}
