
namespace PLOSMaintenance
{
	partial class FrmCalendar_MultiSelect
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCalendar_MultiSelect));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAllWeekdays = new System.Windows.Forms.Button();
            this.btnAllSaturday = new System.Windows.Forms.Button();
            this.btnResetSelect = new System.Windows.Forms.Button();
            this.ucCalendar_MultiSelect = new PLOSMaintenance.UCOperating_Shift_Calendar_MultiSelect();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlBottom.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.tableLayoutPanel2);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.pnlBottom.Location = new System.Drawing.Point(0, 693);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1182, 60);
            this.pnlBottom.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnOK, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1182, 60);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(935, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(244, 54);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
            // 
            // btnOK
            // 
            this.btnOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.Location = new System.Drawing.Point(665, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(244, 54);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "計画登録";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOkClicked);
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.tableLayoutPanel1);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1182, 60);
            this.pnlTop.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.Controls.Add(this.btnAllWeekdays, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAllSaturday, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnResetSelect, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1182, 60);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnAllWeekdays
            // 
            this.btnAllWeekdays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllWeekdays.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAllWeekdays.Location = new System.Drawing.Point(3, 3);
            this.btnAllWeekdays.Name = "btnAllWeekdays";
            this.btnAllWeekdays.Size = new System.Drawing.Size(244, 54);
            this.btnAllWeekdays.TabIndex = 0;
            this.btnAllWeekdays.Text = "平日全選択";
            this.btnAllWeekdays.UseVisualStyleBackColor = true;
            this.btnAllWeekdays.Click += new System.EventHandler(this.OnAllWeekdaysClicked);
            // 
            // btnAllSaturday
            // 
            this.btnAllSaturday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllSaturday.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAllSaturday.Location = new System.Drawing.Point(283, 3);
            this.btnAllSaturday.Name = "btnAllSaturday";
            this.btnAllSaturday.Size = new System.Drawing.Size(244, 54);
            this.btnAllSaturday.TabIndex = 0;
            this.btnAllSaturday.Text = "土曜全選択";
            this.btnAllSaturday.UseVisualStyleBackColor = true;
            this.btnAllSaturday.Click += new System.EventHandler(this.OnAllSaturdayClicked);
            // 
            // btnResetSelect
            // 
            this.btnResetSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnResetSelect.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnResetSelect.Location = new System.Drawing.Point(935, 3);
            this.btnResetSelect.Name = "btnResetSelect";
            this.btnResetSelect.Size = new System.Drawing.Size(244, 54);
            this.btnResetSelect.TabIndex = 0;
            this.btnResetSelect.Text = "選択解除";
            this.btnResetSelect.UseVisualStyleBackColor = true;
            this.btnResetSelect.Click += new System.EventHandler(this.OnResetSelectClicked);
            // 
            // ucCalendar_MultiSelect
            // 
            this.ucCalendar_MultiSelect.BackColor = System.Drawing.SystemColors.Control;
            this.ucCalendar_MultiSelect.DaysFont = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ucCalendar_MultiSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCalendar_MultiSelect.Location = new System.Drawing.Point(0, 60);
            this.ucCalendar_MultiSelect.MultiSelect = true;
            this.ucCalendar_MultiSelect.Name = "ucCalendar_MultiSelect";
            this.ucCalendar_MultiSelect.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.ucCalendar_MultiSelect.SelectDaysBackColor = System.Drawing.Color.LightBlue;
            this.ucCalendar_MultiSelect.SelectedDaysList = ((System.Collections.Generic.List<System.DateTime>)(resources.GetObject("ucCalendar_MultiSelect.SelectedDaysList")));
            this.ucCalendar_MultiSelect.Size = new System.Drawing.Size(1182, 633);
            this.ucCalendar_MultiSelect.TabIndex = 1;
            this.ucCalendar_MultiSelect.TargetMonth = new System.DateTime(2021, 5, 1, 0, 0, 0, 0);
            this.ucCalendar_MultiSelect.UseGuide = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(646, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "※範囲選択：Shiftキーを押しながら開始日と終了日をクリックしてください。";
            // 
            // FrmCalendar_MultiSelect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1182, 753);
            this.ControlBox = false;
            this.Controls.Add(this.ucCalendar_MultiSelect);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCalendar_MultiSelect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "計画複製";
            this.pnlBottom.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlBottom;
		private UCOperating_Shift_Calendar_MultiSelect ucCalendar_MultiSelect;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnAllWeekdays;
		private System.Windows.Forms.Button btnAllSaturday;
		private System.Windows.Forms.Button btnResetSelect;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
    }
}