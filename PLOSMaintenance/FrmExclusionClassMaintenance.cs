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
	public partial class FrmExclusionClassMaintenance : Form
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバ変数"
		/// <summary>
		/// データテーブル
		/// </summary>
		private DataTable mDtExclusionMst;

		/// <summary>
		/// QRリーダ機器マスタのデータ操作オブジェクト
		/// </summary>
		private SqlExclusionMst mSqlExclusionMst;

		/// <summary>
		/// DataGridView列名 削除ボタン
		/// </summary>
		private const string CLM_DELETE = "DeleteActionImage";

		/// <summary>
		/// 削除リスト
		/// </summary>
		private List<Guid> mRemoveList = new List<Guid>();

		private List<String> exClassNameList = new List<string>();

		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// アクセスインスタンス
		/// </summary>
		private UCOperating_Shift_Item mUCOperating_Shift_Item;
		#endregion "メンバ変数"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region "コンストラクタ"
		public FrmExclusionClassMaintenance()
		{
            try
            {
				InitializeComponent();

				mSqlExclusionMst = new SqlExclusionMst(Properties.Settings.Default.ConnectionString_New);
				//ucExclusionRow = new UCOperating_Shift_Exclusion_Row();

				InitializeDataGridView();
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public FrmExclusionClassMaintenance(UCOperating_Shift_Item ucOperating_Shift_Item)
		{
			try
			{
				InitializeComponent();

				mSqlExclusionMst = new SqlExclusionMst(Properties.Settings.Default.ConnectionString_New);

				mUCOperating_Shift_Item = ucOperating_Shift_Item;

				InitializeDataGridView();
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion "コンストラクタ"

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// 登録ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegistButtonClicked(object sender, EventArgs e)
		{
			try
			{
				logger.Info("除外区分登録　登録ボタン押下");

				dgvEditTarget.EndEdit();

				// 入力チェック
				if (CheckInput() == false)
				{
					return;
				}

				// 確認メッセージ表示
				logger.Info("現在の情報で登録してよろしいですか？");
				if (System.Windows.Forms.DialogResult.Yes != MessageBox.Show(this, "現在の情報で登録してよろしいですか？", "登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					logger.Info("No押下");
					return;
				}

				logger.Info("Yes押下");

				// QRリーダ機器マスタ更新処理
				if (UpsertExclusionMst(mDtExclusionMst, mRemoveList))
				{
					mDtExclusionMst.AcceptChanges();

					//データグリッド初期化処理
					InitializeDataGridView();

					logger.Info("登録しました。");
					MessageBox.Show("登録しました。", "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

					// 編集フラグを消す
					FrmMain.gIsDataChange = false;

					DialogResult = DialogResult.OK;

                    Close();
				}
				else
				{
					logger.Info("登録に失敗しました。");
					MessageBox.Show("登録に失敗しました。", "登録失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(ex.Message + "," + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 追加ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddButtonClicked(object sender, EventArgs e)
		{
			try
			{
				logger.Info("除外区分登録　行追加ボタン押下");

				DataRow dr = mDtExclusionMst.NewRow();

				// 除外区分ID
				dr[ColumnExclusionMst.EXCLUSION_ID] = Guid.NewGuid();

				// 除外区分名
				dr[ColumnExclusionMst.EXCLUSION_NAME] = "";

				// 作成日時 ※作成日時は行追加日時にしないと作成日時が同一のレコードが複数できてしまう
				dr[ColumnExclusionMst.CREATE_DATETIME] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

				dr[SqlExclusionMst.EXIST_IN_OPERATING_SHIFT_EXCLUSION] = 0;

				mDtExclusionMst.Rows.Add(dr);

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(ex.Message + "," + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 削除ボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridCellContentClicked(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				DataGridView dgv = (DataGridView)sender;

				var searchId = mDtExclusionMst.Rows[e.RowIndex].Field<Guid>(ColumnExclusionMst.EXCLUSION_ID);
				int targetCnt = mUCOperating_Shift_Item.DtOperatingShiftExclusionTbl.AsEnumerable()
								.Where(x => x.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS) == searchId)
								.Count();

				// "Button"列ならば、ボタンがクリックされた
				if (dgv.Columns[e.ColumnIndex].Name == CLM_DELETE
					&& mDtExclusionMst.Rows[e.RowIndex].Field<int>(SqlExclusionMst.EXIST_IN_OPERATING_SHIFT_EXCLUSION) < 1
					&& targetCnt <= 0
					&& !CheckShiftExclusion(searchId))
				{
					logger.Info("除外区分登録　削除ボタン押下");

					// 確認メッセージ表示
					logger.Info("選択中の行を削除してよろしいですか？");
					if (System.Windows.Forms.DialogResult.Yes != MessageBox.Show(this, "選択中の行を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						logger.Info("No押下");
						return;
					}

					logger.Info("Yes押下");

					// 削除リストに追加
					mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnExclusionMst.EXCLUSION_ID].Value);

					// 行の削除
					// CellFormattingイベントで削除した行へアクセスしないように削除中はイベントのバインドを外す
					dgv.CellFormatting -= new DataGridViewCellFormattingEventHandler(dgvEditTarget_CellFormatting);
					mDtExclusionMst.Rows.RemoveAt(e.RowIndex);
					dgv.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvEditTarget_CellFormatting);

					// 編集フラグを立てる
					FrmMain.gIsDataChange = true;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(ex.Message + "," + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// キャンセルボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelButtonClicked(object sender, EventArgs e)
		{
			logger.Info("除外区分登録　キャンセルボタン押下");

			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		/// グリッド編集時イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvEditTarget_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				// 変更セルを青文字に変更
				DataGridViewCellStyle style = ((DataGridView)sender).CurrentCell.Style;
				style.ForeColor = Color.Blue;
				((DataGridView)sender).CurrentCell.Style = style;

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(ex.Message + "," + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 削除アイコンの表示切替
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvEditTarget_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				DataGridView dgv = (DataGridView)sender;

				if (dgv == null)
				{
					return;
				}
				//"Image"列のセルか確認する
				else if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE) && 0 <= e.RowIndex)
				{
					if (dgv.Rows.Count <= e.RowIndex)
					{
						return;
					}

					var searchId = mDtExclusionMst.Rows[e.RowIndex].Field<Guid>(ColumnExclusionMst.EXCLUSION_ID);
					int targetCnt = mUCOperating_Shift_Item.DtOperatingShiftExclusionTbl.AsEnumerable()
									.Where(x => x.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS) == searchId)
									.Count();

					if (0 < mDtExclusionMst.Rows[e.RowIndex].Field<int>(SqlExclusionMst.EXIST_IN_OPERATING_SHIFT_EXCLUSION)
						|| 0 < targetCnt
						|| CheckShiftExclusion(searchId))
					{
						e.Value = global::PLOSMaintenance.Properties.Resources.利用中アイコン;
                        e.FormattingApplied = true;
                    }
                    else
                    {
                        e.Value = global::PLOSMaintenance.Properties.Resources.削除アイコン;
                        e.FormattingApplied = true;
                    }
                }
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(ex.Message + "," + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		//********************************************
		//* パブリックメソッド
		//********************************************
		#region "パブリックメソッド"
		/// <summary>
		/// DataGridView初期化処理
		/// </summary>
		public void InitializeDataGridView()
		{
			// 削除リストクリア
			mRemoveList.Clear();

			// SQL実行
			mDtExclusionMst = mSqlExclusionMst.Select();

			if (mDtExclusionMst != null)
			{
				// データグリッド行クリア
				dgvEditTarget.Columns.Clear();

				dgvEditTarget.AutoGenerateColumns = false;
				var bindingSource = new BindingSource(mDtExclusionMst, string.Empty);
				dgvEditTarget.DataSource = bindingSource;

				// フォント指定
				DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
				textEditCellStyle.Font = new Font("MS UI Gothic", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));

				// 削除アイコン
				DataGridViewImageColumn imgDelete = new DataGridViewImageColumn();
				imgDelete.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
				imgDelete.ImageLayout = DataGridViewImageCellLayout.Zoom;
				imgDelete.Name = CLM_DELETE;
				imgDelete.HeaderText = "";
				imgDelete.Width = 80;
				imgDelete.FillWeight = 50;
				imgDelete.SortMode = DataGridViewColumnSortMode.NotSortable;
				imgDelete.Resizable = DataGridViewTriState.False;
				dgvEditTarget.Columns.Add(imgDelete);

				// 除外区分ID
				DataGridViewTextBoxColumn txtExclusionID = new DataGridViewTextBoxColumn();
				txtExclusionID.DataPropertyName = ColumnExclusionMst.EXCLUSION_ID;
				txtExclusionID.Name = ColumnExclusionMst.EXCLUSION_ID;
				txtExclusionID.Width = 20;
				txtExclusionID.SortMode = DataGridViewColumnSortMode.NotSortable;
				dgvEditTarget.Columns.Add(txtExclusionID);
				dgvEditTarget.Columns[ColumnExclusionMst.EXCLUSION_ID].Visible = false;

				// 除外区分名
				DataGridViewTextBoxColumn txtExclusionName = new DataGridViewTextBoxColumn();
				txtExclusionName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				txtExclusionName.DataPropertyName = ColumnExclusionMst.EXCLUSION_NAME;
				txtExclusionName.Name = ColumnExclusionMst.EXCLUSION_NAME;
				txtExclusionName.HeaderText = "除外区分";
				txtExclusionName.SortMode = DataGridViewColumnSortMode.NotSortable;
				txtExclusionName.FillWeight = 150;
				txtExclusionName.MaxInputLength = 20;
				txtExclusionName.DefaultCellStyle = textEditCellStyle;
				dgvEditTarget.Columns.Add(txtExclusionName);

				// 作成日時
				DataGridViewTextBoxColumn txtCreateDateTime = new DataGridViewTextBoxColumn();
				txtCreateDateTime.DataPropertyName = ColumnExclusionMst.CREATE_DATETIME;
				txtCreateDateTime.DataPropertyName = ColumnReaderDeviceMst.CREATE_DATETIME;
				txtCreateDateTime.Name = ColumnReaderDeviceMst.CREATE_DATETIME;
				txtCreateDateTime.SortMode = DataGridViewColumnSortMode.NotSortable;
				dgvEditTarget.Columns.Add(txtCreateDateTime);
				dgvEditTarget.Columns[ColumnDoDeviceMst.CREATE_DATETIME].Visible = false;
			}
		}
		#endregion "パブリックメソッド"

		//********************************************
		//* プライベートメソッド
		//********************************************
		#region "プライベートメソッド"
		/// <summary>
		/// 入力チェック
		/// </summary>
		/// <returns></returns>
		private Boolean CheckInput()
		{
			List<String> listReaderName = mDtExclusionMst.AsEnumerable().Select(row => row[ColumnExclusionMst.EXCLUSION_NAME].ToString()).ToList<String>();
			int index = 0;
			foreach (DataRow rowItem in mDtExclusionMst.Rows)
			{
				if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
				{
					if (String.IsNullOrWhiteSpace(rowItem[ColumnExclusionMst.EXCLUSION_NAME] as string))
					{
						logger.Warn("除外区分名を入力してください。");
						MessageBox.Show(this, "除外区分名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}

				// 名称重複チェック
				string[] arr = new string[mDtExclusionMst.Rows.Count];
				listReaderName.CopyTo(arr);
				List<String> tempList = arr.ToList();
				tempList.RemoveAt(index);
				if (tempList.Contains(rowItem.Field<string>(ColumnExclusionMst.EXCLUSION_NAME)))
				{
					logger.Warn("除外区分名重複があります。別名を登録してください。");
					MessageBox.Show(this, "除外区分名重複があります。別名を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				index++;
			}
			return true;
		}

		/// <summary>
		/// QRリーダ機器マスタ更新処理
		/// </summary>
		/// <param name="dt">データテーブル</param>
		/// <param name="removeList">削除リスト</param>
		//private void UpsertCameraDeviceMst(DataTable dt, DataTable removeList)
		private bool UpsertExclusionMst(DataTable dt, List<Guid> removeList)
		{
			bool result = false;

			// 追加/更新処理
			if (mSqlExclusionMst.Upsert(dt))
			{
				logger.Debug("Upsert処理を実行しました。");
				result = true;
			}
            else
            {
				logger.Debug("Upsert処理に失敗しました。");
			}

			// 削除処理
			if (mSqlExclusionMst.Delete(removeList))
			{
				logger.Debug("Delete処理を実行しました。");
				result = true;
			}
            else
            {
				logger.Debug("Delete処理に失敗しました。");
			}

			return result;
		}

		/// <summary>
		/// 除外登録の現在の設定状況を確認
		/// </summary>
		/// <param name="searchId"></param>
		/// <returns></returns>
		private bool CheckShiftExclusion(Guid searchId)
        {
			var target1 = mUCOperating_Shift_Item.Owner.ucOperating_Shift_Item1.ExclusionRowList.Find
							(x => x.ExclusionClass == searchId);
			var target2 = mUCOperating_Shift_Item.Owner.ucOperating_Shift_Item2.ExclusionRowList.Find
							(x => x.ExclusionClass == searchId);
			var target3 = mUCOperating_Shift_Item.Owner.ucOperating_Shift_Item3.ExclusionRowList.Find
							(x => x.ExclusionClass == searchId);

			if(target1 != null || target2 != null || target3 != null)
            {
				return true;
            }

			return false;
		}
		#endregion "プライベートメソッド
	}
}
