using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PLOS.Gui.Core.DataGridViews;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
    public partial class FrmEditProductionQuantity : Form
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// 編成IDカラム
        /// </summary>
        private const string ColumnCompositionId = ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID;

        /// <summary>
        /// 品番タイプIDカラム
        /// </summary>
        private const string ColumnProductTypeId = ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID;

        /// <summary>
        /// 品番タイプ名カラム
        /// </summary>
        private const string ColumnProductTypeName = ColumnProductTypeMst.PRODUCT_TYPE_NAME;

        /// <summary>
        /// 生産数カラム
        /// </summary>
        private const string ColumnProductionQuantity = ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY;

        /// <summary>
        /// 最小生産数
        /// </summary>
        private const int MinProductionQuantity = 0;

        /// <summary>
        /// 最大生産数
        /// </summary>
        private const int MaxProductionQuantity = 99999;

        ///// <summary>
        ///// 表示データリスト
        ///// </summary>
        //private List<DataTable> mDispDataList;

        private Dictionary<Guid, DataTable> mDicDispData;

        /// <summary>
        /// 生産数
        /// </summary>
        private int mProductionQuantity;

        /// <summary>
        /// 選択日
        /// </summary>
        private DateTime mTargetDate;

        private UCOperating_Shift_Item mUCItem;

        /// <summary>
        /// 稼働シフト
        /// </summary>
        private int mOperationShift;

        /// <summary>
        /// サイクル実績テーブルアクセス
        /// </summary>
        private SqlCycleResultTbl mSqlCycleResultTbl;

        private SqlOperatingShiftProductionQuantityTbl mSqlOperatingShiftProductionQuantityTbl;

        /// <summary>
        /// 稼働シフト品番タイプ毎生産数のデータテーブル
        /// </summary>
        private DataTable mOperatingShiftProductionQuantity;

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
        /// 実績生産数入力画面
        /// </summary>
        /// <param name="targetDate">選択日</param>
        /// <param name="operationShift">稼働シフト</param>
        public FrmEditProductionQuantity(DateTime targetDate, int operationShift, DataTable productQuantity, out bool result, UCOperating_Shift_Item ucItem)
        {
            result = true;

            InitializeComponent();

            mTargetDate = targetDate;
            mOperationShift = operationShift;
            mUCItem = ucItem;

            //mDispDataList = new List<DataTable>();
            mDicDispData = new Dictionary<Guid, DataTable>();
            mProductionQuantity = 0;
            mSqlCycleResultTbl = new SqlCycleResultTbl(Properties.Settings.Default.ConnectionString_New);
            mSqlOperatingShiftProductionQuantityTbl = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);

            mOperatingShiftProductionQuantity = productQuantity;

            if (!InitializeDataGridView())
            {
                result = false;
                return;
            }
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 生産数
        /// </summary>
        public int ProductionQuantity
        {
            get
            {
                return mProductionQuantity;
            }
        }

        ///// <summary>
        ///// 実績生産編集後リスト
        ///// </summary>
        //public List<DataTable> ProductionQuantityEditedList
        //{
        //    get
        //    {
        //        return mDispDataList;
        //    }
        //}

        public DataTable OperatingShiftProductionQuantity
        {
            get
            {
                return mOperatingShiftProductionQuantity;
            }
        }
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 適用ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOkClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("実績生産数入力　適用ボタン押下");

                // 生産数設定
                int calcProductionQuantity = 0;
                using (DataTable table = mOperatingShiftProductionQuantity.Clone())
                {
                    // 入力チェック
                    if (CheckInput(out calcProductionQuantity))
                    {
                        mProductionQuantity = calcProductionQuantity;
                        // 編成IDのリスト(タブページごとのリスト)を作成し、そのリスト単位で回す
                        List<Guid> listCompositionId = new List<Guid>(mDicDispData.Keys);
                        for (int i = 0; i < listCompositionId.Count; i++)
                        {
                            DataTable pageData = mDicDispData[listCompositionId[i]];
                            for (int j = 0; j < pageData.Rows.Count; j++)
                            {
                                DataRow dr = table.NewRow();
                                dr[ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE] = mTargetDate;
                                dr[ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT] = mOperationShift;
                                dr[ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID] = listCompositionId[i];
                                dr[ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID] = pageData.Rows[j].Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID);
                                dr[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] = pageData.Rows[j].Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);
                                //dr[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] = mOperatingShiftProductionQuantity.Rows[i].Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);
                                dr[ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE] = mOperatingShiftProductionQuantity.Rows[i].Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);
                                table.Rows.Add(dr);
                            }
                        }

                        mOperatingShiftProductionQuantity = table;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャンセルボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("実績生産数入力　キャンセルボタン押下");

                DialogResult = DialogResult.Cancel;
                Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// セル編集イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProductionQuantity_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;

                // 生産数列
                if (dgv.Columns[e.ColumnIndex].Name.Equals(ColumnProductionQuantity))
                {
                    // 変更セルを青文字に変更
                    DataGridViewCellStyle style = ((DataGridView)sender).CurrentCell.Style;
                    style.ForeColor = Color.Blue;
                    ((DataGridView)sender).CurrentCell.Style = style;

                    // 編集フラグを立てる
                    FrmMain.gIsDataChange = true;
                    //mUCItem.QuantityFlg = true;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// DataGridView初期化処理
        /// </summary>
        private bool InitializeDataGridView()
        {
            tabControl.TabPages.Clear();

            // 実績サイクルテーブルより、選択日、稼働シフトで編成情報取得
            DataTable dtCompositionId = mSqlCycleResultTbl.SelectComposition(mTargetDate, mOperationShift);
            //DataTable dtCompositionId = mSqlOperatingShiftProductionQuantityTbl.SelectComposition(mTargetDate, mOperationShift);


            if (dtCompositionId.Rows.Count < 1)
            {
                MessageBox.Show(String.Format("表示する実績データが存在しません。\n選択稼働日:{0}\n選択シフト:{1}", mTargetDate.ToString("yyyy/MM/dd"), mOperationShift), "表示シフト無し", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            // 編成数分タブページ追加
            for (int i = 0; i < dtCompositionId.Rows.Count; i++)
            {
                // 表示データ
                DataTable dispData = new DataTable();
                dispData.Columns.Add(ColumnCompositionId, typeof(Guid));
                dispData.Columns.Add(ColumnProductTypeId, typeof(Guid));
                dispData.Columns.Add(ColumnProductTypeName, typeof(String));
                dispData.Columns.Add(SqlCycleResultTbl.COLUMN_GOOD_FLAG_COUNT, typeof(int));
                dispData.Columns.Add(ColumnProductionQuantity, typeof(int));

                Guid compositionId = dtCompositionId.Rows[i].Field<Guid>(ColumnCycleResult.COMPOSITION_ID);
                string compositionName = dtCompositionId.Rows[i].Field<string>(ColumnCompositionMst.UNIQUE_NAME);

                //  実績サイクルテーブルより、選択日、稼働シフト、編成IDで品番タイプ情報取得
                DataTable dtProductType = mSqlCycleResultTbl.SelectProductType(mTargetDate, mOperationShift, compositionId);

                // 表示データ作成
                for (int j = 0; j < dtProductType.Rows.Count; j++)
                {
                    Guid productTypeId = dtProductType.Rows[j].Field<Guid>(ColumnCycleResult.PRODUCT_TYPE_ID);
                    string productTypeName = dtProductType.Rows[j].Field<string>(ColumnProductTypeMst.PRODUCT_TYPE_NAME);

                    //  実績サイクルテーブルより、選択日、稼働シフト、編成ID、品番タイプIDで良品工程通過数取得
                    DataTable dtProductionQuantitiy = mSqlCycleResultTbl.SelectProductionQuantity(mTargetDate, mOperationShift, compositionId, productTypeId);

                    dispData.Rows.Add(dispData.NewRow());
                    DataRow addData = dispData.Rows[dispData.Rows.Count - 1];
                    addData[ColumnCompositionId] = compositionId;
                    addData[ColumnProductTypeId] = productTypeId;
                    addData[ColumnProductTypeName] = productTypeName;
                    addData[SqlCycleResultTbl.COLUMN_GOOD_FLAG_COUNT] = dtProductionQuantitiy.Rows[0][SqlCycleResultTbl.COLUMN_GOOD_FLAG_COUNT];

                    Boolean isExistProductionQuantity = true;
                    for (int k = 0; k < mOperatingShiftProductionQuantity.Rows.Count; k++)
                    { 
                        if(mOperatingShiftProductionQuantity.Rows[k][ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY] == DBNull.Value)
                        {
                            isExistProductionQuantity = false;
                            break;
                        }
                    }
                    if(isExistProductionQuantity)
                    {
                        addData[ColumnProductionQuantity] = mOperatingShiftProductionQuantity.AsEnumerable().
                        Where(x => x.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE) == mTargetDate
                           && x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT) == mOperationShift
                           && x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID) == compositionId
                           && x.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID) == productTypeId
                        ).CopyToDataTable().Rows[0].Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);
                    }
                    else
                    {
                        addData[ColumnProductionQuantity] = DBNull.Value;
                    }

                }

                // タブページ追加
                TabPage tabPage = new TabPage();
                tabPage.Name = compositionName;
                tabPage.Text = compositionName;
                tabControl.TabPages.Add(tabPage);

                // データグリッドビュー追加
                DataGridView dataGrid = new DataGridView();
                dataGrid.Dock = DockStyle.Fill;
                dataGrid.AllowUserToAddRows = false;
                dataGrid.AllowUserToDeleteRows = false;
                dataGrid.AllowUserToOrderColumns = false;
                dataGrid.AllowUserToResizeRows = false;
                dataGrid.RowHeadersVisible = false;
                dataGrid.CellEndEdit += dgvProductionQuantity_CellEndEdit;

                BindingSource bindingSource = new BindingSource(dispData, String.Empty);
                dataGrid.Columns.Clear();

                dataGrid.AutoGenerateColumns = false;
                dataGrid.DataSource = bindingSource;

                DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
                textEditCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

                // 編成ID
                DataGridViewTextBoxColumn txtCompositionIdColumn = new DataGridViewTextBoxColumn();
                txtCompositionIdColumn.DataPropertyName = ColumnCompositionId;
                txtCompositionIdColumn.Name = ColumnCompositionId;
                txtCompositionIdColumn.Visible = false;
                txtCompositionIdColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGrid.Columns.Add(txtCompositionIdColumn);

                // 品番タイプID
                DataGridViewTextBoxColumn txtProductTypeIdColumn = new DataGridViewTextBoxColumn();
                txtProductTypeIdColumn.DataPropertyName = ColumnProductTypeId;
                txtProductTypeIdColumn.Name = ColumnProductTypeId;
                txtProductTypeIdColumn.Visible = false;
                txtProductTypeIdColumn.SortMode = DataGridViewColumnSortMode.NotSortable; 
                dataGrid.Columns.Add(txtProductTypeIdColumn);

                // 品番タイプ名
                DataGridViewTextBoxColumn txtProductTypeColumn = new DataGridViewTextBoxColumn();
                txtProductTypeColumn.DataPropertyName = ColumnProductTypeName;
                txtProductTypeColumn.Name = ColumnProductTypeName;
                txtProductTypeColumn.ReadOnly = true;
                txtProductTypeColumn.HeaderText = "品番タイプ";
                txtProductTypeColumn.Width = 150;
                txtProductTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGrid.Columns.Add(txtProductTypeColumn);

                // 良品カウンタ
                DataGridViewTextBoxColumn txtProductionQuantityColumn = new DataGridViewTextBoxColumn();
                txtProductionQuantityColumn.DataPropertyName = SqlCycleResultTbl.COLUMN_GOOD_FLAG_COUNT;
                txtProductionQuantityColumn.Name = SqlCycleResultTbl.COLUMN_GOOD_FLAG_COUNT;
                txtProductionQuantityColumn.ReadOnly = true;
                txtProductionQuantityColumn.HeaderText = "良品カウンタ";
                txtProductionQuantityColumn.Width = 150;
                txtProductionQuantityColumn.SortMode = DataGridViewColumnSortMode.NotSortable; 
                dataGrid.Columns.Add(txtProductionQuantityColumn);

                // 生産数
                DataGridViewNumericColumn txtEditProductionQuantityColumn = new DataGridViewNumericColumn();
                txtEditProductionQuantityColumn.DataPropertyName = ColumnProductionQuantity;
                txtEditProductionQuantityColumn.Name = ColumnProductionQuantity;
                txtEditProductionQuantityColumn.HeaderText = "生産数";
                txtEditProductionQuantityColumn.FillWeight = 50;
                txtEditProductionQuantityColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                txtEditProductionQuantityColumn.MaxLength = 5;
                txtEditProductionQuantityColumn.DefaultCellStyle = textEditCellStyle;
                txtEditProductionQuantityColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGrid.Columns.Add(txtEditProductionQuantityColumn);

                // タブ上にデータグリッドビューを設定
                tabControl.TabPages[i].Controls.Add(dataGrid);

                // リスト追加
                //mDispDataList.Add(dispData);
                mDicDispData.Add(compositionId, dispData);
            }

            return true;
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private Boolean CheckInput(out int sumParam)
        {
            sumParam = 0;

            List<Guid> listCompositionId = new List<Guid>(mDicDispData.Keys);
            for (int i = 0; i < listCompositionId.Count; i++)
            {
                DataTable pageData = mDicDispData[listCompositionId[i]];
                for (int j = 0; j < pageData.Rows.Count; j++)
                {
                    // 生産数の数値範囲を確認
                    if(pageData.Rows[j][ColumnProductionQuantity] == DBNull.Value)
                    {
                        logger.Error("実績生産数入力 生産数未入力");
                        MessageBox.Show(this, "生産数を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    int checkVal = pageData.Rows[j].Field<int>(ColumnProductionQuantity);
                    if (checkVal < MinProductionQuantity || MaxProductionQuantity < checkVal)
                    {
                        logger.Error("実績生産数入力 生産数異常　入力生産数： " + checkVal.ToString());
                        MessageBox.Show(this, "生産数は" + MinProductionQuantity.ToString() + "～" + MaxProductionQuantity.ToString() + "の範囲で入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        sumParam += checkVal;
                    }
                }
            }

            //for (int i = 0; i < mDispDataList.Count; i++)
            //{
            //    for (int j = 0; j < mDispDataList[i].Rows.Count; j++)
            //    {
            //        // 生産数の数値範囲を確認
            //        int checkVal = -1;
            //        int.TryParse(mDispDataList[i].Rows[j][ColumnProductionQuantity].ToString(), out checkVal);

            //        if (checkVal < MinProductionQuantity || MaxProductionQuantity < checkVal)
            //        {
            //            Common.Logger.Instance.WriteTraceLog("実績生産数入力画面 生産数異常 入力生産数： " + checkVal.ToString());
            //            MessageBox.Show(this, "生産数は" + MinProductionQuantity.ToString() + "～" + MaxProductionQuantity.ToString() + "の範囲で入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return false;
            //        }
            //    }
            //}

            return true;
        }
        #endregion "プライベートメソッド
    }
}
