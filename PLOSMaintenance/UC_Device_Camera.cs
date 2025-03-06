using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
    /// <summary>
    /// カメラ登録画面
    /// </summary>
	public partial class UC_Device_Camera : UserControl
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
        /// カメラ機器マスタのデータ操作オブジェクト
        /// </summary>
        private SqlCameraDeviceMst mSqlCameraDeviceMst;

        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable mDtCameraDeviceMst;

        /// <summary>
        /// 削除リスト
        /// </summary>
        private List<Guid> mRemoveList = new List<Guid>();

        /// <summary>
        /// FPSコンボボックスリスト
        /// </summary>
        private List<object> mDeviceNumberList = new List<object>
        {
            new { Value = 30, Name = "30" },
            new { Value = 60, Name = "60" },
        };

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
        /// カメラ登録画面
        /// </summary>
        public UC_Device_Camera()
        {
            try
            {
                InitializeComponent();

                // カメラ機器マスタのデータ操作オブジェクトを実装
                mSqlCameraDeviceMst = new SqlCameraDeviceMst(Properties.Settings.Default.ConnectionString_New);

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
        /// カメラ情報登録ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCameraRegist_Click(object sender, EventArgs e)
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

                // カメラマスタ更新処理
                if(UpsertCameraDeviceMst(mDtCameraDeviceMst, mRemoveList))
                {
                    mDtCameraDeviceMst.AcceptChanges();

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
                    return;
                }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// パスワード変更ボタンクリックイベント　※現在無効化しています。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPassword_Click(object sender, EventArgs e)
        {
            try
            {
                //Common.Logger.Instance.WriteTraceLog("パスワード変更ボタン押下");

                // パスワード変更画面表示
                FrmPasswordChange frm = new FrmPasswordChange();

                frm.ShowDialog(this.ParentForm);
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

                DataRow dr = mDtCameraDeviceMst.NewRow();

                // カメラID
                dr[ColumnCameraDeviceMst.CAMERA_ID] = Guid.NewGuid();

                // カメラ名
                dr[ColumnCameraDeviceMst.CAMERA_NAME] = "";

                // カメラデバイスコード
                dr[ColumnCameraDeviceMst.CAMERA_DEVICE_CODE] = "";

                // FPS設定
                dr[ColumnCameraDeviceMst.DEVICE_NUMNBER] = 30;

                // マイクデバイスコード
                dr[ColumnCameraDeviceMst.MIC_DEVICE_CODE] = "";

                // 機器説明
                dr[ColumnCameraDeviceMst.DEVICE_REMARK] = "";

                // 作成日時 ※作成日時は行追加日時にしないと作成日時が同一のレコードが複数できてしまう
                dr[ColumnCameraDeviceMst.CREATE_DATETIME] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                // データ収集カメラマスタに同じ品番タイプIDが存在するか
                dr[SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA] = 0;

                mDtCameraDeviceMst.Rows.Add(dr);

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
                if (dgv.Columns[e.ColumnIndex].Name == CLM_DELETE && mDtCameraDeviceMst.Rows[e.RowIndex].Field<int>(SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA) < 1)
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
                    mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnCameraDeviceMst.CAMERA_ID].Value);

                    // 行の削除
                    dataGridView.CellFormatting -= new DataGridViewCellFormattingEventHandler(dataGridView_CellFormatting);
                    mDtCameraDeviceMst.Rows.RemoveAt(e.RowIndex);
                    dataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView_CellFormatting);

                    // 編集フラグを立てる
                    FrmMain.gIsDataChange = true;
                }
            }
            catch (Exception ex)
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

        /// <summary>
        /// 削除アイコンの表示切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                    // 選択行とDataTableとのずれを確認
                    if (mDtCameraDeviceMst.Rows.Count <= e.RowIndex)
                    {
                        return;
                    }

                    if (0 < mDtCameraDeviceMst.Rows[e.RowIndex].Field<int>(SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA))
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
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// データグリッド初期化
        /// </summary>
        public void InitializeDataGridView()
        {
            // 削除リストクリア
            mRemoveList.Clear();

            // SQL実行
            mDtCameraDeviceMst = mSqlCameraDeviceMst.Select();

            if (mDtCameraDeviceMst != null)
            {
                // データグリッド行クリア
                dataGridView.Columns.Clear();

                dataGridView.AutoGenerateColumns = false;
                var bindingSource = new BindingSource(mDtCameraDeviceMst, string.Empty);
                dataGridView.DataSource = bindingSource;

                // フォント指定
                DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
                textEditCellStyle.Font = new Font("MS UI Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                // 列幅変更を有効化
                dataGridView.AllowUserToResizeColumns = true;

                // 削除アイコン
                DataGridViewTextBoxColumn iconDeleteColumn = new DataGridViewTextBoxColumn();
                DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
                imgColumn.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
                imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgColumn.Name = CLM_DELETE;
                imgColumn.HeaderText = "";
                imgColumn.Width = 50;
                imgColumn.FillWeight = 50;
                imgColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(imgColumn);

                // カメラID(内部保持)
                DataGridViewTextBoxColumn txtCameraIdColumn = new DataGridViewTextBoxColumn();
                txtCameraIdColumn.DataPropertyName = ColumnCameraDeviceMst.CAMERA_ID;
                txtCameraIdColumn.Name = ColumnCameraDeviceMst.CAMERA_ID;
                txtCameraIdColumn.Width = 20;
                txtCameraIdColumn.Visible = false;
                txtCameraIdColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtCameraIdColumn);
                dataGridView.Columns[ColumnCameraDeviceMst.CAMERA_ID].Visible = false; // カメラIDは非表示

                // カメラ認識コード
                DataGridViewTextBoxColumn txtCameraDeviceCodeColumn = new DataGridViewTextBoxColumn();
                txtCameraDeviceCodeColumn.DataPropertyName = ColumnCameraDeviceMst.CAMERA_DEVICE_CODE;
                txtCameraDeviceCodeColumn.Name = ColumnCameraDeviceMst.CAMERA_DEVICE_CODE;
                txtCameraDeviceCodeColumn.HeaderText = "カメラ認識コード";
                txtCameraDeviceCodeColumn.MaxInputLength = 128;
                txtCameraDeviceCodeColumn.Width = 350;
                txtCameraDeviceCodeColumn.FillWeight = 350;
                txtCameraDeviceCodeColumn.DefaultCellStyle = textEditCellStyle;
                txtCameraDeviceCodeColumn.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                txtCameraDeviceCodeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtCameraDeviceCodeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtCameraDeviceCodeColumn);

                // カメラ名称
                DataGridViewTextBoxColumn txtCameraNameColumn = new DataGridViewTextBoxColumn();
                txtCameraNameColumn.DataPropertyName = ColumnCameraDeviceMst.CAMERA_NAME;
                txtCameraNameColumn.Name = ColumnCameraDeviceMst.CAMERA_NAME;
                txtCameraNameColumn.HeaderText = "カメラ名称";
                txtCameraNameColumn.MaxInputLength = 128;
                txtCameraNameColumn.Width = 250;
                txtCameraNameColumn.FillWeight = 250;
                txtCameraNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtCameraNameColumn.DefaultCellStyle = textEditCellStyle;
                txtCameraNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtCameraNameColumn);

                // FPS
                DataGridViewComboBoxColumn cmbDeviceNumberColumn = new DataGridViewComboBoxColumn();
                cmbDeviceNumberColumn.DataPropertyName = ColumnCameraDeviceMst.DEVICE_NUMNBER;
                cmbDeviceNumberColumn.Name = ColumnCameraDeviceMst.DEVICE_NUMNBER;
                cmbDeviceNumberColumn.HeaderText = "FPS";
                cmbDeviceNumberColumn.Width = 50;
                cmbDeviceNumberColumn.FillWeight = 50;
                cmbDeviceNumberColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                cmbDeviceNumberColumn.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                cmbDeviceNumberColumn.FlatStyle = FlatStyle.Flat;
                cmbDeviceNumberColumn.DataSource = mDeviceNumberList;
                cmbDeviceNumberColumn.ValueMember = "Value";
                cmbDeviceNumberColumn.DisplayMember = "Name";
                cmbDeviceNumberColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(cmbDeviceNumberColumn);

                // マイク認識コード
                DataGridViewTextBoxColumn txtMicDeviceCodeColumn = new DataGridViewTextBoxColumn();
                txtMicDeviceCodeColumn.DataPropertyName = ColumnCameraDeviceMst.MIC_DEVICE_CODE;
                txtMicDeviceCodeColumn.Name = ColumnCameraDeviceMst.MIC_DEVICE_CODE;
                txtMicDeviceCodeColumn.HeaderText = "マイク認識コード";
                txtMicDeviceCodeColumn.MaxInputLength = 128;
                txtMicDeviceCodeColumn.Width = 350;
                txtMicDeviceCodeColumn.FillWeight = 350;
                txtMicDeviceCodeColumn.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                txtMicDeviceCodeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtMicDeviceCodeColumn.DefaultCellStyle = textEditCellStyle;
                txtMicDeviceCodeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtMicDeviceCodeColumn);

                // 機器説明
                DataGridViewTextBoxColumn txtDeviceRemarkColumn = new DataGridViewTextBoxColumn();
                txtDeviceRemarkColumn.DataPropertyName = ColumnCameraDeviceMst.DEVICE_REMARK;
                txtDeviceRemarkColumn.Name = ColumnCameraDeviceMst.DEVICE_REMARK;
                txtDeviceRemarkColumn.HeaderText = "機器説明";
                txtDeviceRemarkColumn.Width = 350;
                txtDeviceRemarkColumn.FillWeight = 350;
                txtDeviceRemarkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtDeviceRemarkColumn.MaxInputLength = 256;
                txtDeviceRemarkColumn.DefaultCellStyle = textEditCellStyle;
                txtDeviceRemarkColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtDeviceRemarkColumn);

                // 作成日時(内部保持)
                DataGridViewTextBoxColumn txtCreateDateTimeColumn = new DataGridViewTextBoxColumn();
                txtCreateDateTimeColumn.DataPropertyName = ColumnCameraDeviceMst.CREATE_DATETIME;
                txtCreateDateTimeColumn.Name = ColumnCameraDeviceMst.CREATE_DATETIME;
                txtCreateDateTimeColumn.Width = 20;
                txtCreateDateTimeColumn.Visible = false;
                txtCreateDateTimeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtCreateDateTimeColumn);
                dataGridView.Columns[ColumnCameraDeviceMst.CREATE_DATETIME].Visible = false; // 作成日時は非表示

                // データ収集カメラマスタに同じカメラIDが存在するか
                DataGridViewTextBoxColumn txtExistInCamera = new DataGridViewTextBoxColumn();
                txtExistInCamera.DataPropertyName = SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA;
                txtExistInCamera.Name = SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA;
                txtExistInCamera.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtExistInCamera);
                dataGridView.Columns[SqlCameraDeviceMst.EXIST_IN_DATA_COLLECTION_CAMERA].Visible = false;
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
        private bool CheckInput()
        {
            List<String> listCameraName = mDtCameraDeviceMst.AsEnumerable().Select(row => row[ColumnCameraDeviceMst.CAMERA_NAME].ToString()).ToList<String>();
            int index = 0;
            foreach (DataRow rowItem in mDtCameraDeviceMst.Rows)
            {
                // 更新か追加された行
                if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
                {
                    // カメラ認識コード
                    if (string.IsNullOrWhiteSpace(rowItem[ColumnCameraDeviceMst.CAMERA_DEVICE_CODE] as string))
                    {
                        logger.Warn("カメラ認識コードを入力してください。");
                        MessageBox.Show(this, "カメラ認識コードを入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // カメラ名称
                    if (string.IsNullOrWhiteSpace(rowItem[ColumnCameraDeviceMst.CAMERA_NAME] as string))
                    {
                        logger.Warn("カメラ名称を入力してください。");
                        MessageBox.Show(this, "カメラ名称を入力してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // FPS
                    int? devNumber = rowItem[ColumnCameraDeviceMst.DEVICE_NUMNBER] as int?;
                    if ( devNumber != null)
                    {
                        if (devNumber != 30 && devNumber != 60)
                        {
                            logger.Warn("FPSが有効な値ではありません。");
                            MessageBox.Show("FPSが有効な値ではありません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        logger.Warn("FPSの入力値が異常です。");
                        MessageBox.Show("FPSの入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // 名称重複チェック
                string[] arr = new string[mDtCameraDeviceMst.Rows.Count];
                listCameraName.CopyTo(arr);
                List<String> tempList = arr.ToList();
                tempList.RemoveAt(index);
                if(tempList.Contains(rowItem.Field<string>(ColumnCameraDeviceMst.CAMERA_NAME)))
                {
                    logger.Warn("カメラ名称に重複があります。");
                    MessageBox.Show(this, "カメラ名称に重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                index++;
            }

            return true;
        }

        /// <summary>
        /// カメラ機器マスタ更新処理
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="removeList">削除リスト</param>
        private bool UpsertCameraDeviceMst(DataTable dt, List<Guid> removeList)
        {
            bool result = false;

            // 追加/更新処理
            if (mSqlCameraDeviceMst.Upsert(dt))
            {
                logger.Debug("Upsert処理を実行しました。");

                // 削除処理
                if (mSqlCameraDeviceMst.Delete(removeList))
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
        #endregion "プライベートメソッド"
    }
}
