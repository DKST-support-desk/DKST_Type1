
namespace PLOSMaintenance
{
    partial class UCOperating_Shift_Calendar_Day_Plan
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblShift1 = new System.Windows.Forms.Label();
            this.lblShift2 = new System.Windows.Forms.Label();
            this.lblShift3 = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tableLayoutPanel1);
            this.pnlMain.Controls.SetChildIndex(this.lblDay, 0);
            this.pnlMain.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lblShift1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblShift2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblShift3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 46);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(144, 71);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblShift1
            // 
            this.lblShift1.AutoSize = true;
            this.lblShift1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblShift1.Location = new System.Drawing.Point(3, 3);
            this.lblShift1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblShift1.Name = "lblShift1";
            this.lblShift1.Size = new System.Drawing.Size(138, 20);
            this.lblShift1.TabIndex = 0;
            this.lblShift1.Text = "1直 ---";
            this.lblShift1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblShift1.Click += new System.EventHandler(this.OnlblShiftClicked);
            // 
            // lblShift2
            // 
            this.lblShift2.AutoSize = true;
            this.lblShift2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblShift2.Location = new System.Drawing.Point(3, 26);
            this.lblShift2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblShift2.Name = "lblShift2";
            this.lblShift2.Size = new System.Drawing.Size(138, 20);
            this.lblShift2.TabIndex = 0;
            this.lblShift2.Text = "2直 ---";
            this.lblShift2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblShift2.Click += new System.EventHandler(this.OnlblShiftClicked);
            // 
            // lblShift3
            // 
            this.lblShift3.AutoSize = true;
            this.lblShift3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblShift3.Location = new System.Drawing.Point(3, 49);
            this.lblShift3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblShift3.Name = "lblShift3";
            this.lblShift3.Size = new System.Drawing.Size(138, 22);
            this.lblShift3.TabIndex = 0;
            this.lblShift3.Text = "3直 ---";
            this.lblShift3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblShift3.Click += new System.EventHandler(this.OnlblShiftClicked);
            // 
            // UCOperating_Shift_Calendar_Day_Plan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UCOperating_Shift_Calendar_Day_Plan";
            this.pnlMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblShift1;
        private System.Windows.Forms.Label lblShift2;
        private System.Windows.Forms.Label lblShift3;
    }
}
