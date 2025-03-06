
namespace PLOSMaintenance
{
	partial class FrmExclusionClassMaintenance
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlDgv = new System.Windows.Forms.Panel();
            this.dgvEditTarget = new System.Windows.Forms.DataGridView();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tableLayoutPanel2.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlDgv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEditTarget)).BeginInit();
            this.pnlControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnOK, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 589);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(582, 60);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(385, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(194, 54);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelButtonClicked);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.Location = new System.Drawing.Point(175, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(194, 54);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "登録";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnRegistButtonClicked);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlDgv);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(582, 589);
            this.pnlMain.TabIndex = 3;
            // 
            // pnlDgv
            // 
            this.pnlDgv.Controls.Add(this.dgvEditTarget);
            this.pnlDgv.Controls.Add(this.pnlControl);
            this.pnlDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDgv.Location = new System.Drawing.Point(0, 0);
            this.pnlDgv.Name = "pnlDgv";
            this.pnlDgv.Padding = new System.Windows.Forms.Padding(8);
            this.pnlDgv.Size = new System.Drawing.Size(582, 589);
            this.pnlDgv.TabIndex = 1;
            // 
            // dgvEditTarget
            // 
            this.dgvEditTarget.AllowUserToAddRows = false;
            this.dgvEditTarget.AllowUserToDeleteRows = false;
            this.dgvEditTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEditTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEditTarget.Location = new System.Drawing.Point(8, 8);
            this.dgvEditTarget.Name = "dgvEditTarget";
            this.dgvEditTarget.RowHeadersVisible = false;
            this.dgvEditTarget.RowHeadersWidth = 51;
            this.dgvEditTarget.RowTemplate.Height = 24;
            this.dgvEditTarget.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEditTarget.Size = new System.Drawing.Size(566, 519);
            this.dgvEditTarget.TabIndex = 0;
            this.dgvEditTarget.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellContentClicked);
            this.dgvEditTarget.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEditTarget_CellEndEdit);
            this.dgvEditTarget.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvEditTarget_CellFormatting);
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.btnAdd);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControl.Location = new System.Drawing.Point(8, 527);
            this.pnlControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.pnlControl.Size = new System.Drawing.Size(566, 54);
            this.pnlControl.TabIndex = 7;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAdd.Location = new System.Drawing.Point(9, 10);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(548, 34);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.OnAddButtonClicked);
            // 
            // FrmExclusionClassMaintenance
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(582, 649);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "FrmExclusionClassMaintenance";
            this.Text = "除外区分登録";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlDgv.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEditTarget)).EndInit();
            this.pnlControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.Panel pnlDgv;
		private System.Windows.Forms.DataGridView dgvEditTarget;
		private System.Windows.Forms.Panel pnlControl;
		private System.Windows.Forms.Button btnAdd;
	}
}