namespace PLOS.Gui.Core.Base
{
	partial class FormSizableDialogBase
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSizableDialogBase));
			this.pnlBtm = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.pnlBtm.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBtm
			// 
			resources.ApplyResources(this.pnlBtm, "pnlBtm");
			this.pnlBtm.Controls.Add(this.btnCancel);
			this.pnlBtm.Controls.Add(this.btnOk);
			this.pnlBtm.Name = "pnlBtm";
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// FormSizableDialogBase
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.pnlBtm);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSizableDialogBase";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.pnlBtm.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel pnlBtm;
		protected System.Windows.Forms.Button btnCancel;
		protected System.Windows.Forms.Button btnOk;
	}
}
