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
    public partial class UCOperating_Shift_Exclusion_Row : UserControl
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        ///  除外区分テーブルアクセス
        /// </summary>
        private SqlExclusionMst mSqlExclusionMst;

        /// <summary>
        /// 除外区分IDリスト
        /// </summary>
        private List<Guid> mExclusionClassList;

        //private List<String> mExclusionNameList;

        private DateTime mTargetDate;

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
        /// 稼働シフト除外時間設定
        /// </summary>
        public UCOperating_Shift_Exclusion_Row()
        {
            InitializeComponent();

            mSqlExclusionMst = new SqlExclusionMst(Properties.Settings.Default.ConnectionString_New);
            mExclusionClassList = new List<Guid>();

            UpdateExclusionClassList(mSqlExclusionMst.Select(), "");

            // イベントの設定
            chkExclusionType.LostFocus += new EventHandler(EditCheck);
            chkbkStartNextDay.LostFocus += new EventHandler(EditCheck);
            chkbkEndNextDay.LostFocus += new EventHandler(EditCheck);
            StartTime.LostFocus += new EventHandler(EditDateTime);
            EndTime.LostFocus += new EventHandler(EditDateTime);
            cmbExclusionClass.LostFocus += new EventHandler(EditCmb);
            txtExclusionRemark.LostFocus += new EventHandler(EditText);

        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 稼働シフト除外区分設定行
        /// </summary>
        [Category("Operating_Shift_Exclusion_Row")]
        [Description("Operating_Shift_Exclusion_Row")]
        public DataRow ExclusionRow
        {
            set
            {
                if (!this.DesignMode)
                {
                    chkExclusionType.Checked = value.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK) > 0 ? true : false;
                    StartTime.Value = value.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME);
                    StartTime.CustomFormat = "HH : mm";
                    EndTime.Value = value.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME);
                    EndTime.CustomFormat = "HH : mm";

                    DateTime targetDate = value.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE);
                    mTargetDate = targetDate;
                    chkbkStartNextDay.Checked = targetDate.Date < StartTime.Value.Date;
                    chkbkEndNextDay.Checked = targetDate.Date < EndTime.Value.Date;

                    cmbExclusionClass.SelectedIndex = mExclusionClassList.IndexOf(value.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS));
                    txtExclusionRemark.Text = value.Field<String>(ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK);
                }
            }
        }

        /// <summary>
        /// 除外開始時刻
        /// </summary>
        public DateTime ExclusionStartTime
        {
            get
            {
                if (chkbkStartNextDay.Checked)
                {
                    var addDay = mTargetDate.AddDays(1);
                    StartTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, StartTime.Value.Hour, StartTime.Value.Minute, 0);
                }
                else
                {
                    StartTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, StartTime.Value.Hour, StartTime.Value.Minute, 0);
                }

                return StartTime.Value;
            }
        }

        /// <summary>
        /// 除外終了時刻
        /// </summary>
        public DateTime ExclusionEndTime
        {
            get
            {
                if (chkbkEndNextDay.Checked)
                {
                    var addDay = mTargetDate.AddDays(1);
                    EndTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, EndTime.Value.Hour, EndTime.Value.Minute, 0);
                }
                else
                {
                    EndTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, EndTime.Value.Hour, EndTime.Value.Minute, 0);
                }

                return EndTime.Value;
            }
        }
        public DateTime TargetDate
        {
            get { return this.mTargetDate; }
            set { this.mTargetDate = value; }
        }
        /// <summary>
        /// 除外チェック有無
        /// </summary>
        public int ExclusionType
        {
            get
            {
                return chkExclusionType.Checked ? 1 : 0;
            }
        }

        /// <summary>
        /// 除外区分ID
        /// </summary>
        public Guid ExclusionClass
        {
            get
            {
                Guid exclusionClass = new Guid();
                if (cmbExclusionClass.SelectedIndex != -1)
                {
                    exclusionClass = mExclusionClassList[cmbExclusionClass.SelectedIndex];
                }

                return exclusionClass;
            }
        }

        /// <summary>
        /// 備考
        /// </summary>
        public string ExclusionRemark
        {
            get
            {
                return txtExclusionRemark.Text;
            }
        }

        /// <summary>
        /// 除外開始時刻翌日フラグ
        /// </summary>
        public Boolean ExclusionStartNextDay
        {
            get { return chkbkStartNextDay.Checked; }
            set { chkbkStartNextDay.Checked = value; }
        }
        /// <summary>
        /// 除外終了時刻翌日フラグ
        /// </summary>
        public Boolean ExclusionEndNextDay
        {
            get { return chkbkEndNextDay.Checked; }
            set { chkbkEndNextDay.Checked = value; }
        }
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 除外区分チェックボックス チェック変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExclusionTypeCheckedChanged(object sender, EventArgs e)
        {
            logger.Info("稼働シフト　除外区分チェック状態変更");
            StartTime.Enabled = EndTime.Enabled = cmbExclusionClass.Enabled = txtExclusionRemark.Enabled = chkExclusionType.Checked;
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 除外区分コンボボックス更新処理
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        public void UpdateExclusionClassList(DataTable dt, string name)
        {
            //logger.Debug("除外区分を更新します。");

            cmbExclusionClass.Items.Clear();
            mExclusionClassList.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cmbExclusionClass.Items.Add(dt.Rows[i].Field<String>(ColumnExclusionMst.EXCLUSION_NAME));
                mExclusionClassList.Add(dt.Rows[i].Field<Guid>(ColumnExclusionMst.EXCLUSION_ID));

                if (dt.Rows[i].Field<string>(ColumnExclusionMst.EXCLUSION_NAME) == name)
                {
                    cmbExclusionClass.Text = dt.Rows[i].Field<string>(ColumnExclusionMst.EXCLUSION_NAME);
                }
            }
        }

        /// <summary>
        /// 稼働シフト除外区分設定行クリア
        /// </summary>
        /// <param name="targetDate">選択日</param>
        public void ClearExclusionRow(DateTime targetDate)
        {
            chkExclusionType.Checked = false;
            StartTime.Value = targetDate.Date;
            EndTime.Value = targetDate.Date;
            chkbkStartNextDay.Checked = false;
            chkbkEndNextDay.Checked = false;
            cmbExclusionClass.SelectedIndex = -1;
            txtExclusionRemark.Text = String.Empty;
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"

        /// <summary>
        /// 対象CheckBoxの編集時に編集フラグを立てます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCheck(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkBox = (CheckBox)sender;

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
        /// 対象DateTimePickerの編集時に編集フラグを立てます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDateTime(object sender, EventArgs e)
        {
            try
            {
                DateTimePicker dtp = (DateTimePicker)sender;
                

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
        /// 対象ComboBoxの編集時に編集フラグを立てます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCmb(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmb = (ComboBox)sender;

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
        /// 対象TextBoxの編集時に編集フラグを立てます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditText(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;

                // 編集フラグを立てる
                FrmMain.gIsDataChange = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion "プライベートメソッド"
    }
}
