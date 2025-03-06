
namespace PLOSMaintenance
{
	partial class UCProductType
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvProductTypeMst = new System.Windows.Forms.DataGridView();
            this.dgvClmDeleteImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.dgvClmTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.upButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.downButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnRegist = new System.Windows.Forms.Button();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductTypeMst)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.pnlControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvProductTypeMst
            // 
            this.dgvProductTypeMst.AllowUserToAddRows = false;
            this.dgvProductTypeMst.AllowUserToDeleteRows = false;
            this.dgvProductTypeMst.AllowUserToResizeColumns = false;
            this.dgvProductTypeMst.AllowUserToResizeRows = false;
            this.dgvProductTypeMst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductTypeMst.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvClmDeleteImage,
            this.dgvClmTypeName,
            this.upButton,
            this.downButton});
            this.dgvProductTypeMst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductTypeMst.Location = new System.Drawing.Point(2, 3);
            this.dgvProductTypeMst.Margin = new System.Windows.Forms.Padding(2);
            this.dgvProductTypeMst.Name = "dgvProductTypeMst";
            this.dgvProductTypeMst.RowHeadersVisible = false;
            this.dgvProductTypeMst.RowHeadersWidth = 62;
            this.dgvProductTypeMst.RowTemplate.Height = 27;
            this.dgvProductTypeMst.Size = new System.Drawing.Size(596, 498);
            this.dgvProductTypeMst.TabIndex = 1;
            this.dgvProductTypeMst.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellContentClicked);
            this.dgvProductTypeMst.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProductTypeMst_CellEndEdit);
            this.dgvProductTypeMst.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvProductTypeMst_CellFormatting);
            this.dgvProductTypeMst.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvProductTypeMst_DataBindingComplete);
            this.dgvProductTypeMst.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvProductTypeMst_KeyDown);
            // 
            // dgvClmDeleteImage
            // 
            this.dgvClmDeleteImage.FillWeight = 25F;
            this.dgvClmDeleteImage.HeaderText = "";
            this.dgvClmDeleteImage.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dgvClmDeleteImage.Name = "dgvClmDeleteImage";
            this.dgvClmDeleteImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dgvClmTypeName
            // 
            this.dgvClmTypeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvClmTypeName.DataPropertyName = "ProductTypeName";
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvClmTypeName.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvClmTypeName.FillWeight = 150F;
            this.dgvClmTypeName.HeaderText = "タイプ名";
            this.dgvClmTypeName.MaxInputLength = 127;
            this.dgvClmTypeName.Name = "dgvClmTypeName";
            // 
            // upButton
            // 
            this.upButton.FillWeight = 25F;
            this.upButton.HeaderText = "";
            this.upButton.Name = "upButton";
            this.upButton.ReadOnly = true;
            this.upButton.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.upButton.Text = "上へ";
            this.upButton.UseColumnTextForButtonValue = true;
            // 
            // downButton
            // 
            this.downButton.FillWeight = 25F;
            this.downButton.HeaderText = "";
            this.downButton.Name = "downButton";
            this.downButton.ReadOnly = true;
            this.downButton.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.downButton.Text = "下へ";
            this.downButton.UseColumnTextForButtonValue = true;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnRegist);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(2, 544);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(596, 53);
            this.pnlBottom.TabIndex = 2;
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnRegist.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRegist.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRegist.Location = new System.Drawing.Point(494, 0);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(102, 53);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "登録";
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.OnRegistButtonClicked);
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.btnAdd);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControl.Location = new System.Drawing.Point(2, 501);
            this.pnlControl.Margin = new System.Windows.Forms.Padding(2);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Padding = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.pnlControl.Size = new System.Drawing.Size(596, 43);
            this.pnlControl.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.White;
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAdd.Location = new System.Drawing.Point(7, 8);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(582, 27);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.OnAddButtonClicked);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn1.FillWeight = 150F;
            this.dataGridViewTextBoxColumn1.HeaderText = "タイプ名";
            this.dataGridViewTextBoxColumn1.MaxInputLength = 127;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // UCProductType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvProductTypeMst);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.pnlBottom);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCProductType";
            this.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Size = new System.Drawing.Size(600, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductTypeMst)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvProductTypeMst;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnRegist;
		private System.Windows.Forms.Panel pnlControl;
		private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewImageColumn dgvClmDeleteImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvClmTypeName;
        private System.Windows.Forms.DataGridViewButtonColumn upButton;
        private System.Windows.Forms.DataGridViewButtonColumn downButton;
    }
}
