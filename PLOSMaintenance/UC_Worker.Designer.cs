
namespace PLOSMaintenance
{
    partial class UC_Worker
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
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRegist = new System.Windows.Forms.Button();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dataGridViewWorker = new System.Windows.Forms.DataGridView();
            this.WorkerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkerRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWorker)).BeginInit();
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
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnRegist, 1, 0);
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
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
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
            // dataGridViewWorker
            // 
            this.dataGridViewWorker.AllowUserToAddRows = false;
            this.dataGridViewWorker.AllowUserToDeleteRows = false;
            this.dataGridViewWorker.AllowUserToResizeColumns = false;
            this.dataGridViewWorker.AllowUserToResizeRows = false;
            this.dataGridViewWorker.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewWorker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewWorker.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WorkerName,
            this.WorkerRemark});
            this.dataGridViewWorker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewWorker.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewWorker.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewWorker.Name = "dataGridViewWorker";
            this.dataGridViewWorker.RowHeadersVisible = false;
            this.dataGridViewWorker.RowHeadersWidth = 62;
            this.dataGridViewWorker.RowTemplate.Height = 27;
            this.dataGridViewWorker.Size = new System.Drawing.Size(600, 488);
            this.dataGridViewWorker.TabIndex = 7;
            this.dataGridViewWorker.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellContentClicked);
            this.dataGridViewWorker.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridViewWorker.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            // 
            // WorkerName
            // 
            this.WorkerName.DataPropertyName = "WorkerName";
            this.WorkerName.HeaderText = "作業者名";
            this.WorkerName.Name = "WorkerName";
            // 
            // WorkerRemark
            // 
            this.WorkerRemark.DataPropertyName = "WorkerRemark";
            this.WorkerRemark.HeaderText = "備考";
            this.WorkerRemark.Name = "WorkerRemark";
            // 
            // UC_Worker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewWorker);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.pnlBottom);
            this.Name = "UC_Worker";
            this.Size = new System.Drawing.Size(600, 600);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWorker)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.DataGridView dataGridViewWorker;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkerRemark;
    }
}
