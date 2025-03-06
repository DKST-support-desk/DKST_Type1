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
    /// ブザー機器登録画面
    /// </summary>
	public partial class UC_Device_DO : UserControl
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
        private readonly List<int> mCmbList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        /// <summary>
        /// DBアクセスクラスインスタンス
        /// </summary>
        private SqlDoDeviceMst mSqlDoDeviceMst;

        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable mDtDoDeviceMst;

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
        /// ブザー機器登録画面
        /// </summary>
        public UC_Device_DO()
        {
            try
            {
                InitializeComponent();

                mSqlDoDeviceMst = new SqlDoDeviceMst(Properties.Settings.Default.ConnectionString_New);

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
                logger.Info("ブザー機器登録 登録ボタンクリック");

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

                // ブザー機器マスタ更新処理
                if(UpsertDoDeviceMst(mDtDoDeviceMst, mRemoveList))
                {
                    mDtDoDeviceMst.AcceptChanges();

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
                logger.Info("ブザー機器登録 行追加ボタン押下");

                DataRow newDataRow = mDtDoDeviceMst.NewRow();

                // センサID
                newDataRow[ColumnDoDeviceMst.DO_ID] = Guid.NewGuid();

                // 長音接点番号
                newDataRow[ColumnDoDeviceMst.LONG_OUTPUT_NO] = mCmbList[0];

                // 短音接点番号
                newDataRow[ColumnDoDeviceMst.SHORT_OUTPUT_NO] = mCmbList[0];

                // 作成日時
                newDataRow[ColumnDoDeviceMst.CREATE_DATETIME] = DateTime.Now;

                // データ収集DO(ブザー)マスタに同じ品番タイプIDが存在するか
                newDataRow[SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO] = 0;

                mDtDoDeviceMst.Rows.Add(newDataRow);

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
                if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE_NAME) && mDtDoDeviceMst.Rows[e.RowIndex].Field<int>(SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO) < 1)
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
                    mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnDoDeviceMst.DO_ID].Value);

                    // 行の削除
                    dataGridView.CellFormatting -= new DataGridViewCellFormattingEventHandler(dataGridView_CellFormatting);
                    mDtDoDeviceMst.Rows.RemoveAt(e.RowIndex);
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
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;

                if (dgv == null)
                {
                    return;
                }
                // "Image"列のセルか確認する
                else if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE_NAME) && 0 <= e.RowIndex)
                {
                    // 選択行とDataTableとのずれを確認
                    if (mDtDoDeviceMst.Rows.Count <= e.RowIndex)
                    {
                        return;
                    }

                    if (0 < mDtDoDeviceMst.Rows[e.RowIndex].Field<int>(SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO))
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
            mDtDoDeviceMst = mSqlDoDeviceMst.Select();

            if (mDtDoDeviceMst != null)
            {
                // データグリッド行クリア
                dataGridView.Columns.Clear();

                dataGridView.AutoGenerateColumns = false;
                var bindingSource = new BindingSource(mDtDoDeviceMst, string.Empty);
                dataGridView.DataSource = bindingSource;

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
                dataGridView.Columns.Add(imgDelete);

                // DOID
                DataGridViewTextBoxColumn txtDoId = new DataGridViewTextBoxColumn();
                txtDoId.DataPropertyName = ColumnDoDeviceMst.DO_ID;
                txtDoId.Name = ColumnDoDeviceMst.DO_ID;
                txtDoId.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtDoId);
                dataGridView.Columns[ColumnDoDeviceMst.DO_ID].Visible = false;

                // ブザー名
                DataGridViewTextBoxColumn txtDoName = new DataGridViewTextBoxColumn();
                txtDoName.DataPropertyName = ColumnDoDeviceMst.DO_NAME;
                txtDoName.Name = ColumnDoDeviceMst.DO_NAME;
                txtDoName.HeaderText = "ブザー名";
                txtDoName.MaxInputLength = 128;
                txtDoName.FillWeight = 50F;
                txtDoName.SortMode = DataGridViewColumnSortMode.NotSortable;
                txtDoName.DefaultCellStyle = textEditCellStyle;
                txtDoName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                txtDoName.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                dataGridView.Columns.Add(txtDoName);

                // 長音接点番号
                DataGridViewComboBoxColumn cmbLongOutputNo = new DataGridViewComboBoxColumn();
                cmbLongOutputNo.DataPropertyName = ColumnDoDeviceMst.LONG_OUTPUT_NO;
                cmbLongOutputNo.Name = ColumnDoDeviceMst.LONG_OUTPUT_NO;
                cmbLongOutputNo.HeaderText = "長音接点番号";
                cmbLongOutputNo.FillWeight = 20F;
                cmbLongOutputNo.SortMode = DataGridViewColumnSortMode.NotSortable;
                cmbLongOutputNo.DefaultCellStyle = textEditCellStyle;
                cmbLongOutputNo.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                cmbLongOutputNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cmbLongOutputNo.Resizable = DataGridViewTriState.False;
                cmbLongOutputNo.FlatStyle = FlatStyle.Flat;
                cmbLongOutputNo.DataSource = mCmbList;
                dataGridView.Columns.Add(cmbLongOutputNo);

                // 短音接点番号
                DataGridViewComboBoxColumn cmbShortOutputNo = new DataGridViewComboBoxColumn();
                cmbShortOutputNo.DataPropertyName = ColumnDoDeviceMst.SHORT_OUTPUT_NO;
                cmbShortOutputNo.Name = ColumnDoDeviceMst.SHORT_OUTPUT_NO;
                cmbShortOutputNo.HeaderText = "短音接点番号";
                cmbShortOutputNo.FillWeight = 20F;
                cmbShortOutputNo.SortMode = DataGridViewColumnSortMode.NotSortable;
                cmbShortOutputNo.DefaultCellStyle = textEditCellStyle;
                cmbShortOutputNo.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                cmbShortOutputNo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cmbShortOutputNo.Resizable = DataGridViewTriState.False;
                cmbShortOutputNo.FlatStyle = FlatStyle.Flat;
                cmbShortOutputNo.DataSource = mCmbList;
                dataGridView.Columns.Add(cmbShortOutputNo);

                // 機器説明
                DataGridViewTextBoxColumn txtDeviceRemark = new DataGridViewTextBoxColumn();
                txtDeviceRemark.DataPropertyName = ColumnDoDeviceMst.DEVICE_REMARK;
                txtDeviceRemark.Name = ColumnDoDeviceMst.DEVICE_REMARK;
                txtDeviceRemark.HeaderText = "機器説明";
                txtDeviceRemark.MaxInputLength = 128;
                txtDeviceRemark.FillWeight = 70F;
                txtDeviceRemark.SortMode = DataGridViewColumnSortMode.NotSortable;
                txtDeviceRemark.DefaultCellStyle = textEditCellStyle;
                txtDeviceRemark.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                txtDeviceRemark.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                dataGridView.Columns.Add(txtDeviceRemark);

                // 作成日時
                DataGridViewTextBoxColumn txtCreateDateTime = new DataGridViewTextBoxColumn();
                txtCreateDateTime.DataPropertyName = ColumnDoDeviceMst.CREATE_DATETIME;
                txtCreateDateTime.Name = ColumnDoDeviceMst.CREATE_DATETIME;
                txtCreateDateTime.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtCreateDateTime);
                dataGridView.Columns[ColumnDoDeviceMst.CREATE_DATETIME].Visible = false;

                // データ収集DO(ブザー)マスタに同じDOIDが存在するか
                DataGridViewTextBoxColumn txtExistInDO = new DataGridViewTextBoxColumn();
                txtExistInDO.DataPropertyName = SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO;
                txtExistInDO.Name = SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO;
                txtExistInDO.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns.Add(txtExistInDO);
                dataGridView.Columns[SqlDoDeviceMst.EXIST_IN_DATA_COLLECTION_DO].Visible = false;
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
            List<int> doubleLongNumList = new List<int>();
            List<int> doubleShortNumList = new List<int>();
            List<String> doubleNameList = new List<string>();

            foreach (DataRow rowItem in mDtDoDeviceMst.Rows)
            {
                // ブザー名
                string buzzorName = rowItem[ColumnDoDeviceMst.DO_NAME] as string;
                if (String.IsNullOrWhiteSpace(buzzorName))
                {
                    logger.Warn("ブザー名を入力してください。");
                    MessageBox.Show(this, "ブザー名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    buzzorName = buzzorName.Trim();

                    // 重複確認
                    if (doubleNameList.Contains(buzzorName))
                    {
                        logger.Warn("ブザー名に重複があります。");
                        MessageBox.Show(this, "ブザー名に重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        doubleNameList.Add(buzzorName);
                    }
                }

                // 長音接点番号
                int? longOutputNum = rowItem[ColumnDoDeviceMst.LONG_OUTPUT_NO] as int?;
                // 数値か否か確認
                if (longOutputNum == null)
                {
                    logger.Warn("長音接点番号の入力値が異常です。");
                    MessageBox.Show("長音接点番号の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    int longNum = (int)longOutputNum;

                    // 範囲確認
                    if (longNum < 1 || 16 < longNum)
                    {
                        logger.Warn("長音接点番号が有効な値ではありません。");
                        MessageBox.Show("長音接点番号が有効な値ではありません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // 重複確認
                    else if (doubleLongNumList.Contains(longNum) && doubleShortNumList.Contains(longNum))
                    {
                        logger.Warn("長音接点番号に重複があります。");
                        MessageBox.Show(this, "長音接点番号に重複があります。別番号を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        doubleLongNumList.Add(longNum);
                        doubleShortNumList.Add(longNum);
                    }
                }

                // 短音接点番号
                int? shortOutputNum = rowItem[ColumnDoDeviceMst.SHORT_OUTPUT_NO] as int?;
                // 数値か否か確認
                if (shortOutputNum == null)
                {
                    logger.Warn("短音接点番号の入力値が異常です。");
                    MessageBox.Show("短音接点番号の入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    int shortNum = (int)shortOutputNum;

                    // 範囲確認
                    if (shortNum < 1 || 16 < shortNum)
                    {
                        logger.Warn("短音接点番号が有効な値ではありません。");
                        MessageBox.Show("短音接点番号が有効な値ではありません。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    // 重複確認
                    else if (doubleShortNumList.Contains(shortNum) && doubleLongNumList.Contains(shortNum))
                    {
                        logger.Warn("短音接点番号に重複があります。");
                        MessageBox.Show(this, "短音接点番号に重複があります。別番号を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        doubleShortNumList.Add(shortNum);
                        doubleLongNumList.Add(shortNum);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// ブザー機器マスタ更新処理
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="removeList">削除リスト</param>
        private bool UpsertDoDeviceMst(DataTable dt, List<Guid> removeList)
        {
            bool result = false;

            // 追加/更新処理
            if (mSqlDoDeviceMst.Upsert(dt))
            {
                logger.Debug("Upsert処理を実行しました。");

                // 削除処理
                if (mSqlDoDeviceMst.Delete(removeList))
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
