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
    /// DI機器登録画面
    /// </summary>
    public partial class UC_Device_DI : UserControl
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// DataGridView列名 「削除」ボタン
        /// </summary>
        private const string CLM_DELETE_NAME = "dgvClmDeleteImage";

        /// <summary>
        /// DataGridView ComboBoxパラメータ
        /// </summary>
        private readonly List<int> mInputList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        /// <summary>
        /// DBアクセスクラスインスタンス
        /// </summary>
        private SqlSensorDeviceMst mSqlSensorDeviceMst;

        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable mDtSensorDeviceMst;

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
        /// DI機器登録画面
        /// </summary>
        public UC_Device_DI()
        {
            try
            {
                InitializeComponent();

                mSqlSensorDeviceMst = new SqlSensorDeviceMst(Properties.Settings.Default.ConnectionString_New);

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
                logger.Info("登録ボタン押下");

                dgvSensorDeviceMst.EndEdit();

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

                // センサ機器マスタ更新処理
                if(UpsertSensorDeviceMst(mDtSensorDeviceMst, mRemoveList))
                {
                    mDtSensorDeviceMst.AcceptChanges();

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
        /// 行追加ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("DI機器登録 行追加ボタン押下");

                DataRow newDataRow = mDtSensorDeviceMst.NewRow();

                // センサID
                newDataRow[ColumnSensorDeviceMst.SENSOR_ID] = Guid.NewGuid();

                // DI接点番号
                newDataRow[ColumnSensorDeviceMst.INPUT_NO] = mInputList[0];

                // 作成日時
                newDataRow[ColumnSensorDeviceMst.CREATE_DATETIME] = DateTime.Now;

                // データ収集トリガーマスタに同じ品番タイプIDが存在するか
                newDataRow[SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER] = 0;

                mDtSensorDeviceMst.Rows.Add(newDataRow);

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

                //"Button"列ならば、ボタンがクリックされた
                if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE_NAME) && mDtSensorDeviceMst.Rows[e.RowIndex].Field<int>(SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER) < 1)
                {
                    logger.Info("行削除ボタン押下");

                    // 確認メッセージ表示
                    logger.Info("選択中の行を削除してよろしいですか？");
                    if (DialogResult.Yes != MessageBox.Show(this, "選択中の行を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        logger.Info("No押下");
                        return;
                    }

                    logger.Info("Yes押下");

                    // 削除リストに追加
                    mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnSensorDeviceMst.SENSOR_ID].Value);

                    // 行の削除
                    dgvSensorDeviceMst.CellFormatting -= new DataGridViewCellFormattingEventHandler(dgvSensorDeviceMst_CellFormatting);
                    mDtSensorDeviceMst.Rows.RemoveAt(e.RowIndex);
                    dgvSensorDeviceMst.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvSensorDeviceMst_CellFormatting);


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
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 削除アイコンの表示切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSensorDeviceMst_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;

                if (dgv == null)
                {
                    return;
                }
                //"Image"列のセルか確認する
                else if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE_NAME) && 0 <= e.RowIndex)
                {
                    // 選択行とDataTableとのずれを確認
                    if (mDtSensorDeviceMst.Rows.Count <= e.RowIndex)
                    {
                        return;
                    }

                    if (0 < mDtSensorDeviceMst.Rows[e.RowIndex].Field<int>(SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER))
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
        /// DataGridView初期化処理
        /// </summary>
        public void InitializeDataGridView()
        {
            // 削除リストクリア
            mRemoveList.Clear();

            // SQL実行
            mDtSensorDeviceMst = mSqlSensorDeviceMst.Select();

            if (mDtSensorDeviceMst != null)
            {
                // データグリッド行クリア
                dgvSensorDeviceMst.Columns.Clear();

                dgvSensorDeviceMst.AutoGenerateColumns = false;
                var bindingSource = new BindingSource(mDtSensorDeviceMst, string.Empty);
                dgvSensorDeviceMst.DataSource = bindingSource;

                // フォント指定
                DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
                textEditCellStyle.Font = new Font("MS UI Gothic", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(128)));

                // 削除アイコン
                DataGridViewImageColumn imgDelete = new DataGridViewImageColumn();
                imgDelete.Image = global::PLOSMaintenance.Properties.Resources.削除アイコン;
                imgDelete.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgDelete.Name = CLM_DELETE_NAME;
                imgDelete.HeaderText = "";
                imgDelete.FillWeight = 15F;
                imgDelete.SortMode = DataGridViewColumnSortMode.NotSortable;
                imgDelete.Resizable = DataGridViewTriState.False;
                dgvSensorDeviceMst.Columns.Add(imgDelete);

                // センサID
                DataGridViewTextBoxColumn txtSensorId = new DataGridViewTextBoxColumn();
                txtSensorId.DataPropertyName = ColumnSensorDeviceMst.SENSOR_ID;
                txtSensorId.Name = ColumnSensorDeviceMst.SENSOR_ID;
                txtSensorId.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvSensorDeviceMst.Columns.Add(txtSensorId);
                dgvSensorDeviceMst.Columns[ColumnSensorDeviceMst.SENSOR_ID].Visible = false;

                // センサ名
                DataGridViewTextBoxColumn txtSensorName = new DataGridViewTextBoxColumn();
                txtSensorName.DataPropertyName = ColumnSensorDeviceMst.SENSOR_NAME;
                txtSensorName.Name = ColumnSensorDeviceMst.SENSOR_NAME;
                txtSensorName.HeaderText = "センサ名";
                txtSensorName.MaxInputLength = 128;
                txtSensorName.FillWeight = 55F;
                txtSensorName.SortMode = DataGridViewColumnSortMode.NotSortable;
                txtSensorName.DefaultCellStyle = textEditCellStyle;
                txtSensorName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                txtSensorName.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                dgvSensorDeviceMst.Columns.Add(txtSensorName);

                // 接点番号
                DataGridViewComboBoxColumn cmbInputNo = new DataGridViewComboBoxColumn();
                cmbInputNo.DataPropertyName = ColumnSensorDeviceMst.INPUT_NO;
                cmbInputNo.Name = ColumnSensorDeviceMst.INPUT_NO;
                cmbInputNo.HeaderText = "接点番号";
                cmbInputNo.FillWeight = 20F;
                cmbInputNo.SortMode = DataGridViewColumnSortMode.NotSortable;
                cmbInputNo.DefaultCellStyle = textEditCellStyle;
                cmbInputNo.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                cmbInputNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cmbInputNo.Resizable = DataGridViewTriState.False;
                cmbInputNo.FlatStyle = FlatStyle.Flat;
                cmbInputNo.DataSource = mInputList;
                dgvSensorDeviceMst.Columns.Add(cmbInputNo);

                // 機器説明
                DataGridViewTextBoxColumn txtDeviceRemark = new DataGridViewTextBoxColumn();
                txtDeviceRemark.DataPropertyName = ColumnSensorDeviceMst.DEVICE_REMARK;
                txtDeviceRemark.Name = ColumnSensorDeviceMst.DEVICE_REMARK;
                txtDeviceRemark.HeaderText = "機器説明";
                txtDeviceRemark.MaxInputLength = 128;
                txtDeviceRemark.SortMode = DataGridViewColumnSortMode.NotSortable;
                txtDeviceRemark.DefaultCellStyle = textEditCellStyle;
                txtDeviceRemark.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                txtDeviceRemark.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                dgvSensorDeviceMst.Columns.Add(txtDeviceRemark);

                // 作成日時
                DataGridViewTextBoxColumn txtCreateDateTime = new DataGridViewTextBoxColumn();
                txtCreateDateTime.DataPropertyName = ColumnSensorDeviceMst.CREATE_DATETIME;
                txtCreateDateTime.Name = ColumnSensorDeviceMst.CREATE_DATETIME;
                txtCreateDateTime.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvSensorDeviceMst.Columns.Add(txtCreateDateTime);
                dgvSensorDeviceMst.Columns[ColumnSensorDeviceMst.CREATE_DATETIME].Visible = false;

                // データ収集トリガーマスタに同じセンサIDが存在するか
                DataGridViewTextBoxColumn txtExistInTrigger = new DataGridViewTextBoxColumn();
                txtExistInTrigger.DataPropertyName = SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER;
                txtExistInTrigger.Name = SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER;
                txtExistInTrigger.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvSensorDeviceMst.Columns.Add(txtExistInTrigger);
                dgvSensorDeviceMst.Columns[SqlSensorDeviceMst.EXIST_IN_DATA_COLLECTION_TRIGGER].Visible = false;
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
            // 重複確認用リスト
            List<int> doubleNumList = new List<int>();
            List<String> doubleNameList = new List<string>();

            foreach (DataRow rowItem in mDtSensorDeviceMst.Rows)
            {
                // センサ名称の確認
                string sensorName = rowItem[ColumnSensorDeviceMst.SENSOR_NAME] as string;
                if (string.IsNullOrWhiteSpace(sensorName))
                {
                    logger.Warn("センサ名を入力してください。");
                    MessageBox.Show(this, "センサ名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    sensorName = sensorName.Trim();

                    // 重複確認
                    if (doubleNameList.Contains(sensorName))
                    {
                        logger.Warn("センサ名に重複があります。");
                        MessageBox.Show(this, "センサ名に重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        doubleNameList.Add(sensorName);
                    }
                }

                // 接点番号
                int? inputNum = rowItem[ColumnSensorDeviceMst.INPUT_NO] as int?;
                // 数値か否か確認
                if (inputNum == null)
                {
                    logger.Warn("接点番号の入力値が異常です。");
                    MessageBox.Show("接点番号の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    int num = (int)inputNum;

                    // 範囲確認
                    if (num < 1 || 16 < num)
                    {
                        logger.Warn("接点番号が有効な値ではありません。");
                        MessageBox.Show("接点番号が有効な値ではありません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // 重複確認
                    else if (doubleNumList.Contains(num))
                    {
                        logger.Warn("接点番号に重複があります。");
                        MessageBox.Show(this, "接点番号に重複があります。別番号を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        doubleNumList.Add(num);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// センサ機器マスタ更新処理
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="removeList">削除リスト</param>
        private bool UpsertSensorDeviceMst(DataTable dt, List<Guid> removeList)
        {
            bool result = false;

            // 追加/更新処理
            if (mSqlSensorDeviceMst.Upsert(dt))
            {
                logger.Debug("Upsert処理を実行しました。");

                // 削除処理
                if (mSqlSensorDeviceMst.Delete(removeList))
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
