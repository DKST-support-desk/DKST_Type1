
namespace PLOSMaintenance
{
	partial class UCCompositionRow
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dgvProcess = new System.Windows.Forms.DataGridView();
            this.pnlDeleteComposition = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStandardVal = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtUniqueNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblNumberPeople = new System.Windows.Forms.Label();
            this.txtUniqueName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcess)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 6;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel.Controls.Add(this.dgvProcess, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.pnlDeleteComposition, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.btnStandardVal, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1136, 166);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // dgvProcess
            // 
            this.dgvProcess.AllowUserToAddRows = false;
            this.dgvProcess.AllowUserToDeleteRows = false;
            this.dgvProcess.ColumnHeadersHeight = 29;
            this.dgvProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProcess.Location = new System.Drawing.Point(274, 4);
            this.dgvProcess.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            this.dgvProcess.Name = "dgvProcess";
            this.dgvProcess.RowHeadersVisible = false;
            this.dgvProcess.RowHeadersWidth = 51;
            this.tableLayoutPanel.SetRowSpan(this.dgvProcess, 5);
            this.dgvProcess.RowTemplate.Height = 21;
            this.dgvProcess.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProcess.Size = new System.Drawing.Size(725, 158);
            this.dgvProcess.TabIndex = 4;
            this.dgvProcess.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProcess_CellEndEdit);
            // 
            // pnlDeleteComposition
            // 
            this.pnlDeleteComposition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlDeleteComposition.BackgroundImage = global::PLOSMaintenance.Properties.Resources.削除アイコン;
            this.pnlDeleteComposition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlDeleteComposition.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlDeleteComposition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDeleteComposition.Location = new System.Drawing.Point(4, 4);
            this.pnlDeleteComposition.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDeleteComposition.Name = "pnlDeleteComposition";
            this.tableLayoutPanel.SetRowSpan(this.pnlDeleteComposition, 5);
            this.pnlDeleteComposition.Size = new System.Drawing.Size(32, 158);
            this.pnlDeleteComposition.TabIndex = 5;
            this.pnlDeleteComposition.Click += new System.EventHandler(this.OnDeleteCompositionPanelClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(244, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this.label3.Name = "label3";
            this.tableLayoutPanel.SetRowSpan(this.label3, 5);
            this.label3.Size = new System.Drawing.Size(17, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "工程";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStandardVal
            // 
            this.btnStandardVal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStandardVal.Location = new System.Drawing.Point(1020, 4);
            this.btnStandardVal.Margin = new System.Windows.Forms.Padding(4);
            this.btnStandardVal.Name = "btnStandardVal";
            this.tableLayoutPanel.SetRowSpan(this.btnStandardVal, 3);
            this.btnStandardVal.Size = new System.Drawing.Size(112, 91);
            this.btnStandardVal.TabIndex = 9;
            this.btnStandardVal.Text = "CT標準値と\r\n機器の設定";
            this.btnStandardVal.UseVisualStyleBackColor = true;
            this.btnStandardVal.Click += new System.EventHandler(this.OnStandardValButton_Clicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.txtUniqueNo, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblNumberPeople, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtUniqueName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(40, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel.SetRowSpan(this.tableLayoutPanel1, 5);
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 166);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // txtUniqueNo
            // 
            this.txtUniqueNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUniqueNo.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtUniqueNo.Location = new System.Drawing.Point(0, 135);
            this.txtUniqueNo.Margin = new System.Windows.Forms.Padding(0);
            this.txtUniqueNo.MaxLength = 12;
            this.txtUniqueNo.Name = "txtUniqueNo";
            this.txtUniqueNo.Size = new System.Drawing.Size(200, 23);
            this.txtUniqueNo.TabIndex = 4;
            this.txtUniqueNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUniqueNo_KeyPress);
            this.txtUniqueNo.Leave += new System.EventHandler(this.txtUniqueNo_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(0, 108);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 27);
            this.label5.TabIndex = 3;
            this.label5.Text = "編成No";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNumberPeople
            // 
            this.lblNumberPeople.AutoSize = true;
            this.lblNumberPeople.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNumberPeople.Location = new System.Drawing.Point(0, 27);
            this.lblNumberPeople.Margin = new System.Windows.Forms.Padding(0);
            this.lblNumberPeople.Name = "lblNumberPeople";
            this.lblNumberPeople.Size = new System.Drawing.Size(200, 27);
            this.lblNumberPeople.TabIndex = 0;
            this.lblNumberPeople.Text = "30";
            this.lblNumberPeople.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUniqueName
            // 
            this.txtUniqueName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUniqueName.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtUniqueName.Location = new System.Drawing.Point(0, 81);
            this.txtUniqueName.Margin = new System.Windows.Forms.Padding(0);
            this.txtUniqueName.MaxLength = 50;
            this.txtUniqueName.Name = "txtUniqueName";
            this.txtUniqueName.Size = new System.Drawing.Size(200, 23);
            this.txtUniqueName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "編成人数";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "編成名";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UCCompositionRow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UCCompositionRow";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(1150, 180);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProcess)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel pnlDeleteComposition;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtUniqueNo;
        public System.Windows.Forms.DataGridView dgvProcess;
        public System.Windows.Forms.TextBox txtUniqueName;
        public System.Windows.Forms.Label lblNumberPeople;
        public System.Windows.Forms.Button btnStandardVal;
    }
}
