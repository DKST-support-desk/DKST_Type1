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
    /// 編成情報登録画面
    /// </summary>
    public partial class UCComposition : UserControl
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// 削除リスト
        /// </summary>
        public List<Guid> mRemoveList = new List<Guid>();

        /// <summary>
		/// ログインスタンス
		/// </summary>
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary> 標準値マスタ登録からの発行イベント </summary>
        public delegate void UpdateStdValEventHandler();
        public event UpdateStdValEventHandler EventUpdateStandardVal;

        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        /// <summary>
        /// 編成情報登録画面
        /// </summary>
        public UCComposition()
        {
            try
            {
                InitializeComponent();

                InitializeComponentData();
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
        private void btnRegist_Clicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("編成情報登録 登録ボタン押下");

                // 削除した編成の実績登録確認
                SqlOperatingShiftProductionQuantityTbl sqlOSPQT = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
                DataTable dt = sqlOSPQT.Select();

                foreach (DataRow row in dt.Rows)
                {
                    if (mRemoveList.Contains(row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID)))
                    {
                        logger.Warn($"削除した編成は実績登録されているため、削除できません。" +
                                    $"編成ID：{row.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID)}");
                        MessageBox.Show("削除した編成は実績登録されているため、削除できません。\n他の画面タブに切り替えて、編成情報をリセットしてください。",
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
                logger.Info("現在の情報で登録してよろしいですか？");
                if (System.Windows.Forms.DialogResult.Yes != MessageBox.Show(this, "現在の情報で登録してよろしいですか？", "登録確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    logger.Info("No押下");
                    return;
                }
                logger.Info("Yes押下");

                // 編成情報登録マスタ更新処理
                if (UpsertComWithComProMst())
                {
                    //データグリッド初期化処理
                    InitializeComponentData();

                    Uc_EventUpdateStandardVal();
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
        /// 項目追加ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("編成情報登録 項目追加ボタン押下");

                FrmAddComposition frm = new FrmAddComposition();

                if (frm.ShowDialog(ParentForm) != DialogResult.Cancel)
                {
                    UCCompositionRow uc = new UCCompositionRow(frm.CompositionId, frm.numPeopleOrganize, (int)frm.numPeopleOrganize, DateTime.Now);

                    // 値の設定
                    uc.lblNumberPeople.Text = frm.numPeopleOrganize.ToString() + "人";
                    uc.txtUniqueName.Text = string.Empty;
                    uc.txtUniqueNo.Text = string.Empty;
                    uc.DeleteClick += Uc_DeleteClick;
                    uc.EventUpdateStandardVal += Uc_EventUpdateStandardVal;

                    int lastI = (int)frm.numPeopleOrganize;
                    for (int i = 0; i < lastI; i++)
                    {
                        uc.dgvProcess.Rows.Add();
                        int index = uc.dgvProcess.Rows.Count - 1;
                        uc.dgvProcess.Rows[index].Cells[ColumnCompositionProcessMst.PROCESS_IDX].Value = i + 1;
                        uc.dgvProcess.Rows[index].Cells[ColumnCompositionProcessMst.PROCESS_NAME].Value = $"工程({i + 1})";
                    }

                    tlPnl.Controls.Add(uc);

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
        /// 削除ボタンがクリックされました。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="compositionId"></param>
        private void Uc_DeleteClick(object sender, Guid compositionId)
        {
            try
            {
                // 対象のコントロールを削除する
                tlPnl.Controls.Remove((UCCompositionRow)sender);

                // 削除リストに追加
                mRemoveList.Add(compositionId);

                logger.Info("対象項目を削除しました。");

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
        //* プライベートメソッド
        //*******************************************
        #region "プライベートメソッド"
        /// <summary>
        /// テーブルレイアウトパネル初期化
        /// </summary>
        public void InitializeComponentData()
        {
            // レイアウトロジック停止
            tlPnl.SuspendLayout();
            SuspendLayout();

            try
            {
                // SQL実行
                SqlCompositionMst sqlCompositionMst = new SqlCompositionMst(Properties.Settings.Default.ConnectionString_New);

                DataTable dtCompositionMst = sqlCompositionMst.Select();

                if (dtCompositionMst != null)
                {
                    // 要素の削除
                    tlPnl.Controls.Clear();

                    Guid beforeId = new Guid();
                    bool isChanged = false;
                    int processId = 0, pnlId = 0;
                    UCCompositionRow controll = null;

                    for (int iCnt = 0; iCnt < dtCompositionMst.Rows.Count; iCnt++)
                    {
                        var id = dtCompositionMst.Rows[iCnt].Field<Guid>(ColumnCompositionMst.COMPOSITION_ID);

                        // IDが変わった場合、コントロールを追加する
                        isChanged = iCnt == 0 || beforeId != id;
                        beforeId = id;

                        if (isChanged)
                        {
                            tlPnl.RowStyles.Add(new System.Windows.Forms.RowStyle());

                            UCCompositionRow uc = new UCCompositionRow(id, 0, dtCompositionMst.Rows[iCnt].Field<int>(ColumnCompositionMst.COMPOSITION_PROCESS_NUM), DateTime.Now);
                            tlPnl.Controls.Add(uc, iCnt, 0);
                            controll = (UCCompositionRow)tlPnl.Controls[pnlId++];
                            controll.dgvProcess.Rows.Clear();
                            processId = 0;
                        }

                        // コントロールに値を設定する
                        controll.txtUniqueName.Text = dtCompositionMst.Rows[iCnt].Field<string>(ColumnCompositionMst.UNIQUE_NAME);
                        controll.txtUniqueNo.Text = dtCompositionMst.Rows[iCnt].Field<string>(ColumnCompositionMst.COMPOSITION_NUMBER);

                        int? processIdx = dtCompositionMst.Rows[iCnt].Field<int?>(ColumnCompositionMst.PROCESS_IDX);

                        if (processIdx != null)
                        {
                            controll.dgvProcess.Rows.Add();

                            controll.dgvProcess.Rows[processId].Cells[ColumnCompositionProcessMst.PROCESS_IDX].Value = processIdx + 1;

                            controll.dgvProcess.Rows[processId].Cells[ColumnCompositionProcessMst.PROCESS_NAME].Value =
                                dtCompositionMst.Rows[iCnt].Field<string>(ColumnCompositionMst.PROCESS_NAME);

                            controll.lblNumberPeople.Text = (++processId).ToString() + "人";
                        }

                        controll.DeleteClick += Uc_DeleteClick;
                        controll.EventUpdateStandardVal += Uc_EventUpdateStandardVal;
                    }

                    for (int iCnt = 0; iCnt < tlPnl.Controls.Count; iCnt++)
                    {
                        controll = (UCCompositionRow)tlPnl.Controls[iCnt];

                        // 取得した編成に紐づく工程数と登録されている編成工程数が違う場合
                        if (controll.dgvProcess.Rows.Count != controll.CompositionProcessNum)
                        {
                            // 編成人数 空欄表示
                            controll.lblNumberPeople.Text = String.Empty;

                            // データグリッドビュークリア
                            controll.dgvProcess.Columns.Clear();
                            controll.dgvProcess.Rows.Clear();

                            // 標準値マスタボタン無効
                            controll.btnStandardVal.Enabled = false;
                        }
                    }
                }
            }
            finally
            {
                // レイアウトロジック再開
                tlPnl.ResumeLayout(false);
                tlPnl.PerformLayout();
                ResumeLayout(false);
            }
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            // 重複確認用リスト
            List<string> nameDoubleCheck = new List<string>();
            List<string> noDoubleCheck = new List<string>();

            foreach (UCCompositionRow item in tlPnl.Controls)
            {
                // 編成名の確認
                item.txtUniqueName.Text = item.txtUniqueName.Text.Trim();
                if (string.IsNullOrWhiteSpace(item.txtUniqueName.Text))
                {
                    logger.Warn("編成名を入力してください。");
                    MessageBox.Show(this, "編成名を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // 文字数確認
                else if (item.txtUniqueName.Text.Length > 50)
                {
                    logger.Warn("編成名の文字数が範囲外です。");
                    MessageBox.Show("編成名の文字数が範囲外です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // 重複確認
                else if (nameDoubleCheck.Contains(item.txtUniqueName.Text))
                {
                    logger.Warn("編成名重複があります。");
                    MessageBox.Show(this, "編成名重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    nameDoubleCheck.Add(item.txtUniqueName.Text);
                }

                // 編成Noの確認
                item.txtUniqueNo.Text = item.txtUniqueNo.Text.Trim();
                if (string.IsNullOrWhiteSpace(item.txtUniqueNo.Text))
                {
                    logger.Warn("編成Noを入力してください。");
                    MessageBox.Show(this, "編成Noを入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // 数値か否か確認
                else if (!long.TryParse(item.txtUniqueNo.Text, out long uniqueNum))
                {
                    logger.Warn("編成Noの入力値が異常です。");
                    MessageBox.Show("編成Noの入力値が異常です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // 文字数確認
                else if (12 < item.txtUniqueNo.Text.Length)
                {
                    logger.Warn("編成Noが範囲外の入力値です。");
                    MessageBox.Show("編成Noが範囲外の入力値です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                // 重複確認
                else if (noDoubleCheck.Contains(item.txtUniqueNo.Text))
                {
                    logger.Warn("編成No重複があります。");
                    MessageBox.Show(this, "編成No重複があります。別番号を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    noDoubleCheck.Add(item.txtUniqueNo.Text);
                }

                // 工程名称の確認
                List<string> processDoubleCheck = new List<string>();
                foreach (DataGridViewRow rowItem in item.dgvProcess.Rows)
                {
                    string processName = rowItem.Cells[ColumnCompositionProcessMst.PROCESS_NAME].Value as string;
                    if (string.IsNullOrWhiteSpace(processName))
                    {
                        logger.Warn("工程名称を入力してください。");
                        MessageBox.Show(this, "工程名称を入力してください", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        processName = processName.Trim();

                        // 文字数確認
                        if (50 < processName.Length)
                        {
                            logger.Warn("工程名称の文字数が範囲外です。");
                            MessageBox.Show("工程名称の文字数が範囲外です。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        // 重複確認
                        else if (processDoubleCheck.Contains(processName))
                        {
                            logger.Warn($"編成名'{item.txtUniqueName.Text}'に工程名称重複があります。");
                            MessageBox.Show(this, $"編成名'{item.txtUniqueName.Text}'に工程名称重複があります。別名称を登録してください。", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else
                        {
                            processDoubleCheck.Add(processName);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 編成マスタ, 編成プロセスマスタ更新処理
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="removeList">削除リスト</param>
        private bool UpsertComWithComProMst()
        {
            bool result = false;

            try
            {
                var now = DateTime.Now;

                // データテーブル作成
                DataTable dtCompositionMst = new DataTable();
                dtCompositionMst.Columns.Add(ColumnCompositionMst.COMPOSITION_ID, typeof(Guid));
                dtCompositionMst.Columns.Add(ColumnCompositionMst.UNIQUE_NAME, typeof(string));
                dtCompositionMst.Columns.Add(ColumnCompositionMst.COMPOSITION_NUMBER, typeof(string));
                dtCompositionMst.Columns.Add(ColumnCompositionMst.COMPOSITION_PROCESS_NUM, typeof(int));
                dtCompositionMst.Columns.Add(ColumnCompositionMst.CREATE_DATETIME, typeof(DateTime));

                DataTable dtCompositionProcessMst = new DataTable();
                dtCompositionProcessMst.Columns.Add(ColumnCompositionProcessMst.COMPOSITION_ID, typeof(Guid));
                dtCompositionProcessMst.Columns.Add(ColumnCompositionProcessMst.PROCESS_IDX, typeof(int));
                dtCompositionProcessMst.Columns.Add(ColumnCompositionProcessMst.PROCESS_NAME, typeof(string));

                // コントロールからデータテーブルに値を読み込む
                foreach (UCCompositionRow item in tlPnl.Controls)
                {
                    object[] data = new object[]
                    {
                        item.CompositionId,
                        item.txtUniqueName.Text,
                        item.txtUniqueNo.Text,
                        item.CompositionProcessNum,
                        item.dateTime,
                    };

                    dtCompositionMst.Rows.Add(data);

                    foreach (DataGridViewRow processItem in item.dgvProcess.Rows)
                    {
                        object[] processData = new object[]
                        {
                            item.CompositionId,
                            processItem.Cells[ColumnCompositionProcessMst.PROCESS_IDX].Value,
                            processItem.Cells[ColumnCompositionProcessMst.PROCESS_NAME].Value,
                        };

                        dtCompositionProcessMst.Rows.Add(processData);
                    }
                }

                // 編成マスタ, 編成プロセスマスタ更新処理
                SqlCompositionMst sqlCompositionMst = new SqlCompositionMst(Properties.Settings.Default.ConnectionString_New);
                SqlCompositionProcessMst sqlCompositionProcessMst = new SqlCompositionProcessMst(Properties.Settings.Default.ConnectionString_New);

                if (sqlCompositionMst.Upsert(dtCompositionMst) && sqlCompositionProcessMst.Upsert(dtCompositionProcessMst))
                {
                    logger.Debug("Upsert処理を実行しました。");

                    // 編成ID該当マスタの削除処理
                    if (sqlCompositionMst.DeleteMultipleTable(mRemoveList) && sqlCompositionProcessMst.Delete(mRemoveList))
                    {
                        logger.Debug("Delete処理を実行しました。");
                        result = true;
                    }
                    else
                    {
                        logger.Debug("Delete処理に失敗しました。");
                    }
                }
                else
                {
                    logger.Debug("Upsert処理に失敗しました。");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        /// <summary>
        /// イベントメソッド：各機器マスタ(品番、カメラ、DI、DO、QRリーダ)の更新処理
        /// </summary>
        private void Uc_EventUpdateStandardVal()
        {
            EventUpdateStandardVal.Invoke();
        }
        #endregion "プライベートメソッド"
    }
}
