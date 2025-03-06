
namespace PLOSMaintenance
{
	partial class UCLineInfo
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
            this.btnRegist = new System.Windows.Forms.Button();
            this.tlPnl = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLineName = new System.Windows.Forms.TextBox();
            this.numtxtOccupancyRate = new PLOS.Gui.Core.CustumContol.NumericTextBox();
            this.numtxtKnittingEfficiency = new PLOS.Gui.Core.CustumContol.NumericTextBox();
            this.numtxtCTResultRetentionDays = new PLOS.Gui.Core.CustumContol.NumericTextBox();
            this.numtxtMoveRetentionDays = new PLOS.Gui.Core.CustumContol.NumericTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlBottom.SuspendLayout();
            this.tlPnl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnRegist);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 427);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(640, 53);
            this.pnlBottom.TabIndex = 7;
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnRegist.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRegist.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRegist.Location = new System.Drawing.Point(538, 0);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(102, 53);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "登録";
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.OnRegistButtonClicked);
            // 
            // tlPnl
            // 
            this.tlPnl.AutoSize = true;
            this.tlPnl.ColumnCount = 1;
            this.tlPnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPnl.Controls.Add(this.label4, 0, 1);
            this.tlPnl.Controls.Add(this.label1, 0, 0);
            this.tlPnl.Controls.Add(this.label3, 0, 4);
            this.tlPnl.Controls.Add(this.label5, 0, 5);
            this.tlPnl.Controls.Add(this.label6, 0, 7);
            this.tlPnl.Controls.Add(this.label2, 0, 10);
            this.tlPnl.Controls.Add(this.label7, 0, 11);
            this.tlPnl.Controls.Add(this.label8, 0, 13);
            this.tlPnl.Controls.Add(this.txtLineName, 0, 2);
            this.tlPnl.Controls.Add(this.numtxtOccupancyRate, 0, 6);
            this.tlPnl.Controls.Add(this.numtxtKnittingEfficiency, 0, 8);
            this.tlPnl.Controls.Add(this.numtxtCTResultRetentionDays, 0, 12);
            this.tlPnl.Controls.Add(this.numtxtMoveRetentionDays, 0, 14);
            this.tlPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPnl.Location = new System.Drawing.Point(4, 4);
            this.tlPnl.Name = "tlPnl";
            this.tlPnl.RowCount = 18;
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.909912F));
            this.tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.01001F));
            this.tlPnl.Size = new System.Drawing.Size(632, 472);
            this.tlPnl.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(2, 37);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "ライン名";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "ウェブ画面表示ライン名";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(2, 107);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "戦略用判定値";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(2, 135);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "可動率　　(％)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(2, 184);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "編成効率　　(％)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(2, 254);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "データ保存期間";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(2, 282);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "計画・実績データ　　(日)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(2, 331);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "現象動画　　(日)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLineName
            // 
            this.txtLineName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.txtLineName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLineName.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtLineName.Location = new System.Drawing.Point(22, 51);
            this.txtLineName.Margin = new System.Windows.Forms.Padding(22, 2, 22, 2);
            this.txtLineName.MaxLength = 50;
            this.txtLineName.Name = "txtLineName";
            this.txtLineName.Size = new System.Drawing.Size(588, 26);
            this.txtLineName.TabIndex = 3;
            // 
            // numtxtOccupancyRate
            // 
            this.numtxtOccupancyRate.AllowDecimalPoint = true;
            this.numtxtOccupancyRate.AllowLeadingSign = true;
            this.numtxtOccupancyRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.numtxtOccupancyRate.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
            this.numtxtOccupancyRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numtxtOccupancyRate.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numtxtOccupancyRate.HideZero = false;
            this.numtxtOccupancyRate.Location = new System.Drawing.Point(22, 149);
            this.numtxtOccupancyRate.Margin = new System.Windows.Forms.Padding(22, 2, 22, 2);
            this.numtxtOccupancyRate.MaxFractionalDigits = ((uint)(1u));
            this.numtxtOccupancyRate.MaxIntegerDigits = ((uint)(8u));
            this.numtxtOccupancyRate.MaxValue = 100D;
            this.numtxtOccupancyRate.MinValue = 0D;
            this.numtxtOccupancyRate.Name = "numtxtOccupancyRate";
            this.numtxtOccupancyRate.Size = new System.Drawing.Size(588, 26);
            this.numtxtOccupancyRate.TabIndex = 4;
            this.numtxtOccupancyRate.Text = "0";
            this.numtxtOccupancyRate.ToolTip = null;
            this.numtxtOccupancyRate.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // numtxtKnittingEfficiency
            // 
            this.numtxtKnittingEfficiency.AllowDecimalPoint = true;
            this.numtxtKnittingEfficiency.AllowLeadingSign = true;
            this.numtxtKnittingEfficiency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.numtxtKnittingEfficiency.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
            this.numtxtKnittingEfficiency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numtxtKnittingEfficiency.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numtxtKnittingEfficiency.HideZero = false;
            this.numtxtKnittingEfficiency.Location = new System.Drawing.Point(22, 198);
            this.numtxtKnittingEfficiency.Margin = new System.Windows.Forms.Padding(22, 2, 22, 2);
            this.numtxtKnittingEfficiency.MaxFractionalDigits = ((uint)(1u));
            this.numtxtKnittingEfficiency.MaxIntegerDigits = ((uint)(8u));
            this.numtxtKnittingEfficiency.MaxValue = 100D;
            this.numtxtKnittingEfficiency.MinValue = 0D;
            this.numtxtKnittingEfficiency.Name = "numtxtKnittingEfficiency";
            this.numtxtKnittingEfficiency.Size = new System.Drawing.Size(588, 26);
            this.numtxtKnittingEfficiency.TabIndex = 5;
            this.numtxtKnittingEfficiency.Text = "0";
            this.numtxtKnittingEfficiency.ToolTip = null;
            this.numtxtKnittingEfficiency.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // numtxtCTResultRetentionDays
            // 
            this.numtxtCTResultRetentionDays.AllowDecimalPoint = true;
            this.numtxtCTResultRetentionDays.AllowLeadingSign = true;
            this.numtxtCTResultRetentionDays.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.numtxtCTResultRetentionDays.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
            this.numtxtCTResultRetentionDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numtxtCTResultRetentionDays.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numtxtCTResultRetentionDays.HideZero = false;
            this.numtxtCTResultRetentionDays.Location = new System.Drawing.Point(22, 296);
            this.numtxtCTResultRetentionDays.Margin = new System.Windows.Forms.Padding(22, 2, 22, 2);
            this.numtxtCTResultRetentionDays.MaxFractionalDigits = ((uint)(2u));
            this.numtxtCTResultRetentionDays.MaxIntegerDigits = ((uint)(8u));
            this.numtxtCTResultRetentionDays.MaxValue = 90D;
            this.numtxtCTResultRetentionDays.MinValue = 1D;
            this.numtxtCTResultRetentionDays.Name = "numtxtCTResultRetentionDays";
            this.numtxtCTResultRetentionDays.Size = new System.Drawing.Size(588, 26);
            this.numtxtCTResultRetentionDays.TabIndex = 5;
            this.numtxtCTResultRetentionDays.Text = "0";
            this.numtxtCTResultRetentionDays.ToolTip = null;
            this.numtxtCTResultRetentionDays.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // numtxtMoveRetentionDays
            // 
            this.numtxtMoveRetentionDays.AllowDecimalPoint = true;
            this.numtxtMoveRetentionDays.AllowLeadingSign = true;
            this.numtxtMoveRetentionDays.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.numtxtMoveRetentionDays.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
            this.numtxtMoveRetentionDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numtxtMoveRetentionDays.Font = new System.Drawing.Font("MS UI Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numtxtMoveRetentionDays.HideZero = false;
            this.numtxtMoveRetentionDays.Location = new System.Drawing.Point(22, 345);
            this.numtxtMoveRetentionDays.Margin = new System.Windows.Forms.Padding(22, 2, 22, 2);
            this.numtxtMoveRetentionDays.MaxFractionalDigits = ((uint)(2u));
            this.numtxtMoveRetentionDays.MaxIntegerDigits = ((uint)(8u));
            this.numtxtMoveRetentionDays.MaxValue = 90D;
            this.numtxtMoveRetentionDays.MinValue = 1D;
            this.numtxtMoveRetentionDays.Name = "numtxtMoveRetentionDays";
            this.numtxtMoveRetentionDays.Size = new System.Drawing.Size(588, 26);
            this.numtxtMoveRetentionDays.TabIndex = 5;
            this.numtxtMoveRetentionDays.Text = "0";
            this.numtxtMoveRetentionDays.ToolTip = null;
            this.numtxtMoveRetentionDays.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tlPnl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(640, 480);
            this.panel1.TabIndex = 9;
            // 
            // UCLineInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCLineInfo";
            this.Size = new System.Drawing.Size(640, 480);
            this.pnlBottom.ResumeLayout(false);
            this.tlPnl.ResumeLayout(false);
            this.tlPnl.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnRegist;
		private System.Windows.Forms.TableLayoutPanel tlPnl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtLineName;
		private PLOS.Gui.Core.CustumContol.NumericTextBox numtxtOccupancyRate;
		private PLOS.Gui.Core.CustumContol.NumericTextBox numtxtKnittingEfficiency;
		private PLOS.Gui.Core.CustumContol.NumericTextBox numtxtCTResultRetentionDays;
		private PLOS.Gui.Core.CustumContol.NumericTextBox numtxtMoveRetentionDays;
    }
}
