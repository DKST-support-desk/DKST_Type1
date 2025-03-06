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
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
	/// <summary>
	/// ライン情報登録画面
	/// </summary>
	public partial class UCLineInfo : UserControl
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバー変数"
		/// <summary>
		/// DBコネクション
		/// </summary>
		SqlLineInfoMst SqlLineInfoMst;

		/// <summary>
		/// データテーブル
		/// </summary>
		private DataTable mDtLineInfoMst;

		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion "メンバー変数"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region コンストラクタ
		public UCLineInfo()
		{
            try
            {
				InitializeComponent();

				SqlLineInfoMst = new SqlLineInfoMst(Properties.Settings.Default.ConnectionString_New);

				DispData();
				SetEvent();
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion コンストラクタ

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// 登録ボタン押下時の処理です。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			try
			{
				logger.Info("登録ボタン押下");

				// 入力チェック
				if (CheckInput() == false)
				{
					return;
				}

				logger.Info("現在の情報で登録してよろしいですか？");

				if (DialogResult.Yes != MessageBox.Show("現在の情報で登録してよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					logger.Info("No押下");
					return;
				}
				logger.Info("Yes押下");

				// 変更前のデータ行を保持
				DataTable oldDt = mDtLineInfoMst.Copy();

				// 変更用のデータ行を保持
				if (mDtLineInfoMst.Rows.Count == 0)
				{
					// 行を追加
					mDtLineInfoMst.Rows.Add();
				}
				DataRow dr = mDtLineInfoMst.Rows[0];

				// 各項目に値を設定
				dr[ColumnLineInfoMst.LINE_NAME] = txtLineName.Text;
				dr[ColumnLineInfoMst.OCCUPANCY_RATE] = numtxtOccupancyRate.Text;
				dr[ColumnLineInfoMst.COMPOSITION_EFFICIENCY] = numtxtKnittingEfficiency.Text;
				dr[ColumnLineInfoMst.RESULT_SAVE_SPAN] = numtxtCTResultRetentionDays.Text;
				dr[ColumnLineInfoMst.MOVIE_SAVE_SPAN] = numtxtMoveRetentionDays.Text;

				// データがあれば更新、無ければ挿入
				if (CheckDBRow(oldDt))
				{
                    if (!SqlLineInfoMst.Update(mDtLineInfoMst))
                    {
						MessageBox.Show("登録に失敗しました。", "登録失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
						logger.Info("Update処理に失敗しました。");
						return;
					}

					logger.Info("Update処理を実行しました。");
				}
				else
				{
					if (!SqlLineInfoMst.Insert(mDtLineInfoMst))
                    {
						MessageBox.Show("登録に失敗しました。", "登録失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
						logger.Info("Insert処理に失敗しました。");
						return;
					}

					logger.Info("Insert処理を実行しました。");
				}

				TextBox[] textBoxes = { txtLineName, numtxtOccupancyRate, numtxtKnittingEfficiency,
										numtxtCTResultRetentionDays, numtxtMoveRetentionDays };

				foreach (TextBox txt in textBoxes)
				{
					txt.ForeColor = Color.FromArgb(0, 0, 0);
				}

				logger.Info("登録しました。");
				MessageBox.Show("登録しました。", "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

				// 編集フラグを消す
				FrmMain.gIsDataChange = false;
				
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + ", " + ex.StackTrace);
			}
		}

		/// <summary>
		/// 入力範囲をチェックします。(整数値)
		/// </summary>
		private bool CheckValue(int value)
		{
			bool check = false;

            try
            {
				if (0 < value && value <= 90)
				{
					check = true;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return check;
		}

		/// <summary>
		/// 入力範囲をチェックします。(小数値)
		/// </summary>
		private bool CheckValueDec(decimal value)
		{
			bool check = false;

            try
            {
				if (0 <= value && value <= 100)
				{
					check = true;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return check;
		}

		/// <summary>
		/// データの有無を判断します。
		/// </summary>
		/// <param name="dt">データテーブル</param>
		/// <returns></returns>
		private bool CheckDBRow(DataTable dt)
		{
			bool frag = false;

            try
            {
				if (dt.Rows.Count != 0)
				{
					frag = true;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return frag;
		}

		/// <summary>
		/// 文字色変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChangeTextColor(object sender, EventArgs e)
		{
            try
            {
				var txt = (TextBox)sender;
				txt.ForeColor = Color.FromArgb(0, 0, 255);

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
			}
		}

		/// <summary>
		/// 値の修正
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FixValue(object sender, EventArgs e)
        {
            try
            {
				var txt = (TextBox)sender;
				string[] txtBox = { ".0", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9" };

				if (txtBox.Contains(txt.Text))
                {
					txt.Text = "0" + txt.Text;
                }
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
            }
        }
		#endregion "イベント"

		//********************************************
		//* パブリックメソッド
		//********************************************
		#region パブリックメソッド
		/// <summary>
		/// データを表示します。
		/// </summary>
		public void DispData()
		{
            try
            {
				mDtLineInfoMst = SqlLineInfoMst.Select();

				if (mDtLineInfoMst != null && 0 < mDtLineInfoMst.Rows.Count)
				{
					// ライン名
					DataRow dr = mDtLineInfoMst.Rows[0];
					txtLineName.Text = dr.Field<String>(ColumnLineInfoMst.LINE_NAME);

					// 可動率
					Decimal occupancyRate = dr.Field<Decimal>(ColumnLineInfoMst.OCCUPANCY_RATE);
					numtxtOccupancyRate.Text = occupancyRate.ToString();

					// 編成効率
					Decimal compositionEfficiency = dr.Field<Decimal>(ColumnLineInfoMst.COMPOSITION_EFFICIENCY);
					numtxtKnittingEfficiency.Text = compositionEfficiency.ToString();

					// 計画・実績データ
					numtxtCTResultRetentionDays.Text = dr.Field<int>(ColumnLineInfoMst.RESULT_SAVE_SPAN).ToString();

					// 現象動画
					numtxtMoveRetentionDays.Text = dr.Field<int>(ColumnLineInfoMst.MOVIE_SAVE_SPAN).ToString();

					logger.Debug("値を表示しました。");
				}
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				return;
			}
		}
		#endregion パブリックメソッド

		//********************************************
		//* Private Method
		//********************************************
		#region Private Methods
		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <returns></returns>
		private Boolean CheckInput()
		{
			if (String.IsNullOrWhiteSpace(txtLineName.Text as string))
			{
				logger.Warn("ライン名を入力してください。");
				MessageBox.Show(this, "ライン名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
            else if(50 < txtLineName.Text.Length)
            {
				logger.Warn("ライン名の文字数が範囲外です。");
				MessageBox.Show(this, "ライン名の文字数が範囲外です。\nライン名は50文字以内で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			
			if (String.IsNullOrWhiteSpace(numtxtOccupancyRate.Text as string))
			{
				logger.Warn("可動率を入力してください。");
				MessageBox.Show(this, "可動率を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
            else
            {
				if (decimal.TryParse(numtxtOccupancyRate.Text, out decimal numOR))
				{
					if (!CheckValueDec(numOR))
					{
						logger.Warn("可動率が範囲外の入力値です。");
						MessageBox.Show("可動率が範囲外の入力値です。\n可動率は0～100の範囲で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
				else
				{
					logger.Warn("可動率の入力値が異常です。");
					MessageBox.Show("可動率の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (String.IsNullOrWhiteSpace(numtxtKnittingEfficiency.Text as string))
			{
				logger.Warn("編成効率を入力してください。");
				MessageBox.Show(this, "編成効率を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
            else
            {
				if (decimal.TryParse(numtxtKnittingEfficiency.Text, out decimal numKE))
				{
					if (!CheckValueDec(numKE))
					{
						logger.Warn("編成効率が範囲外の入力値です。");
						MessageBox.Show("編成効率が範囲外の入力値です。\n編成効率は0～100の範囲で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
				else
				{
					logger.Warn("編成効率の入力値が異常です。");
					MessageBox.Show("編成効率の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (String.IsNullOrWhiteSpace(numtxtCTResultRetentionDays.Text as string))
			{
				logger.Warn("計画・実績データ保存期間を入力してください。");
				MessageBox.Show(this, "計画・実績データ保存期間を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
            else
            {
				if (int.TryParse(numtxtCTResultRetentionDays.Text, out int numCTRRD))
				{
					if (!CheckValue(numCTRRD))
					{
						logger.Warn("計画・実績データ保存期間が範囲外の入力値です。");
						MessageBox.Show("計画・実績データ保存期間が範囲外の入力値です。\n計画・実績データ保存期間は1～90の範囲で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
				else
				{
					logger.Warn("計画・実績データ保存期間の入力値が異常です。");
					MessageBox.Show("計画・実績データ保存期間の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (String.IsNullOrWhiteSpace(numtxtMoveRetentionDays.Text as string))
			{
				logger.Warn("現象動画保存期間を入力してください。");
				MessageBox.Show(this, "現象動画保存期間を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
            else
            {
				if (int.TryParse(numtxtMoveRetentionDays.Text, out int numMRD))
				{
					if (!CheckValue(numMRD))
					{
						logger.Warn("現象動画保存期間が範囲外の入力値です。");
						MessageBox.Show("現象動画保存期間が範囲外の入力値です。\n現象動画保存期間は1～90の範囲で入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
				else
				{
					logger.Warn("現象動画保存期間の入力値が異常です。");
					MessageBox.Show("現象動画保存期間の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// イベント設定
		/// </summary>
		private void SetEvent()
        {
			try
			{
				TextBox[] textBoxes = { txtLineName, numtxtOccupancyRate, numtxtKnittingEfficiency, numtxtCTResultRetentionDays, numtxtMoveRetentionDays };
				foreach (TextBox txtbox in textBoxes)
				{
					txtbox.Font = new Font("MS UI Gothic", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));

					// 文字色変更イベント設定
					txtbox.LostFocus += new EventHandler(ChangeTextColor);

					if(txtbox == numtxtOccupancyRate || txtbox == numtxtKnittingEfficiency)
                    {
						// 入力値修正イベント設定
						txtbox.LostFocus += new EventHandler(FixValue);
                    }
				}
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + ", " + ex.StackTrace);
			}
		}
		#endregion Private Methods
	}
}
