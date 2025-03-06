
namespace PLOSMaintenance
{
	partial class UCWorker
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
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.btnRegist = new System.Windows.Forms.Button();
			this.pnlControl = new System.Windows.Forms.Panel();
			this.btnAdd = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.pnlBottom.SuspendLayout();
			this.pnlControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeColumns = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 0);
			this.dataGridView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.RowHeadersWidth = 62;
			this.dataGridView.RowTemplate.Height = 27;
			this.dataGridView.Size = new System.Drawing.Size(800, 630);
			this.dataGridView.TabIndex = 4;
			this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellContentClicked);
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.btnRegist);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 684);
			this.pnlBottom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(800, 66);
			this.pnlBottom.TabIndex = 5;
			// 
			// btnRegist
			// 
			this.btnRegist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnRegist.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnRegist.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnRegist.Location = new System.Drawing.Point(664, 0);
			this.btnRegist.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnRegist.Name = "btnRegist";
			this.btnRegist.Size = new System.Drawing.Size(136, 66);
			this.btnRegist.TabIndex = 0;
			this.btnRegist.Text = "登録";
			this.btnRegist.UseVisualStyleBackColor = false;
			this.btnRegist.Click += new System.EventHandler(this.OnRegistButtonClicked);
			// 
			// pnlControl
			// 
			this.pnlControl.Controls.Add(this.btnAdd);
			this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlControl.Location = new System.Drawing.Point(0, 630);
			this.pnlControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.pnlControl.Name = "pnlControl";
			this.pnlControl.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
			this.pnlControl.Size = new System.Drawing.Size(800, 54);
			this.pnlControl.TabIndex = 6;
			// 
			// btnAdd
			// 
			this.btnAdd.BackColor = System.Drawing.Color.White;
			this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnAdd.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.btnAdd.Location = new System.Drawing.Point(9, 10);
			this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(782, 34);
			this.btnAdd.TabIndex = 0;
			this.btnAdd.Text = "+";
			this.btnAdd.UseVisualStyleBackColor = false;
			this.btnAdd.Click += new System.EventHandler(this.OnAddButtonClicked);
			// 
			// UCWorker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.pnlControl);
			this.Controls.Add(this.pnlBottom);
			this.Name = "UCWorker";
			this.Size = new System.Drawing.Size(800, 750);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			this.pnlControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnRegist;
		private System.Windows.Forms.Panel pnlControl;
		private System.Windows.Forms.Button btnAdd;
	}
}
