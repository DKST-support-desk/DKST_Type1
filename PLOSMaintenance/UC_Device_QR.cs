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
	/// QRリーダ登録画面
	/// </summary>
	public partial class UC_Device_QR : UserControl
	{
		//********************************************
		//* メンバー変数
		//********************************************
		#region "メンバー変数"

		/// <summary>
		/// DataGridView列名 削除ボタン
		/// </summary>
		private const string CLM_DELETE = "DeleteActionImage";

		/// <summary>
		/// QRリーダ機器マスタのデータ操作オブジェクト
		/// </summary>
		private SqlReaderDeviceMst mSqlReaderDeviceMst;

		/// <summary>
		/// データテーブル
		/// </summary>
		private DataTable mDtReaderDeviceMst;

		/// <summary>
		/// 削除リスト
		/// </summary>
		private List<Guid> mRemoveList = new List<Guid>();

		/// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion "メンバー変数"

		//********************************************
		//* コンストラクタ
		//********************************************
		#region "コンストラクタ"
		/// <summary>
		/// QRリーダ登録画面
		/// </summary>
		public UC_Device_QR()
		{
			try
			{
				InitializeComponent();

				// QRリーダ機器マスタのデータ操作オブジェクトを実装
				mSqlReaderDeviceMst = new SqlReaderDeviceMst(Properties.Settings.Default.ConnectionString_New);

				// データグリッド初期化処理
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
		/// 登録ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRegist_Click(object sender, EventArgs e)
		{
			try
			{
				logger.Info("登録ボタン押下");

				dataGridView.EndEdit();

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
				if(UpsertReaderDeviceMst(mDtReaderDeviceMst, mRemoveList))
                {
					mDtReaderDeviceMst.AcceptChanges();

					//データグリッド初期化処理
					InitializeDataGridView();

					logger.Info("登録しました。");
					MessageBox.Show("登録しました。", "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

					// 編集フラグを消す
					FrmMain.gIsDataChange = false;
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
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 行追加ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddButtonClicked(object sender, EventArgs e)
		{
			try
			{
				logger.Info("行追加ボタン押下");

				DataRow dr = mDtReaderDeviceMst.NewRow();

				// リーダID
				dr[ColumnReaderDeviceMst.READER_ID] = Guid.NewGuid();

				// リーダ名
				dr[ColumnReaderDeviceMst.READER_NAME] = "";

				// ポート番号
				dr[ColumnReaderDeviceMst.PORT_NO] = 0;

				// 機器説明
				dr[ColumnReaderDeviceMst.DEVICE_REMARK] = "";

				// 作成日時 ※作成日時は行追加日時にしないと作成日時が同一のレコードが複数できてしまう
				dr[ColumnReaderDeviceMst.CREATE_DATETIME] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

				mDtReaderDeviceMst.Rows.Add(dr);

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// データグリッドクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGridCellContentClicked(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				DataGridView dgv = (DataGridView)sender;

				// "Button"列ならば、ボタンがクリックされた
				if (dgv.Columns[e.ColumnIndex].Name == CLM_DELETE)
				{
					logger.Info("行削除ボタン押下");

					// 確認メッセージ表示
					logger.Info("選択中の行を削除してよろしいですか？");
					if (System.Windows.Forms.DialogResult.Yes != MessageBox.Show(this, "選択中の行を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						logger.Info("No押下");
						return;
					}

					logger.Info("Yes押下");

					// 削除リストに追加
					mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnReaderDeviceMst.READER_ID].Value);

					// 行の削除
					mDtReaderDeviceMst.Rows.RemoveAt(e.RowIndex);

					// 編集フラグを立てる
					FrmMain.gIsDataChange = true;
				}
			}
            catch(Exception ex)
            {
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// グリッド編集時イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				DataGridViewCellStyle style = ((DataGridView)sender).CurrentCell.Style;
				style.ForeColor = Color.Blue;
				((DataGridView)sender).CurrentCell.Style = style;

				// 編集フラグを立てる
				FrmMain.gIsDataChange = true;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message + "," + ex.StackTrace);
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion "イベント"

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
			mDtReaderDeviceMst = mSqlReaderDeviceMst.Select();

			if (mDtReaderDeviceMst != null)
			{
				// データグリッド行クリア
				dataGridView.Columns.Clear();

				dataGridView.AutoGenerateColumns = false;
				var bindingSource = new BindingSource(mDtReaderDeviceMst, string.Empty);
				dataGridView.DataSource = bindingSource;

				// フォント指定
				DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
				textEditCellStyle.Font = new Font("MS UI Gothic", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));

				// 削除アイコン
				DataGridViewImageColumn imgDelete = new DataGridViewImageColumn();
				imgDelete.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
				imgDelete.ImageLayout = DataGridViewImageCellLayout.Zoom;
				imgDelete.Name = CLM_DELETE;
				imgDelete.HeaderText = "";
				imgDelete.Width = 50;
				imgDelete.FillWeight = 50;
				imgDelete.SortMode = DataGridViewColumnSortMode.NotSortable;
				imgDelete.Resizable = DataGridViewTriState.False;
				dataGridView.Columns.Add(imgDelete);

				// リーダID
				DataGridViewTextBoxColumn txtDeviceId = new DataGridViewTextBoxColumn();
				txtDeviceId.DataPropertyName = ColumnReaderDeviceMst.READER_ID;
				txtDeviceId.Name = ColumnReaderDeviceMst.READER_ID;
				txtDeviceId.Width = 20;
				txtDeviceId.SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridView.Columns.Add(txtDeviceId);
				dataGridView.Columns[ColumnReaderDeviceMst.READER_ID].Visible = false;

				// リーダ名
				DataGridViewTextBoxColumn txtDeviceReaderName = new DataGridViewTextBoxColumn();
				txtDeviceReaderName.DataPropertyName = ColumnReaderDeviceMst.READER_NAME;
				txtDeviceReaderName.Name = ColumnReaderDeviceMst.READER_NAME;
				txtDeviceReaderName.HeaderText = "リーダ名";
				txtDeviceReaderName.MaxInputLength = 512;
				txtDeviceReaderName.Width = 350;
				txtDeviceReaderName.FillWeight = 350;
				txtDeviceReaderName.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
				txtDeviceReaderName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				txtDeviceReaderName.DefaultCellStyle = textEditCellStyle;
				txtDeviceReaderName.SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridView.Columns.Add(txtDeviceReaderName);

				// ポート番号
				DataGridViewTextBoxColumn txtDevicePortNo = new DataGridViewTextBoxColumn();
				txtDevicePortNo.DataPropertyName = ColumnReaderDeviceMst.PORT_NO;
				txtDevicePortNo.Name = ColumnReaderDeviceMst.PORT_NO;
				txtDevicePortNo.HeaderText = "COMポート番号";
				txtDevicePortNo.Width = 100;
				txtDevicePortNo.FillWeight = 100;
				txtDevicePortNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				txtDevicePortNo.MaxInputLength = 256;
				txtDevicePortNo.DefaultCellStyle = textEditCellStyle;
				txtDevicePortNo.SortMode = DataGridViewColumnSortMode.NotSortable;
				txtDevicePortNo.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
				txtDevicePortNo.MaxInputLength = 2;
				dataGridView.Columns.Add(txtDevicePortNo);

				// 機器説明
				DataGridViewTextBoxColumn txtDeviceRemark = new DataGridViewTextBoxColumn();
				txtDeviceRemark.DataPropertyName = ColumnReaderDeviceMst.DEVICE_REMARK;
				txtDeviceRemark.Name = ColumnReaderDeviceMst.DEVICE_REMARK;
				txtDeviceRemark.HeaderText = "機器説明";
				txtDeviceRemark.Width = 350;
				txtDeviceRemark.FillWeight = 350;
				txtDeviceRemark.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				txtDeviceRemark.MaxInputLength = 256;
				txtDeviceRemark.DefaultCellStyle = textEditCellStyle;
				txtDeviceRemark.SortMode = DataGridViewColumnSortMode.NotSortable;
				txtDeviceRemark.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
				dataGridView.Columns.Add(txtDeviceRemark);

				// 作成日時
				DataGridViewTextBoxColumn txtCreateDateTime = new DataGridViewTextBoxColumn();
				txtCreateDateTime.DataPropertyName = ColumnReaderDeviceMst.CREATE_DATETIME;
				txtCreateDateTime.Name = ColumnReaderDeviceMst.CREATE_DATETIME;
				txtCreateDateTime.SortMode = DataGridViewColumnSortMode.NotSortable;
				dataGridView.Columns.Add(txtCreateDateTime);
				dataGridView.Columns[ColumnDoDeviceMst.CREATE_DATETIME].Visible = false;	// 作成日時は非表示
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
			// 重複確認用リスト
			List<string> doubleNameList = new List<string>();
			List<int> doubleCOMPortList = new List<int>();

			foreach (DataRow rowItem in mDtReaderDeviceMst.Rows)
			{
				// リーダ名の確認
				string readerName = rowItem[ColumnReaderDeviceMst.READER_NAME] as string;
				if (String.IsNullOrWhiteSpace(readerName))
				{
					logger.Warn("リーダ名を入力してください。");
					MessageBox.Show(this, "リーダ名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
				{
					readerName = readerName.Trim();

					// 重複確認
					if (doubleNameList.Contains(readerName))
					{
						logger.Warn("リーダ名に重複があります。");
						MessageBox.Show(this, "リーダ名に重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					else
					{
						doubleNameList.Add(readerName);
					}
				}

				// COMポート番号の確認
				int? COMPortNum = rowItem[ColumnReaderDeviceMst.PORT_NO] as int?;

				if (COMPortNum == null)
				{
					logger.Warn("COMポート番号を入力してください。");
					MessageBox.Show(this, "COMポート番号を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
                else
                {
					int portNum = (int)COMPortNum;

					// 範囲確認
					if (portNum < 0 || 99 < portNum)
					{
						logger.Warn("COMポート番号が有効な値ではありません。");
						MessageBox.Show("COMポート番号が有効な値ではありません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
					// 重複確認
					else if (doubleCOMPortList.Contains(portNum))
                    {
						logger.Warn("COMポート番号に重複があります。");
						MessageBox.Show(this, "COMポート番号に重複があります。別番号を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
                    else
                    {
						doubleCOMPortList.Add(portNum);
                    }
				}
			}
			return true;
		}

		/// <summary>
		/// QRリーダ機器マスタ更新処理
		/// </summary>
		/// <param name="dt">データテーブル</param>
		/// <param name="removeList">削除リスト</param>
		//private void UpsertCameraDeviceMst(DataTable dt, DataTable removeList)
		private bool UpsertReaderDeviceMst(DataTable dt, List<Guid> removeList)
		{
			bool result = false;

			// 追加/更新処理
			if (mSqlReaderDeviceMst.Upsert(dt))
			{
				logger.Debug("Upsert処理を実行しました。");

				// 削除処理
				if (mSqlReaderDeviceMst.Delete(removeList))
				{
					logger.Debug("Delete処理を実行しました。");
					result = true;
				}
				else
				{
					logger.Info("Delete処理に失敗しました。");
				}
			}
			else
			{
				logger.Info("Upsert処理に失敗しました。");
			}

			return result;
		}

		/// <summary>
		/// COMポート番号の編集時にイベントを追加
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
			if(e.Control is DataGridViewTextBoxEditingControl)
            {
				DataGridView dgv = (DataGridView)sender;

				// 編集のために選択したセルを取得
				DataGridViewTextBoxEditingControl portCell = (DataGridViewTextBoxEditingControl)e.Control;

				// イベントハンドラを削除 ※同じセルに同じイベントを追加するのを防ぐため
				portCell.KeyPress -= new KeyPressEventHandler(CheckValue);

				// COMポート番号列のセルにイベントを追加
				if (dgv.CurrentCell.OwningColumn.Name.Equals(ColumnReaderDeviceMst.PORT_NO))
                {
					portCell.KeyPress += new KeyPressEventHandler(CheckValue);
                }
			}
        }

		/// <summary>
		/// 値チェック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckValue(object sender, KeyPressEventArgs e)
        {
			//バックスペースが押された時は有効（Deleteキーも有効）
			if (e.KeyChar == '\b')
			{
				return;
			}

			// 数字のみ許可
			if (e.KeyChar < '0' || '9' < e.KeyChar)
			{
				e.Handled = true;
			}
		}
		#endregion "プライベートメソッド"
	}
}
