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
    /// 編成情報ユーザーコントロール
    /// </summary>
	public partial class UCCompositionRow : UserControl
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
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        /// <summary>
        /// 編成情報ユーザーコントロール
        /// </summary>
        public UCCompositionRow() : this(new Guid(), 0, 0, DateTime.Now)
        { }

        /// <summary>
        /// 編成情報ユーザーコントロール
        /// </summary>
        /// <param name="compositionId">構成ID</param>
        /// <param name="numberOfPeople">構成人数</param>
		public UCCompositionRow(Guid compositionId, decimal numberOfPeople, int compositionProcessNum, DateTime dateTime)
        {
            InitializeComponent();

            InitializeComponentData(compositionId, numberOfPeople, compositionProcessNum, dateTime);
        }
        #endregion "コンストラクタ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 削除ボタンクリックイベントデリゲート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="compositionId">構成ID</param>
        public delegate void DeleteClickEventHandler(object sender, Guid compositionId);

        /// <summary>
        /// 削除ボタンをクリックしました。
        /// </summary>
        public event DeleteClickEventHandler DeleteClick;

        /// <summary> 標準値マスタ登録からの発行イベント </summary>
        public delegate void UpdateStdValEventHandler();
        public event UpdateStdValEventHandler EventUpdateStandardVal;

        /// <summary>
        /// CT標準値と機器の設定ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStandardValButton_Clicked(object sender, EventArgs e)
        {
            if(txtUniqueName.Text != "")
            {
                FrmStandardVal_Table form = new FrmStandardVal_Table(CompositionId, lblNumberPeople.Text, txtUniqueName.Text)
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = ParentForm.Location,
                    Size = ParentForm.Size,
                };
                if(!form.IsEnableShowDialog)
                {
                    return;
                }
                form.ShowDialog(this);

                /// 各機器マスタ(品番、カメラ、DI、DO、QRリーダ)の更新イベント発行
                EventUpdateStandardVal?.Invoke();
            }
            else
            {
                logger.Warn("編成名を入力してください。");
                MessageBox.Show("編成名を入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 削除パネル押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteCompositionPanelClicked(object sender, EventArgs e)
        {
            logger.Info("項目削除ボタン押下");

            // 実績登録を確認し、対象編成の削除の可否を判断
            SqlOperatingShiftProductionQuantityTbl sqlOSPQT = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
            DataTable dtOSPQT = sqlOSPQT.Select();
            // 実績あり
            if (0 < dtOSPQT.Select($"{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID} = '{CompositionId}'").Length)
            {
                logger.Warn("選択した編成は実績登録があるため削除できません。");
                MessageBox.Show("選択した編成は実績登録があるため削除できません。", "削除エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // 実績なし
            else
            {
                if (DialogResult.Yes != MessageBox.Show(this, "選択中の項目を削除してよろしいですか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    logger.Info("No押下");
                    return;
                }

                logger.Info("Yes押下");

                // 削除ボタンクリックイベント発生
                DeleteClick?.Invoke(this, CompositionId);
            }
        }

        /// <summary>
        /// 入力制限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUniqueNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // バックスペースが押された時は有効（Deleteキーも有効）
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
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 編成ID
        /// </summary>
        public Guid CompositionId { get; private set; }

        /// <summary>
        /// 編成工程数
        /// </summary>
        public int CompositionProcessNum { get; private set; }

        public DateTime dateTime { get; private set; }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// TextBox編集時の文字色変更
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
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// DataGridView編集時の文字色変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProcess_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
        /// 初期化処理
        /// </summary>
        private void InitializeComponentData(Guid compositionId, decimal numberOfPeople, int compositionProcessNum, DateTime dt)
        {
            InitializeControl();

            // 実績登録を確認し、対象編成の削除の可否を判断
            SqlOperatingShiftProductionQuantityTbl sqlOSPQT = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
            DataTable dtOSPQT = sqlOSPQT.Select();
            // 削除アイコンの表示切替
            if (0 < dtOSPQT.Select($"{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID} = '{compositionId}'").Length)
            {
                pnlDeleteComposition.BackgroundImage = global::PLOSMaintenance.Properties.Resources.利用中アイコン;
                pnlDeleteComposition.Enabled = false;
            }
            else
            {
                pnlDeleteComposition.BackgroundImage = global::PLOSMaintenance.Properties.Resources.削除アイコン;
                pnlDeleteComposition.Enabled = true;
            }

            // 編成ID
            CompositionId = compositionId;
            // 編成人数
            lblNumberPeople.Text = numberOfPeople.ToString() + "人";
            // 編成工程数
            CompositionProcessNum = compositionProcessNum;
            // 作成日時
            dateTime = dt;

            txtUniqueName.LostFocus += new EventHandler(ChangeTextColor);
            txtUniqueNo.LostFocus += new EventHandler(ChangeTextColor);
        }

        /// <summary>
        /// コントロールを初期化します。
        /// </summary>
        private void InitializeControl()
        {
            dgvProcess.Columns.Clear();
            dgvProcess.AutoGenerateColumns = false;

            DataGridViewCellStyle textEditCellStyle = new DataGridViewCellStyle();
            textEditCellStyle.Font = new Font("MS UI Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, (byte)128);

            // 番号
            DataGridViewTextBoxColumn txtDispProcessIdxColumn = new DataGridViewTextBoxColumn();
            txtDispProcessIdxColumn.DataPropertyName = ColumnCompositionProcessMst.PROCESS_IDX;
            txtDispProcessIdxColumn.Name = ColumnCompositionProcessMst.PROCESS_IDX;
            txtDispProcessIdxColumn.HeaderText = "番号";
            txtDispProcessIdxColumn.Width = 70;
            txtDispProcessIdxColumn.ReadOnly = true;
            txtDispProcessIdxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvProcess.Columns.Add(txtDispProcessIdxColumn);

            // 工程名称
            DataGridViewTextBoxColumn txtProcessName = new DataGridViewTextBoxColumn();
            txtProcessName.DataPropertyName = ColumnCompositionProcessMst.PROCESS_NAME;
            txtProcessName.Name = ColumnCompositionProcessMst.PROCESS_NAME;
            txtProcessName.HeaderText = "工程名称";
            txtProcessName.FillWeight = 80;
            txtProcessName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            txtProcessName.MaxInputLength = 50;
            txtProcessName.DefaultCellStyle = textEditCellStyle;
            txtProcessName.SortMode = DataGridViewColumnSortMode.NotSortable; 
            dgvProcess.Columns.Add(txtProcessName);
        }

        #endregion "プライベートメソッド"

        private void txtUniqueNo_Leave(object sender, EventArgs e)
        {
            int outNum = 0;
            int.TryParse(txtUniqueNo.Text, out outNum);

            txtUniqueNo.Text = outNum.ToString("D12");
        }
    }
}
