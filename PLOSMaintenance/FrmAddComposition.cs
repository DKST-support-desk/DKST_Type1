using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBConnect;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
    /// <summary>
    /// 編成追加フォーム
    /// </summary>
	public partial class FrmAddComposition : Form
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバー変数"
		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion "メンバー変数"

		//********************************************
		//* プロパティ
		//********************************************
		#region "プロパティ"
		/// <summary>
		/// 編成ID
		/// </summary>
		public Guid CompositionId { get; set; } = Guid.Empty;

        /// <summary>
        /// 編成人数
        /// </summary>
        public decimal numPeopleOrganize { get; set; }
        #endregion "プロパティ"

        /// <summary>
        /// 編成追加フォーム
        /// </summary>
        public FrmAddComposition()
		{
            try
            {
				InitializeComponent();
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 編成追加ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOKClicked(object sender, EventArgs e)
		{
            try
            {
                if (NumberOfPeople.Value < 1 || 30 < NumberOfPeople.Value)
                {
					logger.Warn("編成人数が範囲外の値です。");
					MessageBox.Show("編成人数が範囲外の値です。", "追加エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
					// ダイアログ結果取得
					this.DialogResult = DialogResult.OK;

					logger.Info("編成追加画面　編成追加ボタン押下");

					// 編成ID生成
					CompositionId = Guid.NewGuid();

					// 編成人数保持
					numPeopleOrganize = NumberOfPeople.Value;
				}
            }
			catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            finally
            {
                Close();
            }
		}

		/// <summary>
		/// キャンセルボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClicked(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			logger.Info("編成追加画面　キャンセルボタン押下");
			Close();
		}
	}
}
