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
    /// 品番タイプ登録画面
    /// </summary>
    public partial class UCProductType : UserControl
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
        /// DataGridView列名 品番タイプ名
        /// </summary>
        private const string CLM_TYPE_NAME = "dgvClmTypeName";

        /// <summary>
        /// DataGridView列名 「上へ」ボタン
        /// </summary>
        private const string CLM_UP_BUTTON = "upButton";

        /// <summary>
        /// DataGridView列名 「下へ」ボタン
        /// </summary>
        private const string CLM_DOWN_BUTTON = "downButton";

        /// <summary>
        /// 行の追加、値の変更がされているかを保存します。
        /// </summary>
        private const string ROW_CHANGED = "rowChanged";

        /// <summary>
        /// DBアクセスクラスインスタンス
        /// </summary>
        private SqlProductTypeMst mSqlProductTypeMst;

        /// <summary>
        /// データテーブル
        /// </summary>
        private DataTable mDtProductTypeMst;

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
        /// 品番タイプ登録画面
        /// </summary>
        public UCProductType()
        {
            try
            {
                InitializeComponent();

                // 品番タイプマスタのデータ操作オブジェクトを実装
                mSqlProductTypeMst = new SqlProductTypeMst(Properties.Settings.Default.ConnectionString_New);
                
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
        /// 登録ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRegistButtonClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("登録ボタン押下");

                dgvProductTypeMst.EndEdit();

                // 削除した品番の実績登録確認
                SqlOperatingShiftProductionQuantityTbl sqlOSPQT = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
                DataTable dt = sqlOSPQT.Select();

                foreach (DataRow row in dt.Rows)
                {
                    if (mRemoveList.Contains(row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID)))
                    {
                        logger.Warn($"削除した品番は実績登録されているため、削除できません。" +
                                    $"品番ID：{row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID)}");
                        MessageBox.Show("削除した品番は実績登録されているため、削除できません。\n他の画面タブに切り替えて、品番情報をリセットしてください。",
                                        "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // 入力チェック
                if (CheckInput() == false)
                {
                    return;
                }

                // 確認メッセージ表示
                logger.Info("現在の情報で登録してよろしいですか?");
                if (System.Windows.Forms.DialogResult.Yes != MessageBox.Show(this, "現在の情報で登録してよろしいですか？", "登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    logger.Info("No押下");
                    return;
                }
                logger.Info("Yes押下");

                // 品番タイプマスタ更新処理
                if(UpsertProductTypeMst(mDtProductTypeMst, mRemoveList))
                {
                    logger.Debug("Upsert処理を実行します。");

                    mDtProductTypeMst.AcceptChanges();

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
                logger.Info("行追加ボタン押下");

                DataRow newDataRow = mDtProductTypeMst.NewRow();

                // 品番タイプID
                newDataRow[ColumnProductTypeMst.PRODUCT_TYPE_ID] = Guid.NewGuid();

                // 表示順
                newDataRow[ColumnProductTypeMst.ORDER_IDX] = dgvProductTypeMst.Rows.Count;

                // 標準値品番別マスタに同じ品番タイプIDが存在するか
                newDataRow[SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE] = 0;

                // 行の追加、値の変更がされているかを保存する
                newDataRow[ROW_CHANGED] = true;

                mDtProductTypeMst.Rows.Add(newDataRow);

                // 昇順で並べ替え
                mDtProductTypeMst.DefaultView.Sort = ColumnProductTypeMst.ORDER_IDX + " ASC";

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

                // "削除"ボタン
                if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DELETE_NAME) && mDtProductTypeMst.Rows[e.RowIndex].Field<int>(SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE) < 1)
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
                    mRemoveList.Add((Guid)dgv.Rows[e.RowIndex].Cells[ColumnProductTypeMst.PRODUCT_TYPE_ID].Value);

                    // 行の削除
                    dgvProductTypeMst.CellFormatting -= new DataGridViewCellFormattingEventHandler(dgvProductTypeMst_CellFormatting);
                    mDtProductTypeMst.Rows.Remove(mDtProductTypeMst.DefaultView[e.RowIndex].Row);
                    dgvProductTypeMst.Refresh();
                    dgvProductTypeMst.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvProductTypeMst_CellFormatting);

                    // 表示順を連番にする
                    int col = mDtProductTypeMst.Columns.IndexOf(ColumnProductTypeMst.ORDER_IDX);
                    int lastI = mDtProductTypeMst.DefaultView.Count;

                    for (int i = 0; i < lastI; i++)
                    {
                        mDtProductTypeMst.DefaultView[i][col] = i;
                    }

                    // 昇順で並べ替え
                    mDtProductTypeMst.DefaultView.Sort = ColumnProductTypeMst.ORDER_IDX + " ASC";

                    // 編集フラグを立てる
                    FrmMain.gIsDataChange = true;
                }
                // "上へ"ボタン
                else if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_UP_BUTTON) && 0 < e.RowIndex)
                {
                    logger.Info("上へボタン押下");

                    int dtId1 = mDtProductTypeMst.Rows.IndexOf(mDtProductTypeMst.DefaultView[e.RowIndex - 1].Row);
                    int dtId2 = mDtProductTypeMst.Rows.IndexOf(mDtProductTypeMst.DefaultView[e.RowIndex].Row);

                    // 現在行と上の行のORDER_IDXを取得
                    int viewId1 = mDtProductTypeMst.Rows[dtId1].Field<int>(ColumnProductTypeMst.ORDER_IDX);
                    int viewId2 = mDtProductTypeMst.Rows[dtId2].Field<int>(ColumnProductTypeMst.ORDER_IDX);

                    // 値の入れ替え
                    mDtProductTypeMst.Rows[dtId1][ColumnProductTypeMst.ORDER_IDX] = viewId2;
                    mDtProductTypeMst.Rows[dtId2][ColumnProductTypeMst.ORDER_IDX] = viewId1;
                    

                    // 昇順で並べ替え
                    mDtProductTypeMst.DefaultView.Sort = ColumnProductTypeMst.ORDER_IDX + " ASC";

                    dgv.CurrentCell = dgv[3, viewId1];
                    // 編集フラグを立てる
                    FrmMain.gIsDataChange = true;
                }
                // "下へ"ボタン
                else if (dgv.Columns[e.ColumnIndex].Name.Equals(CLM_DOWN_BUTTON) && e.RowIndex < mDtProductTypeMst.Rows.Count - 1)
                {
                    logger.Info("下へボタン押下");

                    int dtId1 = mDtProductTypeMst.Rows.IndexOf(mDtProductTypeMst.DefaultView[e.RowIndex].Row);
                    int dtId2 = mDtProductTypeMst.Rows.IndexOf(mDtProductTypeMst.DefaultView[e.RowIndex + 1].Row);

                    // 現在行と上の行のORDER_IDXを取得
                    int viewId1 = mDtProductTypeMst.Rows[dtId1].Field<int>(ColumnProductTypeMst.ORDER_IDX);
                    int viewId2 = mDtProductTypeMst.Rows[dtId2].Field<int>(ColumnProductTypeMst.ORDER_IDX);

                    // 値の入れ替え
                    mDtProductTypeMst.Rows[dtId1][ColumnProductTypeMst.ORDER_IDX] = viewId2;
                    mDtProductTypeMst.Rows[dtId2][ColumnProductTypeMst.ORDER_IDX] = viewId1;

                    // 昇順で並べ替え
                    mDtProductTypeMst.DefaultView.Sort = ColumnProductTypeMst.ORDER_IDX + " ASC";

                    dgv.CurrentCell = dgv[4, viewId1];
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
        private void dgvProductTypeMst_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // 行の追加、値の変更がされているかを保存する
                mDtProductTypeMst.DefaultView[e.RowIndex].Row[ROW_CHANGED] = true;

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
        private void dgvProductTypeMst_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
                    if (mDtProductTypeMst.Rows.Count <= e.RowIndex)
                    {
                        return;
                    }

                    if (0 < mDtProductTypeMst.DefaultView[e.RowIndex].Row.Field<int>(SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE))
                    {
                        e.Value = global::PLOSMaintenance.Properties.Resources.利用中アイコン;
                    }
                    else
                    {
                        e.Value = global::PLOSMaintenance.Properties.Resources.削除アイコン;
                    }
                    e.FormattingApplied = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProductTypeMst_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow item in dgvProductTypeMst.Rows)
            {
                DataGridViewCell cell = item.Cells[ColumnProductTypeMst.PRODUCT_TYPE_NAME];
                Color cellColor;

                if (mDtProductTypeMst.DefaultView[cell.RowIndex].Row.Field<bool>(ROW_CHANGED))
                {
                    cellColor = Color.Blue;
                }
                else
                {
                    cellColor = Color.Black;
                }

                // 変更セルを青文字に変更
                DataGridViewCellStyle style = cell.Style;
                style.ForeColor = cellColor;
                cell.Style = style;
            }
        }
        /// <summary>
        /// 行移動ボタンでのEnterKey押下で行を移動します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProductTypeMst_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DataGridView dataGridView = (DataGridView)sender;
                DataGridViewCell cell = dataGridView.CurrentCell;
                DataGridViewCellEventArgs eventArgs = new DataGridViewCellEventArgs(cell.ColumnIndex, cell.RowIndex);

                // 2行目以降の「上へ」ボタン押下
                if (cell.OwningColumn.Name == CLM_UP_BUTTON)
                {
                    if (0 < cell.RowIndex)
                    {
                        OnGridCellContentClicked(dataGridView, eventArgs);
                        //Console.WriteLine("[IF]Col, Row =" + cell.ColumnIndex.ToString() + ", " + cell.RowIndex.ToString());
                    }
                    SendKeys.Send("{UP}");
                }
                // 最終行より前の「下へ」ボタン押下
                else if (cell.OwningColumn.Name == CLM_DOWN_BUTTON && cell.RowIndex < dataGridView.RowCount - 1)
                {
                    OnGridCellContentClicked(dataGridView, eventArgs);
                    //Console.WriteLine("[ELIF]Col, Row ="+ cell.ColumnIndex.ToString() + ", " +cell.RowIndex.ToString());
                }
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
            mDtProductTypeMst = mSqlProductTypeMst.Select();

            // 行の追加、値の変更がされているかを保存する列を追加する
            DataColumn dc = new DataColumn();
            dc.ColumnName = ROW_CHANGED;
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            mDtProductTypeMst.Columns.Add(dc);

            if (mDtProductTypeMst != null)
            {
                // データグリッド行クリア
                dgvProductTypeMst.Columns.Clear();

                dgvProductTypeMst.AutoGenerateColumns = false;
                var bindingSource = new BindingSource(mDtProductTypeMst, string.Empty);
                dgvProductTypeMst.DataSource = bindingSource;

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
                dgvProductTypeMst.Columns.Add(imgDelete);

                // 品番タイプID
                DataGridViewTextBoxColumn txtProductTypeId = new DataGridViewTextBoxColumn();
                txtProductTypeId.DataPropertyName = ColumnProductTypeMst.PRODUCT_TYPE_ID;
                txtProductTypeId.Name = ColumnProductTypeMst.PRODUCT_TYPE_ID;
                txtProductTypeId.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvProductTypeMst.Columns.Add(txtProductTypeId);
                dgvProductTypeMst.Columns[ColumnProductTypeMst.PRODUCT_TYPE_ID].Visible = false;

                // 品番タイプ名
                DataGridViewTextBoxColumn txtProductTypeName = new DataGridViewTextBoxColumn();
                txtProductTypeName.DataPropertyName = ColumnProductTypeMst.PRODUCT_TYPE_NAME;
                txtProductTypeName.Name = ColumnProductTypeMst.PRODUCT_TYPE_NAME;
                txtProductTypeName.HeaderText = "タイプ名";
                txtProductTypeName.MaxInputLength = 127;
                txtProductTypeName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtProductTypeName.FillWeight = 150;
                txtProductTypeName.SortMode = DataGridViewColumnSortMode.NotSortable;
                txtProductTypeName.DefaultCellStyle = textEditCellStyle;
                txtProductTypeName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                txtProductTypeName.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 192);
                dgvProductTypeMst.Columns.Add(txtProductTypeName);

                // 上へボタン
                DataGridViewButtonColumn btnUpColumn = new DataGridViewButtonColumn();
                btnUpColumn.ReadOnly = true;
                btnUpColumn.Resizable = DataGridViewTriState.True;
                btnUpColumn.Name = CLM_UP_BUTTON;
                btnUpColumn.UseColumnTextForButtonValue = true;
                btnUpColumn.Text = "上へ";
                btnUpColumn.HeaderText = "";
                dgvProductTypeMst.Columns.Add(btnUpColumn);

                // 下へボタン
                DataGridViewButtonColumn btnDownColumn = new DataGridViewButtonColumn();
                btnDownColumn.ReadOnly = true;
                btnDownColumn.Resizable = DataGridViewTriState.True;
                btnDownColumn.Name = CLM_DOWN_BUTTON;
                btnDownColumn.UseColumnTextForButtonValue = true;
                btnDownColumn.Text = "下へ";
                btnDownColumn.HeaderText = "";
                dgvProductTypeMst.Columns.Add(btnDownColumn);

                // 表示順
                DataGridViewTextBoxColumn txtOrderIdx = new DataGridViewTextBoxColumn();
                txtOrderIdx.DataPropertyName = ColumnProductTypeMst.ORDER_IDX;
                txtOrderIdx.Name = ColumnProductTypeMst.ORDER_IDX;
                txtOrderIdx.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvProductTypeMst.Columns.Add(txtOrderIdx);
                dgvProductTypeMst.Columns[ColumnProductTypeMst.ORDER_IDX].Visible = false;

                // 標準値品番別マスタに同じ品番タイプIDが存在するか
                DataGridViewTextBoxColumn txtExistInTrigger = new DataGridViewTextBoxColumn();
                txtExistInTrigger.DataPropertyName = SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE;
                txtExistInTrigger.Name = SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE;
                txtExistInTrigger.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvProductTypeMst.Columns.Add(txtExistInTrigger);
                dgvProductTypeMst.Columns[SqlProductTypeMst.EXIST_IN_STANDARD_PRODUCT_TYPE].Visible = false;
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
            List<String> listReaderName = mDtProductTypeMst.AsEnumerable().Select(row => row[ColumnProductTypeMst.PRODUCT_TYPE_NAME].ToString()).ToList<String>();
            int index = 0;
            foreach (DataRow rowItem in mDtProductTypeMst.Rows)
            {
                // 更新か追加された行
                if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
                {
                    // タイプ名
                    if (string.IsNullOrWhiteSpace(rowItem[ColumnProductTypeMst.PRODUCT_TYPE_NAME] as string))
                    {
                        logger.Warn("品番タイプ名を入力してください。");
                        MessageBox.Show(this, "品番タイプ名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // 名称重複チェック
                string[] arr = new string[mDtProductTypeMst.Rows.Count];
                listReaderName.CopyTo(arr);
                List<String> tempList = arr.ToList();
                tempList.RemoveAt(index);
                if (tempList.Contains(rowItem.Field<string>(ColumnProductTypeMst.PRODUCT_TYPE_NAME)))
                {
                    logger.Warn("品番タイプ名に重複があります。");
                    MessageBox.Show(this, "品番タイプ名に重複があります。別名を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                index++;
            }
            return true;
        }

        /// <summary>
        /// 品番タイプマスタ更新処理
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="removeList">削除リスト</param>
        private bool UpsertProductTypeMst(DataTable dt, List<Guid> removeList)
        {
            bool result = false;

            // 追加/更新処理
            if (mSqlProductTypeMst.Upsert(dt))
            {
                logger.Debug("Upsert処理を実行しました。");

                // 削除処理
                if (mSqlProductTypeMst.Delete(removeList))
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
