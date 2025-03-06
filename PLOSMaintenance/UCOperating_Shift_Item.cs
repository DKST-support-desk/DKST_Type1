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
    /// 日付パターン選択
    /// </summary>
    public enum SELECT_DATE_STATUS
    {
        /// <summary> 過去の選択 </summary>
        PAST,
        /// <summary> 本日の選択 </summary>
        TODAY,
        /// <summary> 未来の選択 </summary>
        FUTURE
    }

    public partial class UCOperating_Shift_Item : UserControl
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 選択日
        /// </summary>
        private DateTime mTargetDate;

        /// <summary>
        /// 稼働シフト
        /// </summary>
        private int mOperationShift;

        /// <summary>
        /// 計画稼働シフト入力有効/無効
        /// </summary>
        public Boolean mEnablePlan;

        /// <summary>
        /// 実績稼働シフト入力有効/無効
        /// </summary>
        public Boolean mEnableActual;

        /// <summary>
        /// 計画稼働シフトデータ
        /// </summary>
        private DataTable mPlanOperatingShift;

        /// <summary>
        /// 実績稼働シフトデータ
        /// </summary>
        private DataTable mActualOperatingShift;

        /// <summary>
        /// 稼働シフト除外時間データ
        /// </summary>
        private DataTable mExclusionOperatingShift;

        /// <summary>
        /// 実績生産編集日リスト
        /// </summary>
        private List<DateTime> mDateQuantityList;

        /// <summary>
        /// 除外区分テーブルアクセス
        /// </summary>
        private SqlExclusionMst mSqlExclusionMst;

        /// <summary>
        /// 除外区分設定行リスト
        /// </summary>
        private List<UCOperating_Shift_Exclusion_Row> mExclusionRowList;

        /// <summary>
        /// 実績生産数変更イベント デリゲート
        /// </summary>
        public event EventHandler ProductionQuantityChanged;

        /// <summary>
        /// 除外区分変更イベント デリゲート
        /// </summary>
        public event EventHandler ExclusionClassChanged;

        /// <summary>
        /// 実績生産数テーブルのデータテーブル
        /// </summary>
        private DataTable mDtProductionQuantity;

        /// <summary> 稼働シフトパターンテーブルアクセス </summary>
        private SqlOperatingShiftPatternTbl mSqlOperatingShiftPatternTbl;

        /// <summary> 稼働除外パターンテーブルアクセス </summary>
        private SqlOperatingShiftExclusionPatternTbl mSqlOperatingShiftExclusionPatternTbl;

        /// <summary> 稼働シフトパターンテーブルデータ </summary>
        private DataTable mDtSqlOperatingShiftTbl;

        /// <summary> 稼働除外パターンテーブルデータ </summary>
        private DataTable mDtOperatingShiftExclusionPatternTbl;

        /// <summary>
        /// 選択日ステータス(本日に対して過去、現在、未来)
        /// </summary>
        private SELECT_DATE_STATUS mSelectDateStatus;

        ///// <summary>
        ///// 表示中除外区分名リスト
        ///// </summary>
        //private List<String> mExclusionNameList;
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        /// <summary>
        /// 勤帯パターン
        /// </summary>
        public UCOperating_Shift_Item()
        {
            InitializeComponent();

            mTargetDate = DateTime.Today;
            mOperationShift = 1;
            mEnablePlan = false;
            mEnableActual = false;
            mPlanOperatingShift = new DataTable(PlanOperatingShiftTblColumn.TABLE_NAME);
            mActualOperatingShift = new DataTable(ColumnResultOperatingShiftTbl.TABLE_NAME);
            mExclusionOperatingShift = new DataTable(ColumnOperatingShiftExclusionTbl.TABLE_NAME);
            //mProductionQuantityEditedList = new List<DataTable>();
            mSqlExclusionMst = new SqlExclusionMst(Properties.Settings.Default.ConnectionString_New);
            mExclusionRowList = new List<UCOperating_Shift_Exclusion_Row>();
            mDateQuantityList = new List<DateTime>();

            mSqlOperatingShiftPatternTbl = new SqlOperatingShiftPatternTbl(Properties.Settings.Default.ConnectionString_New);
            mSqlOperatingShiftExclusionPatternTbl = new SqlOperatingShiftExclusionPatternTbl(Properties.Settings.Default.ConnectionString_New);

            // 稼働シフトパターンテーブルからの全件取得
            mDtSqlOperatingShiftTbl = mSqlOperatingShiftPatternTbl.Select();
            // 稼働除外パターンテーブルからの全件取得
            mDtOperatingShiftExclusionPatternTbl = mSqlOperatingShiftExclusionPatternTbl.Select();

            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row1);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row2);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row3);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row4);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row5);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row6);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row7);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row8);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row9);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row10);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row11);
            mExclusionRowList.Add(ucOperating_Shift_Exclusion_Row12);

            CheckBox[] checkBoxes = { chkbxUse, chkbkPlanStartNextDay, chkbkPlanEndNextDay, chkbkActualStartNextDay, chkbkActualEndNextDay};
            foreach(CheckBox checkBox in checkBoxes)
            {
                checkBox.LostFocus += new EventHandler(EditCheck);
            }
            DateTimePicker[] dtps = { PlanStartTime, PlanEndTime, ActualStartTime, ActualEndTime };
            foreach (DateTimePicker dtp in dtps)
            {
                dtp.LostFocus += new EventHandler(EditDateTime);
            }
            OperationSecond.LostFocus += new EventHandler(EditText);
            PlanProductionQuantity.LostFocus += new EventHandler(EditText);
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 稼働シフト
        /// </summary>
        [Category("Calendar_Day")]
        [Description("Calendar_Day")]
        [DefaultValue(1)]
        public int OperationShift
        {
            get
            {
                return mOperationShift;
            }
            set
            {
                mOperationShift = value;
                label2.Text = $"{mOperationShift} 直";
            }
        }

        /// <summary>
        /// 選択日
        /// </summary>
        [Category("Calendar_Day")]
        [Description("Calendar_Day")]
        [DefaultValue("")]
        public DateTime TargetDate
        {
            get
            {
                return mTargetDate;
            }
            set
            {
                mTargetDate = value.Date;
                foreach (var item in mExclusionRowList)
                {
                    item.TargetDate = mTargetDate;
                }
            }
        }

        /// <summary>
        /// 計画稼働シフト入力有効/無効
        /// </summary>
        public Boolean EnablePlan
        {
            get
            {
                return mEnablePlan;
            }
            set
            {
                mEnablePlan = value;
                tlpOperationSecond.Enabled = mEnablePlan;
                tlpPlanProductionQuantity.Enabled = mEnablePlan;
            }
        }

        /// <summary>
        /// 実績稼働シフト入力有効/無効
        /// </summary>
        public Boolean EnableActual
        {
            get
            {
                return mEnableActual;
            }
            set
            {
                mEnableActual = value;
            }
        }

        /// <summary>
        /// シフト入力有効/無効
        /// </summary>
        public Boolean UseShift
        {
            get
            {
                return chkbxUse.Checked;
            }
            set
            {
                chkbxUse.Checked = value;
            }
        }

        /// <summary>
        /// 計画稼働シフト
        /// </summary>
        public DataTable PlanOperatingShift
        {
            get
            {
                //if (mEnablePlan)
                if (UseShift)
                {
                    if (mPlanOperatingShift.Rows.Count == 0)
                    {
                        mPlanOperatingShift.Rows.Add(mPlanOperatingShift.NewRow());
                    }

                    DataRow planData = mPlanOperatingShift.Rows[0];
                    planData[PlanOperatingShiftTblColumn.USE_FLAG] = chkbxUse.Checked;
                    planData[PlanOperatingShiftTblColumn.OPERATION_DATE] = mTargetDate;
                    planData[PlanOperatingShiftTblColumn.OPERATION_SHIFT] = mOperationShift;
                    planData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = PlanProductionQuantity.Value;
                    planData[PlanOperatingShiftTblColumn.OPERATION_SECOND] = OperationSecond.Value;

                    if (chkbkPlanStartNextDay.Checked)
                    {
                        var addDay = mTargetDate.AddDays(1);
                        PlanStartTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, PlanStartTime.Value.Hour, PlanStartTime.Value.Minute, 0);
                    }
                    else
                    {
                        PlanStartTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, PlanStartTime.Value.Hour, PlanStartTime.Value.Minute, 0);
                    }
                    planData[PlanOperatingShiftTblColumn.START_TIME] = PlanStartTime.Value;

                    if (chkbkPlanEndNextDay.Checked)
                    {
                        var addDay = mTargetDate.AddDays(1);
                        PlanEndTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, PlanEndTime.Value.Hour, PlanEndTime.Value.Minute, 0);
                    }
                    else
                    {
                        PlanEndTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, PlanEndTime.Value.Hour, PlanEndTime.Value.Minute, 0);
                    }
                    planData[PlanOperatingShiftTblColumn.END_TIME] = PlanEndTime.Value;
                }

                return mPlanOperatingShift;
            }
            set
            {
                mPlanOperatingShift = value;
                if (mPlanOperatingShift != null && 0 < mPlanOperatingShift.Rows.Count)
                {
                    DataRow planData = mPlanOperatingShift.Rows[0];
                    PlanProductionQuantity.Value = (int)planData[PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY];

                    PlanStartTime.Value = (DateTime)planData[PlanOperatingShiftTblColumn.START_TIME];
                    PlanStartTime.CustomFormat = "HH : mm";

                    PlanEndTime.Value = (DateTime)planData[PlanOperatingShiftTblColumn.END_TIME];
                    PlanEndTime.CustomFormat = "HH : mm";

                    chkbkPlanStartNextDay.Checked = mTargetDate.Date < PlanStartTime.Value.Date;
                    chkbkPlanEndNextDay.Checked = mTargetDate.Date < PlanEndTime.Value.Date;

                    decimal tempSecond = 0;
                    if(decimal.TryParse(planData[PlanOperatingShiftTblColumn.OPERATION_SECOND].ToString(), out tempSecond))
                    {
                        OperationSecond.Value = tempSecond;
                    }
                    else
                    {
                        OperationSecond.Value = 0;
                    }
                    var temp = planData[PlanOperatingShiftTblColumn.USE_FLAG];
                    if(temp == null || temp.Equals(System.DBNull.Value))
                    {
                        if (OperationSecond.Value > 0 && PlanProductionQuantity.Value > 0)
                        {
                            chkbxUse.Checked = true;
                        }
                        else
                        {
                            chkbxUse.Checked = false;
                        }
                    }
                    else
                    {
                        chkbxUse.Checked = (Boolean)planData[PlanOperatingShiftTblColumn.USE_FLAG];
                    }
                }
                else
                {
                    ClearPlanOperatingShiftItem();
                }
            }
        }

        /// <summary>
        /// 実績稼働シフト
        /// </summary>
        public DataTable ActualOperatingShift
        {
            get
            {
                if (!this.DesignMode)
                {
                    if (mEnableActual)
                    {
                        if(mActualOperatingShift == null)
                        {
                            return null;
                        }

                        if (mActualOperatingShift.Rows.Count == 0)
                        {
                            mActualOperatingShift.Rows.Add(mActualOperatingShift.NewRow());
                        }

                        DataRow actualData = mActualOperatingShift.Rows[0];

                        actualData[ColumnResultOperatingShiftTbl.OPERATION_DATE] = mTargetDate;
                        actualData[ColumnResultOperatingShiftTbl.OPERATION_SHIFT] = mOperationShift;

                        if (chkbkActualStartNextDay.Checked)
                        {
                            var addDay = mTargetDate.AddDays(1);
                            ActualStartTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, ActualStartTime.Value.Hour, ActualStartTime.Value.Minute, 0);
                        }
                        else
                        {
                            ActualStartTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, ActualStartTime.Value.Hour, ActualStartTime.Value.Minute, 0);
                        }


                        actualData[ColumnResultOperatingShiftTbl.START_TIME] = ActualStartTime.Value;

                        if (chkbkActualEndNextDay.Checked)
                        {
                            var addDay = mTargetDate.AddDays(1);
                            ActualEndTime.Value = new DateTime(addDay.Year, addDay.Month, addDay.Day, ActualEndTime.Value.Hour, ActualEndTime.Value.Minute, 0);
                        }
                        else
                        {
                            ActualEndTime.Value = new DateTime(mTargetDate.Year, mTargetDate.Month, mTargetDate.Day, ActualEndTime.Value.Hour, ActualEndTime.Value.Minute, 0);
                        }
                        actualData[ColumnResultOperatingShiftTbl.END_TIME] = ActualEndTime.Value;
                    }

                    return mActualOperatingShift;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!this.DesignMode)
                {
                    mActualOperatingShift = value;
                    if (mActualOperatingShift != null)
                    {
                        if (0 < mActualOperatingShift.Rows.Count)
                        {
                            DataRow actualData = mActualOperatingShift.Rows[0];
                            ActualStartTime.Value = (DateTime)actualData[ColumnResultOperatingShiftTbl.START_TIME];
                            ActualStartTime.CustomFormat = "HH : mm";
                            ActualEndTime.Value = (DateTime)actualData[ColumnResultOperatingShiftTbl.END_TIME];
                            ActualEndTime.CustomFormat = "HH : mm";
                            chkbkActualStartNextDay.Checked = mTargetDate.Date < ActualStartTime.Value.Date;
                            chkbkActualEndNextDay.Checked = mTargetDate.Date < ActualEndTime.Value.Date;
                        }
                        else
                        {
                            ClearActualOperatingShiftItem();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 実績生産数テーブル
        /// </summary>
        public DataTable DtProductionQuantity
        {
            get { return this.mDtProductionQuantity; }
            set { this.mDtProductionQuantity = value; }
        }

        /// <summary>
        /// 実績生産数
        /// </summary>
        public int ActualProductionQuantity
        {
            get
            {
                if(int.TryParse(lblActualProductionQuantity.Text, out int quantity))
                {
                    return quantity;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                lblActualProductionQuantity.Text = value.ToString();
            }
        }

        /// <summary>
        /// 稼働シフト除外時間
        /// </summary>
        public DataTable ExclusionOperatingShift
        {
            get
            {
                if (!this.DesignMode)
                {
                    if(mExclusionOperatingShift == null)
                    {
                        return null;
                    }

                    for (int i = 0; i < mExclusionRowList.Count; i++)
                    {
                        if (mExclusionOperatingShift.Rows.Count != mExclusionRowList.Count)
                        {
                            mExclusionOperatingShift.Rows.Add(mExclusionOperatingShift.NewRow());
                        }
                        DataRow exclusionData = mExclusionOperatingShift.Rows[i];

                        exclusionData[ColumnOperatingShiftExclusionTbl.OPERATION_DATE] = mTargetDate;
                        exclusionData[ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT] = mOperationShift;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX] = i;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME] = mExclusionRowList[i].ExclusionStartTime;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME] = mExclusionRowList[i].ExclusionEndTime;

                        TimeSpan interval = mExclusionRowList[i].ExclusionEndTime - mExclusionRowList[i].ExclusionStartTime;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME] = interval.TotalSeconds;

                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK] = mExclusionRowList[i].ExclusionType;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS] = mExclusionRowList[i].ExclusionClass;
                        exclusionData[ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK] = mExclusionRowList[i].ExclusionRemark;
                    }

                    return mExclusionOperatingShift;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (!this.DesignMode)
                {
                    mExclusionOperatingShift = value;
                    if (mExclusionOperatingShift != null)
                    {
                        if (0 < mExclusionOperatingShift.Rows.Count)
                        {
                            for (int i = 0; i < mExclusionRowList.Count && i < mExclusionOperatingShift.Rows.Count; i++)
                            {
                                mExclusionRowList[i].ExclusionRow = mExclusionOperatingShift.Rows[i];
                            }
                        }
                        else
                        {
                            ClearExclusionOperatingShiftItem();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 日付ステータス
        /// </summary>
        public SELECT_DATE_STATUS SelectDateStatus
        {
            get 
            { 
                return mSelectDateStatus; 
            }
            set
            {
                mSelectDateStatus = value;
                tplShiftArea.Enabled = true;
                switch (mSelectDateStatus)
                {
                    case SELECT_DATE_STATUS.PAST:
                        EnablePlan = true;
                        // 過去日選択(シフト有効チェックボックスの無効化)
                        tlpShiftUseFlagCheck.Enabled = false;
                        // 計画入力領域の無効化
                        lblOperationSecondTitle.Enabled = false;
                        tlpOperationSecond.Enabled = false;
                        lblPlanProductionQuantityTitle.Enabled = false;
                        tlpPlanProductionQuantity.Enabled = false;
                        tlpPlanStartEnd.Enabled = false;

                        // 選択日が登録されていて、かつシフト入力にチェックが入っている場合に実績入力領域を有効化
                        if (mPlanOperatingShift.AsEnumerable().Select(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE)).Contains(mTargetDate.Date)
                            && chkbxUse.Checked == true)
                        {
                            // 実績入力領域の有効化
                            btnClear.Enabled = true;
                            lblActualProductionQuantityTitle.Enabled = true;
                            tlpEditProductQuantity.Enabled = true;

                            // 実績生産数の有無で編集ボタンと実績稼働時刻の表示を切り替え
                            if (0 < ActualProductionQuantity)
                            {
                                tlpActualWorkTime.Enabled = true;
                            }
                            else
                            {
                                tlpActualWorkTime.Enabled = false;
                            }
                        }
                        else
                        {
                            // 実績入力領域の無効化
                            btnClear.Enabled = false;
                            lblActualProductionQuantityTitle.Enabled = false;
                            tlpEditProductQuantity.Enabled = false;
                            tlpActualWorkTime.Enabled = false;
                        }

                        // 除外時間関連処理を無効化
                        if (tlpPlanStartEnd.Enabled || tlpActualWorkTime.Enabled)
                        {
                            pnlExclusionTitle.Enabled = true;
                            tlpExclusionRowList.Enabled = true;
                        }
                        else
                        {
                            pnlExclusionTitle.Enabled = false;
                            tlpExclusionRowList.Enabled = false;
                        }
                        break;

                    case SELECT_DATE_STATUS.TODAY:
                        tlpShiftUseFlagCheck.Enabled = true;

                        // 計画入力領域の有効化
                        lblOperationSecondTitle.Enabled = chkbxUse.Checked;
                        tlpOperationSecond.Enabled = chkbxUse.Checked;
                        lblPlanProductionQuantityTitle.Enabled = chkbxUse.Checked;
                        tlpPlanProductionQuantity.Enabled = chkbxUse.Checked;
                        tlpPlanStartEnd.Enabled = chkbxUse.Checked;

                        // 選択日が登録されていて、かつシフト入力にチェックが入っている場合に実績入力領域を有効化
                        if (mPlanOperatingShift.AsEnumerable().Select(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE)).Contains(mTargetDate.Date)
                            && chkbxUse.Checked == true)
                        {
                            // 実績入力領域の有効化
                            btnClear.Enabled = true;
                            lblActualProductionQuantityTitle.Enabled = true;
                            tlpEditProductQuantity.Enabled = true;

                            // 実績生産数の有無で編集ボタンと実績稼働時刻の表示を切り替え
                            if (0 < ActualProductionQuantity)
                            {
                                tlpActualWorkTime.Enabled = true;
                            }
                            else
                            {
                                tlpActualWorkTime.Enabled = false;
                            }
                        }
                        else
                        {
                            // 実績入力領域の無効化
                            btnClear.Enabled = false;
                            lblActualProductionQuantityTitle.Enabled = false;
                            tlpEditProductQuantity.Enabled = false;
                            tlpActualWorkTime.Enabled = false;
                        }

                        // 除外時間関連処理を有効化
                        if (tlpPlanStartEnd.Enabled || tlpActualWorkTime.Enabled)
                        {
                            pnlExclusionTitle.Enabled = true;
                            tlpExclusionRowList.Enabled = true;
                        }
                        else
                        {
                            pnlExclusionTitle.Enabled = false;
                            tlpExclusionRowList.Enabled = false;
                        }
                        break;

                    case SELECT_DATE_STATUS.FUTURE:
                        EnablePlan = false;
                        // 未来日選択(シフト有効チェックボックスの有効化)
                        tlpShiftUseFlagCheck.Enabled = true;

                        // 計画入力領域の有効化
                        lblOperationSecondTitle.Enabled = chkbxUse.Checked;
                        tlpOperationSecond.Enabled = chkbxUse.Checked;
                        lblPlanProductionQuantityTitle.Enabled = chkbxUse.Checked;
                        tlpPlanProductionQuantity.Enabled = chkbxUse.Checked;
                        tlpPlanStartEnd.Enabled = chkbxUse.Checked;

                        // 実績入力領域の無効化
                        btnClear.Enabled = chkbxUse.Checked;
                        lblActualProductionQuantityTitle.Enabled = false;
                        tlpEditProductQuantity.Enabled = false;
                        tlpActualWorkTime.Enabled = false;

                        // 除外時間関連処理を有効化
                        if (tlpPlanStartEnd.Enabled || tlpActualWorkTime.Enabled)
                        {
                            pnlExclusionTitle.Enabled = true;
                            tlpExclusionRowList.Enabled = true;
                        }
                        else
                        {
                            pnlExclusionTitle.Enabled = false;
                            tlpExclusionRowList.Enabled = false;
                        }
                        break;
                }
                {
                    // 計画欄の有効・無効変更
                    label4.Enabled = tlpPlanStartEnd.Enabled;
                    lblPlanStartTimeTitle.Enabled = tlpPlanStartEnd.Enabled;
                    chkbkPlanStartNextDay.Enabled = tlpPlanStartEnd.Enabled;
                    PlanStartTime.Enabled = tlpPlanStartEnd.Enabled;
                    label5.Enabled = tlpPlanStartEnd.Enabled;
                    lblPlanEndTimeTitle.Enabled = tlpPlanStartEnd.Enabled;
                    chkbkPlanEndNextDay.Enabled = tlpPlanStartEnd.Enabled;
                    PlanEndTime.Enabled = tlpPlanStartEnd.Enabled;

                    // 実績欄の有効・無効変更
                    label6.Enabled = tlpActualWorkTime.Enabled;
                    lblActualStartTimeTitle.Enabled = tlpActualWorkTime.Enabled;
                    chkbkActualStartNextDay.Enabled = tlpActualWorkTime.Enabled;
                    ActualStartTime.Enabled = tlpActualWorkTime.Enabled;
                    label7.Enabled = tlpActualWorkTime.Enabled;
                    lblActualEndTimeTitle.Enabled = tlpActualWorkTime.Enabled;
                    chkbkActualEndNextDay.Enabled = tlpActualWorkTime.Enabled;
                    ActualEndTime.Enabled = tlpActualWorkTime.Enabled;
                }
            }
        }

        /// <summary>
        /// 定時稼働時間
        /// </summary>
        public String OperationSecondProp
        {
            get { return  OperationSecond.Text; }
        }

        /// <summary>
        /// 計画生産数
        /// </summary>
        public String PlanProductionQuantityProp
        {
            get { return PlanProductionQuantity.Text; }
        }

        /// <summary>
        /// 実績時間の表示切替
        /// </summary>
        public bool TlpActualWorkTime
        {
            get { return tlpActualWorkTime.Enabled; }
        }

        /// <summary>
        /// 実績生産数編集日リスト
        /// </summary>
        public List<DateTime> DtQuantityList
        {
            get { return mDateQuantityList; }
            set { mDateQuantityList = value; }
        }

        ///// <summary>
        ///// 除外区分設定行リスト
        ///// </summary>
        //public List<UCOperating_Shift_Exclusion_Row> ExclusionRowList
        //{
        //    get
        //    {
        //        return mExclusionRowList;
        //    }
        //}

        /// <summary>
        /// 稼働シフト除外時間テーブルデータ
        /// </summary>
        public DataTable DtOperatingShiftExclusionTbl { get; set; }

        /// <summary>
        /// 除外区分設定行リスト
        /// </summary>
        public List<UCOperating_Shift_Exclusion_Row> ExclusionRowList
        {
            get
            {
                return mExclusionRowList;
            }
        }

        /// <summary>
        /// 親コントロール
        /// </summary>
        public UCOperating_Shift_Regist Owner { get; set; }
        #endregion "プロパティ"
        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 実績生産数編集ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnActualProductionQuantityClicked(object sender, EventArgs e)
        {
            try
            {
                logger.Info("稼働シフト　実績生産数編集ボタン押下");
                //mProductionQuantityEditedList.Clear();
                FrmEditProductionQuantity frm = new FrmEditProductionQuantity(TargetDate, OperationShift, mDtProductionQuantity, out bool flgResult, this);

                //if (int.TryParse(lblActualProductionQuantity.Text, out int result) && 0 < result && 0 < mPlanOperatingShift.Rows.Count && flgResult == true)
                int cycleRowCount = mDtProductionQuantity.AsEnumerable().Count(x => 1 <= x.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE));
                if (0 < cycleRowCount && 0 < mPlanOperatingShift.Rows.Count && flgResult == true)
                {
                    if (frm.ShowDialog(ParentForm) == DialogResult.OK)
                    {
                        lblActualProductionQuantity.Text = frm.ProductionQuantity.ToString();
                        //mProductionQuantityEditedList = frm.ProductionQuantityEditedList;
                        mDtProductionQuantity = frm.OperatingShiftProductionQuantity;

                        NotifyProductionQuantityChanged(new EventArgs());

                        mDateQuantityList.Add(TargetDate);

                        // 編集フラグを立てる
                        FrmMain.gIsDataChange = true;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// シフトチェックボックス チェック変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnUseStateChanged(object sender, EventArgs e)
        {
            //EnablePlan = DateTime.Today <= TargetDate;
            //EnableActual = DateTime.Today >= TargetDate;

            //tableLayoutPanel1.Enabled = tableLayoutPanel2.Enabled = tableLayoutPanel3.Enabled = chkbxUse.Checked;
            logger.Info("シフトのチェック状態を変更しました。");

            EnablePlan = chkbxUse.Checked;
            switch (mSelectDateStatus)
            {
                case SELECT_DATE_STATUS.TODAY:
                case SELECT_DATE_STATUS.FUTURE:
                    tplShiftArea.Enabled = tlpExclusionRowList.Enabled = pnlExclusionTitle.Enabled = chkbxUse.Checked;
                    // 計画欄の有効・無効変更
                    // 計画入力領域の有効化
                    lblOperationSecondTitle.Enabled = chkbxUse.Checked;
                    tlpOperationSecond.Enabled = chkbxUse.Checked;
                    lblPlanProductionQuantityTitle.Enabled = chkbxUse.Checked;
                    tlpPlanProductionQuantity.Enabled = chkbxUse.Checked;
                    tlpPlanStartEnd.Enabled = chkbxUse.Checked;

                    btnClear.Enabled = chkbxUse.Checked;
                    label4.Enabled = chkbxUse.Checked;
                    lblPlanStartTimeTitle.Enabled = chkbxUse.Checked;
                    chkbkPlanStartNextDay.Enabled = chkbxUse.Checked;
                    PlanStartTime.Enabled = chkbxUse.Checked;
                    label5.Enabled = chkbxUse.Checked;
                    lblPlanEndTimeTitle.Enabled = chkbxUse.Checked;
                    chkbkPlanEndNextDay.Enabled = chkbxUse.Checked;
                    PlanEndTime.Enabled = chkbxUse.Checked;

                    // 選択日が登録されていて、かつシフト入力にチェックが入っている場合に実績入力領域を有効化
                    if (mPlanOperatingShift.AsEnumerable().Select(x => x.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE)).Contains(mTargetDate.Date)
                        && chkbxUse.Checked)
                    {
                        // 実績入力領域の有効化
                        btnClear.Enabled = true;
                        lblActualProductionQuantityTitle.Enabled = true;

                        // 実績生産数の有無で編集ボタンと実績稼働時刻の表示を切り替え
                        if (0 < ActualProductionQuantity)
                        {
                            tlpActualWorkTime.Enabled = true;
                            tlpEditProductQuantity.Enabled = true;
                        }
                        else
                        {
                            tlpActualWorkTime.Enabled = false;
                            tlpEditProductQuantity.Enabled = false;
                        }
                    }
                    else
                    {
                        // 実績入力領域の無効化
                        btnClear.Enabled = false;
                        lblActualProductionQuantityTitle.Enabled = false;
                        tlpEditProductQuantity.Enabled = false;
                        tlpActualWorkTime.Enabled = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// 除外区分設定ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExclusionClassEditIconClicked(object sender, EventArgs e)
        {
            logger.Info("稼働シフト　除外区分設定ボタン押下");

            //mExclusionOperatingShift = mSqlOperatingShiftExclusionTbl.SelectExclusionName();

            //// 表示中の除外区分はリストに追加
            //mExclusionNameList.Clear();

            //foreach (var rowList in mExclusionRowList)
            //{
            //    if (!String.IsNullOrEmpty(rowList.cmbExclusionClass.SelectedItem as string))
            //    {
            //        mExclusionNameList.Add(rowList.cmbExclusionClass.SelectedItem.ToString());
            //    }
            //}

            FrmExclusionClassMaintenance frm = new FrmExclusionClassMaintenance(this);

            // 除外区分登録画面の表示
            if (frm.ShowDialog(ParentForm) != DialogResult.Cancel)
            {
                NotifyExclusionClassChanged(new EventArgs());
            }
        }

        /// <summary>
        /// クリアボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            logger.Info("稼働シフト　クリアボタン押下");

            // 本日以降の稼働日が選択されている場合のみクリアする
            if (DateTime.Today <= TargetDate)
            {
                if(DialogResult.Yes == MessageBox.Show(String.Format("選択中のシフト情報をクリアします。よろしいですか\n対象稼働日：{0}\n対象シフト：{1}",TargetDate.ToString("yyyy/MM/dd"), OperationShift),"",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question))
                {
                    logger.Info("Yes押下");

                    // 計画稼働シフト項目クリア
                    logger.Debug("計画稼働シフト項目をクリアします。");
                    ClearPlanOperatingShiftItem();

                    //// 実績稼働シフト設定項目クリア
                    //ClearActualOperatingShiftItem();

                    // 稼働シフト除外時間設定項目クリア  
                    logger.Debug("稼働シフト除外時間設定項目をクリアします。");
                    ClearExclusionOperatingShiftItem();

                    chkbxUse.Checked = false;

                    // 編集フラグを立てる
                    FrmMain.gIsDataChange = true;
                }
                logger.Info("No押下");
            }
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UpdateTagetDay"></param>
        /// <param name="ActualType"></param>
        /// <returns></returns>
        public Boolean RegistOperatingShift(DateTime UpdateTagetDay, int ActualType)
        {
            //SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
            //if (dbConnector != null)
            //{
            //    dbConnector.Create();
            //    dbConnector.OpenDatabase();

            //    if (ActualType == 1)
            //    {
            //        // 計画　ActialType == 1
            //        if (chkbxUse.CheckState == CheckState.Checked)
            //        {
            //            // 本日以降(未来)の日付けにのみ保存可能
            //            if (TargetDate >= DateTime.Today)
            //            {
            //                UpdateOperatingShiftTbl(dbConnector
            //                    , UpdateTagetDay, OperationShift, ActualType
            //                    , (int)PlanProductionQuantity.Value
            //                    , TimeCombining(UpdateTagetDay, PlanStartTime.Value)
            //                    , TimeCombining(UpdateTagetDay.AddDays(PlanStartTime.Value > PlanEndTime.Value ? 1 : 0), PlanEndTime.Value)
            //                    , (int)OperationSecond.Value
            //                    );

            //                for (int iCnt = 0; iCnt < ExclusionRowList.Count(); iCnt++)
            //                {
            //                    UpdateOperatingShiftExclusionTbl(dbConnector
            //                        , UpdateTagetDay, OperationShift, ActualType, iCnt
            //                        , TimeCombining(UpdateTagetDay, ExclusionRowList[iCnt].ExclusionStartTime)
            //                        , TimeCombining(UpdateTagetDay.AddDays(ExclusionRowList[iCnt].ExclusionStartTime > ExclusionRowList[iCnt].ExclusionEndTime ? 1 : 0),
            //                            ExclusionRowList[iCnt].ExclusionEndTime)
            //                        , ExclusionRowList[iCnt].ExclusionType
            //                        , ExclusionRowList[iCnt].ExclusionRemark, ExclusionRowList[iCnt].ExclusionClass);
            //                }

            //                // 直開始、直終了　タイマー処理を追加する
            //                // 直開始
            //                UpdateSpecifiedTimeProcessingTbl(dbConnector
            //                    , TimeCombining(UpdateTagetDay, PlanStartTime.Value)
            //                    , 1, 0, "", 0, UpdateTagetDay, OperationShift);
            //                // 直終了
            //                UpdateSpecifiedTimeProcessingTbl(dbConnector
            //                    , TimeCombining(UpdateTagetDay.AddDays(PlanStartTime.Value > PlanEndTime.Value ? 1 : 0), PlanEndTime.Value)
            //                    , 2, 0, "", 0, UpdateTagetDay, OperationShift);
            //            }
            //            else
            //            {
            //                // 本日以前(過去)であっても除外のみ保存可能
            //                for (int iCnt = 0; iCnt < ExclusionRowList.Count(); iCnt++)
            //                {
            //                    UpdateOperatingShiftExclusionTbl(dbConnector
            //                        , UpdateTagetDay, OperationShift, ActualType, iCnt
            //                        , TimeCombining(UpdateTagetDay, ExclusionRowList[iCnt].ExclusionStartTime)
            //                        , TimeCombining(UpdateTagetDay.AddDays(ExclusionRowList[iCnt].ExclusionStartTime > ExclusionRowList[iCnt].ExclusionEndTime ? 1 : 0),
            //                                ExclusionRowList[iCnt].ExclusionEndTime)
            //                        , ExclusionRowList[iCnt].ExclusionType
            //                        , ExclusionRowList[iCnt].ExclusionRemark, ExclusionRowList[iCnt].ExclusionClass);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            RemoveOperatingShiftTbl(dbConnector
            //                , UpdateTagetDay, OperationShift, ActualType);

            //            // 直開始、直終了　タイマー処理を削除する
            //            RemoveSpecifiedTimeProcessingTbl(dbConnector
            //                , 1, UpdateTagetDay, OperationShift);
            //            RemoveSpecifiedTimeProcessingTbl(dbConnector
            //                , 2, UpdateTagetDay, OperationShift);
            //        }
            //    }
            //    else
            //    {
            //        // 実績 ActualType == 0
            //        if (chkbxUse.CheckState == CheckState.Checked)
            //        {
            //            // 本日以前(過去)の日付けにのみ保存可能
            //            if (UpdateTagetDay <= DateTime.Today)
            //            {
            //                DataTable dtProductionQuantity;
            //                if (mDtProductionQuantityEdited == null)
            //                {
            //                    dtProductionQuantity = QueryOperatingShiftProductionQuantityTbl(
            //                                        dbConnector, TargetDate, OperationShift, 0);
            //                }
            //                else
            //                {
            //                    dtProductionQuantity = mDtProductionQuantityEdited;
            //                }
            //                int ActualProductionQuantity = dtProductionQuantity.AsEnumerable().Sum(x => x.Field<Int32>("EditProductionQuantity"));

            //                UpdateOperatingShiftTbl(dbConnector
            //                    , UpdateTagetDay, OperationShift, ActualType
            //                    , ActualProductionQuantity
            //                    , TimeCombining(UpdateTagetDay, ActualStartTime.Value)
            //                    , TimeCombining(UpdateTagetDay.AddDays(ActualStartTime.Value > ActualEndTime.Value ? 1 : 0), ActualEndTime.Value)
            //                    , (int)OperationSecond.Value
            //                    );
            //                if (mDtProductionQuantityEdited != null)
            //                {
            //                    UpdateProductionQuantityTbl(mDtProductionQuantityEdited);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            RemoveOperatingShiftTbl(dbConnector
            //                , UpdateTagetDay, OperationShift, ActualType);
            //        }
            //    }

            //    dbConnector.CloseDatabase();
            //    dbConnector.Dispose();
            //}

            return true;
        }

        /// <summary>
        /// 勤怠パターン登録イベント
        /// </summary>
        /// <param name="PatternId"></param>
        public void RegistOperatingShiftPattern(int patternId, Boolean isUpdate)
        {
            try
            {
                if (isUpdate)
                {
                    DataTable dtTempReg = mDtSqlOperatingShiftTbl.Clone();
                    DataTable dtTempExcReg = mDtOperatingShiftExclusionPatternTbl.Clone();

                    dtTempReg.Rows.Add(dtTempReg.NewRow());

                    TimeSpan tsStart = TimeSpan.Parse(String.Format("{0}:{1}", PlanStartTime.Value.Hour, PlanStartTime.Value.Minute));
                    TimeSpan tsEnd = TimeSpan.Parse(String.Format("{0}:{1}", PlanEndTime.Value.Hour, PlanEndTime.Value.Minute));

                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.PATTERN_ID] = patternId;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.OPERATION_SHIFT] = mOperationShift;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY] = PlanProductionQuantity.Value;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.START_TIME] = tsStart;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG] = chkbkPlanStartNextDay.Checked;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.END_TIME] = tsEnd;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG] = chkbkPlanEndNextDay.Checked;
                    dtTempReg.Rows[0][ColumnOperatingShiftPatternTbl.OPERATION_SECOND] = OperationSecond.Value;

                    mSqlOperatingShiftPatternTbl.Upsert(dtTempReg);

                    for(int i  =0; i< mExclusionRowList.Count; i++)
                    {
                        dtTempExcReg.Rows.Add(dtTempExcReg.NewRow());
                        TimeSpan tsExcStart = TimeSpan.Parse(String.Format("{0}:{1}", mExclusionRowList[i].ExclusionStartTime.Hour, mExclusionRowList[i].ExclusionStartTime.Minute));
                        TimeSpan tsExcEnd = TimeSpan.Parse(String.Format("{0}:{1}", mExclusionRowList[i].ExclusionEndTime.Hour, mExclusionRowList[i].ExclusionEndTime.Minute));

                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID] = patternId;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT] = mOperationShift;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX] = i;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME] = tsExcStart;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG] = mExclusionRowList[i].ExclusionStartNextDay;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME] = tsExcEnd;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG] = mExclusionRowList[i].ExclusionEndNextDay;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK] = mExclusionRowList[i].ExclusionType;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID] = mExclusionRowList[i].ExclusionClass;
                        dtTempExcReg.Rows[i][ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK] = mExclusionRowList[i].ExclusionRemark;
                    }

                    if(mSqlOperatingShiftExclusionPatternTbl.Upsert(dtTempExcReg))
                    {
                        logger.Debug("Upsert処理を実行しました。");
                    }
                    else
                    {
                        logger.Debug("Upsert処理に失敗しました。");
                    }
                }
                else
                {
                    if(mSqlOperatingShiftPatternTbl.Delete(patternId, mOperationShift)
                        && mSqlOperatingShiftExclusionPatternTbl.Delete(patternId, mOperationShift))
                    {
                        logger.Debug("Delete処理を実行しました。");
                    }
                    else
                    {
                        logger.Debug("Delete処理に失敗しました。");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
            }
        }

        /// <summary>
        /// 勤帯パターン読み込み
        /// </summary>
        /// <param name="planData">計画データ</param>
        /// <param name="actualData">実績データ</param>
        /// <param name="exclusionData">除外区分データ</param>
        public void LoadOperatingShiftPattern(int PatternId)
        {
            chkbxUse.CheckState = CheckState.Unchecked;

            PlanProductionQuantity.Value = 0;
            lblActualProductionQuantity.Text = "";
            PlanStartTime.Value = PlanEndTime.Value = ActualStartTime.Value = ActualEndTime.Value = new DateTime(2001, 1, 1, 0, 0, 0);
            //mProductionQuantityEditedList = null;

            //for (int iCnt = 0; iCnt < mExclusionRowList.Count(); iCnt++)
            //{
            //    mExclusionRowList[iCnt].ExclusionRow = null;
            //}

            if (TargetDate == DateTime.MinValue) return;

            //SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
            //if (dbConnector != null)
            //{
            //    dbConnector.Create();
            //    dbConnector.OpenDatabase();

            //    DataTable dtPlan = QueryOperatingShiftPatternTbl(dbConnector, PatternId, mOperationShift);

            //    if (dtPlan.Rows.Count > 0) chkbxUse.CheckState = CheckState.Checked;

            //    if (dtPlan.Rows.Count > 0)
            //    {
            //        DataRow drPlan = dtPlan.Rows[0];

            //        OperationSecond.Value = drPlan.Field<Int32>("OperationSecond");
            //        PlanProductionQuantity.Value = drPlan.Field<Int32>("ProductionQuantity");
            //        PlanStartTime.Value = drPlan.Field<DateTime>("StartTime");
            //        PlanEndTime.Value = drPlan.Field<DateTime>("EndTime");
            //    }

            //    DataTable dtExclusion = QueryOperatingShiftExclusionPatternTbl(dbConnector, PatternId, mOperationShift);

            //    if (dtExclusion.Rows.Count > 0)
            //    {
            //        for (int iCnt = 0; iCnt < ExclusionRowList.Count() && iCnt < dtExclusion.Rows.Count; iCnt++)
            //        {
            //            ExclusionRowList[iCnt].ExclusionRow = dtExclusion.Rows[iCnt];
            //        }
            //    }

            //    dbConnector.CloseDatabase();
            //    dbConnector.Dispose();
            //}
        }

        /// <summary>
        /// 除外区分変更通知イベント
        /// </summary>
        public void NotifyExclusionClassChanged(EventArgs args)
        {
            EventHandler handler = ExclusionClassChanged;
            if (handler != null)
            {
                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }

        /// <summary>
        /// 実績生産数変更通知イベント
        /// </summary>
        /// <param name="args"></param>
        public void NotifyProductionQuantityChanged(EventArgs args)
        {
            EventHandler handler = ProductionQuantityChanged;
            if (handler != null)
            {
                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }

        /// <summary>
        /// 除外区分更新処理
        /// </summary>
        public void UpdateExclusionClassList()
        {
            DataTable dt = mSqlExclusionMst.Select();
            string exclusionName;

            foreach (var item in mExclusionRowList)
            {
                exclusionName = item.cmbExclusionClass.Text;
                item.UpdateExclusionClassList(dt, exclusionName);
            }
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// 計画稼働シフト設定項目クリア
        /// </summary>
        private void ClearPlanOperatingShiftItem()
        {
            //mPlanOperatingShift.Rows.Clear();

            PlanProductionQuantity.Value = 0;
            PlanStartTime.Value = mTargetDate.Date;
            PlanStartTime.CustomFormat = "HH : mm";
            PlanEndTime.Value = mTargetDate.Date;
            PlanEndTime.CustomFormat = "HH : mm";
            chkbkPlanStartNextDay.Checked = false;
            chkbkPlanEndNextDay.Checked = false;
            OperationSecond.Value = 0;

            if(mPlanOperatingShift.Rows.Count > 0)
            {
                mPlanOperatingShift.Rows[0][PlanOperatingShiftTblColumn.OPERATION_SECOND] = 0;
                mPlanOperatingShift.Rows[0][PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY] = 0;
                mPlanOperatingShift.Rows[0][PlanOperatingShiftTblColumn.START_TIME] = mTargetDate.Date;
                mPlanOperatingShift.Rows[0][PlanOperatingShiftTblColumn.END_TIME] = mTargetDate.Date;
            }
        }

        /// <summary>
        /// 実績稼働シフト設定項目クリア
        /// </summary>
        private void ClearActualOperatingShiftItem()
        {
            mActualOperatingShift.Rows.Clear();

            ActualStartTime.Value = mTargetDate.Date;
            ActualEndTime.Value = mTargetDate.Date;
            chkbkActualStartNextDay.Checked = false;
            chkbkActualEndNextDay.Checked = false;
        }

        /// <summary>
        /// 稼働シフト除外時間設定項目クリア
        /// </summary>
        private void ClearExclusionOperatingShiftItem()
        {
            mExclusionOperatingShift.Rows.Clear();

            for (int i = 0; i < mExclusionRowList.Count; i++)
            {
                mExclusionRowList[i].ClearExclusionRow(mTargetDate);
            }
        }

        ///// <summary>
        ///// 使用中の除外区分をリストに追加
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void AddUseName(object sender, EventArgs e)
        //{
        //    ComboBox combo = (ComboBox)sender;

        //    // 表示中の除外区分はリストに追加
        //    if (!string.IsNullOrWhiteSpace(combo.SelectedItem as string))
        //    {
        //        if (!mExclusionNameList.Contains(combo.SelectedItem.ToString()))
        //        {
        //            mExclusionNameList.Add(combo.SelectedItem.ToString());
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseDay"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime TimeCombining(DateTime baseDay, DateTime time)
        {
            return new DateTime(
                    baseDay.Year, baseDay.Month, baseDay.Day
                    , time.Hour, time.Minute, 0);
        }

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

        #region NO_USE_SQLs
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        /// <returns></returns>
        private DataTable QueryOperatingShiftTbl(
                SqlDBConnector dbConnector, DateTime OperationDate, int OperationShift, int ActualType)
        {
            DataTable dt = new DataTable("Operating_Shift_Tbl");

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT T1.* ");
                sb.Append(" FROM Operating_Shift_Tbl T1 ");
                sb.Append(" WHERE T1.OperationDate = @OperationDate ");
                sb.Append("  AND T1.OperationShift = @OperationShift ");
                sb.Append("  AND T1.ActualType = @ActualType ");

                cmd.CommandText = sb.ToString();

                cmd.Parameters.Add("@OperationDate", SqlDbType.DateTime).Value = OperationDate;
                cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;
                cmd.Parameters.Add("@ActualType", SqlDbType.Int).Value = ActualType;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        /// <returns></returns>
        private DataTable QueryOperatingShiftExclusionTbl(
                SqlDBConnector dbConnector, DateTime OperationDate, int OperationShift, int ActualType)
        {
            DataTable dt = new DataTable("Operating_Shift_Exclusion_Tbl");

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT T1.* ");
                sb.Append(" FROM Operating_Shift_Exclusion_Tbl T1 ");
                sb.Append(" WHERE T1.OperationDate = @OperationDate ");
                sb.Append("  AND T1.OperationShift = @OperationShift ");
                sb.Append("  AND T1.ActualType = @ActualType ");
                sb.Append("  Order By T1.ExclusionIdx ");

                cmd.CommandText = sb.ToString();

                cmd.Parameters.Add("@OperationDate", SqlDbType.DateTime).Value = OperationDate;
                cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;
                cmd.Parameters.Add("@ActualType", SqlDbType.Int).Value = ActualType;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        /// <returns></returns>
        private DataTable QueryOperatingShiftProductionQuantityTbl(
                SqlDBConnector dbConnector, DateTime OperationDate, int OperationShift, int ActualType)
        {
            DataTable dt = new DataTable("Operating_Shift_ProductionQuantity_Tbl");

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT  ");
                sb.Append("  T1.ProductTypeId");
                sb.Append("  , T1.ProductTypeName");
                sb.Append("  , @OperationDate OperationDate");
                sb.Append("  , @OperationShift OperationShift");
                sb.Append("  , @ActualType ActualType");
                sb.Append("  , CASE WHEN T2.[ProductionQuantity] IS NULL THEN 0 ELSE T2.[ProductionQuantity] END [ProductionQuantity] ");
                sb.Append("  , CASE WHEN T2.[EditProductionQuantity] IS NULL THEN 0 ELSE  ");
                sb.Append("    (CASE WHEN (T2.[EditProductionQuantity] < 0) THEN 0 ELSE T2.[EditProductionQuantity] END) END [EditProductionQuantity] ");
                sb.Append("  , CASE WHEN T2.[WorkingProductionQuantity] IS NULL THEN 0 ELSE T2.[WorkingProductionQuantity] END [WorkingProductionQuantity] ");
                sb.Append(" FROM ProductType_Mst T1 ");
                sb.Append("  LEFT JOIN Operating_Shift_ProductionQuantity_Tbl T2");
                sb.Append("   ON T2.ProductTypeId = T1.ProductTypeId");
                sb.Append("   AND T2.OperationDate = @OperationDate");
                sb.Append("   AND T2.OperationShift = @OperationShift");
                sb.Append("   AND T2.ActualType = @ActualType ");
                sb.Append("  Order By T1.OrderIdx ");

                cmd.CommandText = sb.ToString();

                cmd.Parameters.Add("@OperationDate", SqlDbType.Date).Value = OperationDate;
                cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;
                cmd.Parameters.Add("@ActualType", SqlDbType.Int).Value = ActualType;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        /// <param name="ProductionQuantity"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="OperationSecond"></param>
        private void UpdateOperatingShiftTbl(SqlDBConnector dbConnector
                , DateTime OperationDate, int OperationShift, int ActualType
                , int ProductionQuantity, DateTime StartTime, DateTime EndTime, int OperationSecond)
        {
            String CommandText = "";
            StringBuilder sb = new StringBuilder();

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                sb.Clear();
                sb.Append("MERGE INTO Operating_Shift_Tbl AS T1  ");
                sb.Append("  USING ");
                sb.Append("    (SELECT ");
                sb.Append("      @OperationDate AS OperationDate ");
                sb.Append("      ,@OperationShift AS OperationShift");
                sb.Append("      ,@ActualType AS ActualType");
                sb.Append("    ) AS T2");
                sb.Append("  ON (");
                sb.Append("   T1.OperationDate = @OperationDate ");
                sb.Append("   AND T1.OperationShift = @OperationShift ");
                sb.Append("   AND T1.ActualType = @ActualType ");
                sb.Append("  )");
                sb.Append(" WHEN MATCHED THEN ");
                sb.Append("  UPDATE SET ");
                sb.Append("   ProductionQuantity = @ProductionQuantity");
                sb.Append("   ,StartTime = @StartTime");
                sb.Append("   ,EndTime = @EndTime");
                sb.Append("   ,OperationSecond = @OperationSecond");
                sb.Append(" WHEN NOT MATCHED THEN ");
                sb.Append("  INSERT (");
                sb.Append("   OperationDate");
                sb.Append("   ,OperationShift");
                sb.Append("   ,ActualType");
                sb.Append("   ,ProductionQuantity");
                sb.Append("   ,StartTime");
                sb.Append("   ,EndTime");
                sb.Append("   ,OperationSecond");
                sb.Append("   ) VALUES (");
                sb.Append("   @OperationDate ");
                sb.Append("   ,@OperationShift ");
                sb.Append("   ,@ActualType ");
                sb.Append("   ,@ProductionQuantity ");
                sb.Append("   ,@StartTime ");
                sb.Append("   ,@EndTime ");
                sb.Append("   ,@OperationSecond ");
                sb.Append("   )");
                sb.Append(";");

                cmd.CommandText = sb.ToString();

                CommandText = cmd.CommandText;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;
                cmd.Parameters.Add(new SqlParameter("@ActualType", SqlDbType.Int)).Value = ActualType;
                cmd.Parameters.Add(new SqlParameter("@ProductionQuantity", SqlDbType.Int)).Value = ProductionQuantity;
                cmd.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime)).Value = StartTime;
                cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime)).Value = EndTime;
                cmd.Parameters.Add(new SqlParameter("@OperationSecond", SqlDbType.Int)).Value = OperationSecond;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        /// <param name="ExclusionIdx"></param>
        /// <param name="ExclusionStartTime"></param>
        /// <param name="ExclusionEndTime"></param>
        /// <param name="ExclusionType"></param>
        /// <param name="ExclusionRemark"></param>
        /// <param name="ExclusionClass"></param>
        private void UpdateOperatingShiftExclusionTbl(SqlDBConnector dbConnector
                , DateTime OperationDate, int OperationShift, int ActualType, int ExclusionIdx
                , DateTime ExclusionStartTime, DateTime ExclusionEndTime, int ExclusionType
                , String ExclusionRemark, String ExclusionClass)
        {
            String CommandText = "";
            StringBuilder sb = new StringBuilder();

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                sb.Clear();
                sb.Append("MERGE INTO Operating_Shift_Exclusion_Tbl AS T1  ");
                sb.Append("  USING ");
                sb.Append("    (SELECT ");
                sb.Append("      @OperationDate AS OperationDate ");
                sb.Append("      ,@OperationShift AS OperationShift");
                sb.Append("      ,@ActualType AS ActualType");
                sb.Append("      ,@ExclusionIdx AS ExclusionIdx");
                sb.Append("    ) AS T2");
                sb.Append("  ON (");
                sb.Append("   T1.OperationDate = @OperationDate ");
                sb.Append("   AND T1.OperationShift = @OperationShift ");
                sb.Append("   AND T1.ActualType = @ActualType ");
                sb.Append("   AND T1.ExclusionIdx = @ExclusionIdx ");
                sb.Append("  )");
                sb.Append(" WHEN MATCHED THEN ");
                sb.Append("  UPDATE SET ");
                sb.Append("   ExclusionStartTime = @ExclusionStartTime");
                sb.Append("   ,ExclusionEndTime = @ExclusionEndTime");
                sb.Append("   ,ExclusionType = @ExclusionType");
                sb.Append("   ,ExclusionRemark = @ExclusionRemark");
                sb.Append("   ,ExclusionClass = @ExclusionClass");
                sb.Append(" WHEN NOT MATCHED THEN ");
                sb.Append("  INSERT (");
                sb.Append("   OperationDate");
                sb.Append("   ,OperationShift");
                sb.Append("   ,ActualType");
                sb.Append("   ,ExclusionIdx");
                sb.Append("   ,ExclusionStartTime");
                sb.Append("   ,ExclusionEndTime");
                sb.Append("   ,ExclusionType");
                sb.Append("   ,ExclusionRemark");
                sb.Append("   ,ExclusionClass");
                sb.Append("   ) VALUES (");
                sb.Append("   @OperationDate ");
                sb.Append("   ,@OperationShift ");
                sb.Append("   ,@ActualType ");
                sb.Append("   ,@ExclusionIdx ");
                sb.Append("   ,@ExclusionStartTime ");
                sb.Append("   ,@ExclusionEndTime ");
                sb.Append("   ,@ExclusionType ");
                sb.Append("   ,@ExclusionRemark ");
                sb.Append("   ,@ExclusionClass ");
                sb.Append("   )");
                sb.Append(";");

                cmd.CommandText = sb.ToString();

                CommandText = cmd.CommandText;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;
                cmd.Parameters.Add(new SqlParameter("@ActualType", SqlDbType.Int)).Value = ActualType;
                cmd.Parameters.Add(new SqlParameter("@ExclusionIdx", SqlDbType.Int)).Value = ExclusionIdx;
                cmd.Parameters.Add(new SqlParameter("@ExclusionStartTime", SqlDbType.DateTime)).Value = ExclusionStartTime;
                cmd.Parameters.Add(new SqlParameter("@ExclusionEndTime", SqlDbType.DateTime)).Value = ExclusionEndTime;
                cmd.Parameters.Add(new SqlParameter("@ExclusionType", SqlDbType.Int)).Value = ExclusionType;
                cmd.Parameters.Add(new SqlParameter("@ExclusionRemark", SqlDbType.NVarChar)).Value = ExclusionRemark;
                cmd.Parameters.Add(new SqlParameter("@ExclusionClass", SqlDbType.NVarChar)).Value = ExclusionClass;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ActualType"></param>
        private void RemoveOperatingShiftTbl(
                SqlDBConnector dbConnector
                , DateTime OperationDate, int OperationShift, int ActualType)
        {
            try
            {
                String CommandText = "";

                using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Clear();
                    sb.Append("DELETE T1 ");
                    sb.Append("  FROM Operating_Shift_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.OperationDate = @OperationDate ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");
                    sb.Append("   AND T1.ActualType = @ActualType ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                    cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;
                    cmd.Parameters.Add(new SqlParameter("@ActualType", SqlDbType.Int)).Value = ActualType;

                    cmd.ExecuteNonQuery();

                    sb.Clear();
                    sb.Append("DELETE T1 ");
                    sb.Append("  FROM Operating_Shift_Exclusion_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.OperationDate = @OperationDate ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");
                    sb.Append("   AND T1.ActualType = @ActualType ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                logger.Error("SqlException RemoveOperatingShiftTbl", ex);
            }
            catch (Exception ex)
            {
                logger.Error("Exception RemoveOperatingShiftTbl", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="PatternId"></param>
        /// <param name="OperationShift"></param>
        /// <returns></returns>
        private DataTable QueryOperatingShiftPatternTbl(
                SqlDBConnector dbConnector, int PatternId, int OperationShift)
        {
            DataTable dt = new DataTable("Operating_Shift_Tbl");

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT T1.* ");
                sb.Append(" FROM Operating_Shift_Pattern_Tbl T1 ");
                sb.Append(" WHERE T1.PatternId = @PatternId ");
                sb.Append("  AND T1.OperationShift = @OperationShift ");

                cmd.CommandText = sb.ToString();

                cmd.Parameters.Add("@PatternId", SqlDbType.Int).Value = PatternId;
                cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="PatternId"></param>
        /// <param name="OperationShift"></param>
        /// <returns></returns>
        private DataTable QueryOperatingShiftExclusionPatternTbl(
                SqlDBConnector dbConnector, int PatternId, int OperationShift)
        {
            DataTable dt = new DataTable("Operating_Shift_Exclusion_Pattern_Tbl");

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT T1.* ");
                sb.Append(" FROM Operating_Shift_Exclusion_Pattern_Tbl T1 ");
                sb.Append(" WHERE T1.PatternId = @PatternId ");
                sb.Append("  AND T1.OperationShift = @OperationShift ");
                sb.Append("  Order By T1.ExclusionIdx ");

                cmd.CommandText = sb.ToString();

                cmd.Parameters.Add("@PatternId", SqlDbType.Int).Value = PatternId;
                cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = OperationShift;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="PatternId"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ProductionQuantity"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="OperationSecond"></param>
        private void UpdateOperatingShiftPatternTbl(SqlDBConnector dbConnector
                , int PatternId, int OperationShift
                , int ProductionQuantity, DateTime StartTime, DateTime EndTime, int OperationSecond)
        {
            String CommandText = "";
            StringBuilder sb = new StringBuilder();

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                sb.Clear();
                sb.Append("MERGE INTO Operating_Shift_Pattern_Tbl AS T1  ");
                sb.Append("  USING ");
                sb.Append("    (SELECT ");
                sb.Append("      @PatternId AS PatternId ");
                sb.Append("      ,@OperationShift AS OperationShift");
                sb.Append("    ) AS T2");
                sb.Append("  ON (");
                sb.Append("   T1.PatternId = @PatternId ");
                sb.Append("   AND T1.OperationShift = @OperationShift ");
                sb.Append("  )");
                sb.Append(" WHEN MATCHED THEN ");
                sb.Append("  UPDATE SET ");
                sb.Append("   ProductionQuantity = @ProductionQuantity");
                sb.Append("   ,StartTime = @StartTime");
                sb.Append("   ,EndTime = @EndTime");
                sb.Append("   ,OperationSecond = @OperationSecond");
                sb.Append(" WHEN NOT MATCHED THEN ");
                sb.Append("  INSERT (");
                sb.Append("   PatternId");
                sb.Append("   ,OperationShift");
                sb.Append("   ,ProductionQuantity");
                sb.Append("   ,StartTime");
                sb.Append("   ,EndTime");
                sb.Append("   ,OperationSecond");
                sb.Append("   ) VALUES (");
                sb.Append("   @PatternId ");
                sb.Append("   ,@OperationShift ");
                sb.Append("   ,@ProductionQuantity ");
                sb.Append("   ,@StartTime ");
                sb.Append("   ,@EndTime ");
                sb.Append("   ,@OperationSecond ");
                sb.Append("   )");
                sb.Append(";");

                cmd.CommandText = sb.ToString();

                CommandText = cmd.CommandText;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@PatternId", SqlDbType.Int)).Value = PatternId;
                cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;
                cmd.Parameters.Add(new SqlParameter("@ProductionQuantity", SqlDbType.Int)).Value = ProductionQuantity;
                cmd.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime)).Value = StartTime;
                cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime)).Value = EndTime;
                cmd.Parameters.Add(new SqlParameter("@OperationSecond", SqlDbType.Int)).Value = OperationSecond;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="PatternId"></param>
        /// <param name="OperationShift"></param>
        /// <param name="ExclusionIdx"></param>
        /// <param name="ExclusionStartTime"></param>
        /// <param name="ExclusionEndTime"></param>
        /// <param name="ExclusionType"></param>
        /// <param name="ExclusionRemark"></param>
        /// <param name="ExclusionClass"></param>
        private void UpdateOperatingShiftExclusionPatternTbl(SqlDBConnector dbConnector
                , int PatternId, int OperationShift, int ExclusionIdx
                , DateTime ExclusionStartTime, DateTime ExclusionEndTime, int ExclusionType
                , String ExclusionRemark, String ExclusionClass)
        {
            String CommandText = "";
            StringBuilder sb = new StringBuilder();

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                sb.Clear();
                sb.Append("MERGE INTO Operating_Shift_Exclusion_Pattern_Tbl AS T1  ");
                sb.Append("  USING ");
                sb.Append("    (SELECT ");
                sb.Append("      @PatternId AS PatternId ");
                sb.Append("      ,@OperationShift AS OperationShift");
                sb.Append("      ,@ExclusionIdx AS ExclusionIdx");
                sb.Append("    ) AS T2");
                sb.Append("  ON (");
                sb.Append("   T1.PatternId = @PatternId ");
                sb.Append("   AND T1.OperationShift = @OperationShift ");
                sb.Append("   AND T1.ExclusionIdx = @ExclusionIdx ");
                sb.Append("  )");
                sb.Append(" WHEN MATCHED THEN ");
                sb.Append("  UPDATE SET ");
                sb.Append("   ExclusionStartTime = @ExclusionStartTime");
                sb.Append("   ,ExclusionEndTime = @ExclusionEndTime");
                sb.Append("   ,ExclusionType = @ExclusionType");
                sb.Append("   ,ExclusionRemark = @ExclusionRemark");
                sb.Append("   ,ExclusionClass = @ExclusionClass");
                sb.Append(" WHEN NOT MATCHED THEN ");
                sb.Append("  INSERT (");
                sb.Append("   PatternId");
                sb.Append("   ,OperationShift");
                sb.Append("   ,ExclusionIdx");
                sb.Append("   ,ExclusionStartTime");
                sb.Append("   ,ExclusionEndTime");
                sb.Append("   ,ExclusionType");
                sb.Append("   ,ExclusionRemark");
                sb.Append("   ,ExclusionClass");
                sb.Append("   ) VALUES (");
                sb.Append("   @PatternId ");
                sb.Append("   ,@OperationShift ");
                sb.Append("   ,@ExclusionIdx ");
                sb.Append("   ,@ExclusionStartTime ");
                sb.Append("   ,@ExclusionEndTime ");
                sb.Append("   ,@ExclusionType ");
                sb.Append("   ,@ExclusionRemark ");
                sb.Append("   ,@ExclusionClass ");
                sb.Append("   )");
                sb.Append(";");

                cmd.CommandText = sb.ToString();

                CommandText = cmd.CommandText;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@PatternId", SqlDbType.Int)).Value = PatternId;
                cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;
                cmd.Parameters.Add(new SqlParameter("@ExclusionIdx", SqlDbType.Int)).Value = ExclusionIdx;
                cmd.Parameters.Add(new SqlParameter("@ExclusionStartTime", SqlDbType.DateTime)).Value = ExclusionStartTime;
                cmd.Parameters.Add(new SqlParameter("@ExclusionEndTime", SqlDbType.DateTime)).Value = ExclusionEndTime;
                cmd.Parameters.Add(new SqlParameter("@ExclusionType", SqlDbType.Int)).Value = ExclusionType;
                cmd.Parameters.Add(new SqlParameter("@ExclusionRemark", SqlDbType.NVarChar)).Value = ExclusionRemark;
                cmd.Parameters.Add(new SqlParameter("@ExclusionClass", SqlDbType.NVarChar)).Value = ExclusionClass;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="PatternId"></param>
        /// <param name="OperationShift"></param>
        private void RemoveOperatingShiftPatternTbl(
                SqlDBConnector dbConnector
                , int PatternId, int OperationShift)
        {
            try
            {
                String CommandText = "";

                using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Clear();
                    sb.Append("DELETE T1 ");
                    sb.Append("  FROM Operating_Shift_Pattern_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.PatternId = @PatternId ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@PatternId", SqlDbType.Int)).Value = PatternId;
                    cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;

                    cmd.ExecuteNonQuery();

                    sb.Clear();
                    sb.Append("DELETE T1 ");
                    sb.Append("  FROM Operating_Shift_Exclusion_Pattern_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.PatternId = @PatternId ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                logger.Error("SqlException RemoveOperatingShiftPatternTbl", ex);
            }
            catch (Exception ex)
            {
                logger.Error("Exception RemoveOperatingShiftPatternTbl", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        private void UpdateProductionQuantityTbl(
                DataTable dt)
        {
            try
            {
                SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
                if (dbConnector != null)
                {
                    dbConnector.Create();
                    dbConnector.OpenDatabase();
                    String CommandText = "";
                    StringBuilder sb = new StringBuilder();

                    using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                    {
                        foreach (DataRow rowItem in dt.Rows)
                        {
                            sb.Clear();
                            sb.Append("MERGE INTO Operating_Shift_ProductionQuantity_Tbl AS T1  ");
                            sb.Append("  USING ");
                            sb.Append("    (SELECT ");
                            sb.Append("      @OperationDate AS OperationDate ");
                            sb.Append("      ,@OperationShift AS OperationShift ");
                            sb.Append("      ,@ActualType AS ActualType ");
                            sb.Append("      ,@ProductTypeId AS ProductTypeId ");
                            sb.Append("    ) AS T2");
                            sb.Append("  ON (");
                            sb.Append("   T1.OperationDate = T2.OperationDate ");
                            sb.Append("   AND T1.OperationShift = T2.OperationShift ");
                            sb.Append("   AND T1.ActualType = T2.ActualType ");
                            sb.Append("   AND T1.ProductTypeId = T2.ProductTypeId ");
                            sb.Append("  )");
                            sb.Append(" WHEN MATCHED THEN ");
                            sb.Append("  UPDATE SET ");
                            sb.Append("   ProductionQuantity = @ProductionQuantity");
                            sb.Append("   ,EditProductionQuantity = @EditProductionQuantity");
                            sb.Append("   ,WorkingProductionQuantity = @WorkingProductionQuantity");
                            sb.Append(" WHEN NOT MATCHED THEN ");
                            sb.Append("  INSERT (");
                            sb.Append("   OperationDate");
                            sb.Append("   ,OperationShift");
                            sb.Append("   ,ActualType");
                            sb.Append("   ,ProductTypeId");
                            sb.Append("   ,ProductionQuantity");
                            sb.Append("   ,EditProductionQuantity");
                            sb.Append("   ,WorkingProductionQuantity");
                            sb.Append("   ) VALUES (");
                            sb.Append("   @OperationDate ");
                            sb.Append("   ,@OperationShift ");
                            sb.Append("   ,@ActualType ");
                            sb.Append("   ,@ProductTypeId ");
                            sb.Append("   ,@ProductionQuantity ");
                            sb.Append("   ,@EditProductionQuantity ");
                            sb.Append("   ,@WorkingProductionQuantity ");
                            sb.Append("   )");
                            sb.Append(";");

                            cmd.CommandText = sb.ToString();

                            CommandText = cmd.CommandText;

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@OperationDate", SqlDbType.Date).Value = rowItem.Field<DateTime>("OperationDate");
                            cmd.Parameters.Add("@OperationShift", SqlDbType.Int).Value = rowItem.Field<Int32>("OperationShift");
                            cmd.Parameters.Add("@ActualType", SqlDbType.Int).Value = rowItem.Field<Int32>("ActualType");
                            cmd.Parameters.Add("@ProductTypeId", SqlDbType.UniqueIdentifier).Value = rowItem.Field<Guid>("ProductTypeId");
                            cmd.Parameters.Add("@ProductionQuantity", SqlDbType.Int).Value = rowItem.Field<Int32>("ProductionQuantity");
                            cmd.Parameters.Add("@EditProductionQuantity", SqlDbType.Int).Value = rowItem.Field<Int32>("EditProductionQuantity");
                            cmd.Parameters.Add("@WorkingProductionQuantity", SqlDbType.Int).Value = rowItem.Field<Int32>("WorkingProductionQuantity");

                            cmd.ExecuteNonQuery();
                        }
                    }
                    dbConnector.CloseDatabase();
                    dbConnector.Dispose();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// RemoveSpecifiedTimeProcessingTbl
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="ProcessingType"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        private DataTable QuerySpecifiedTimeProcessingTbl(
                SqlDBConnector dbConnector
                , int ProcessingType
                , DateTime OperationDate, int OperationShift)
        {
            DataTable dt = new DataTable("SpecifiedTimeProcessing_Tbl");
            try
            {
                String CommandText = "";

                using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Clear();
                    sb.Append("SELECT ");
                    sb.Append(" T1.* ");
                    sb.Append("FROM SpecifiedTimeProcessing_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.ProcessingType = @ProcessingType ");
                    sb.Append("   AND T1.OperationDate = @OperationDate ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@ProcessingType", SqlDbType.Int)).Value = ProcessingType;
                    cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                    cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                logger.Error("SqlException RemoveSpecifiedTimeProcessingTbl", ex);
            }
            catch (Exception ex)
            {
                logger.Error("Exception RemoveSpecifiedTimeProcessingTbl", ex);
            }

            return dt;
        }

        /// <summary>
        /// RemoveSpecifiedTimeProcessingTbl
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="ProcessingType"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        private void RemoveSpecifiedTimeProcessingTbl(
                SqlDBConnector dbConnector
                , int ProcessingType
                , DateTime OperationDate, int OperationShift)
        {
            try
            {
                String CommandText = "";

                using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Clear();
                    sb.Append("DELETE T1 ");
                    sb.Append("  FROM SpecifiedTimeProcessing_Tbl T1 ");
                    sb.Append(" WHERE ");
                    sb.Append("   T1.ProcessingType = @ProcessingType ");
                    sb.Append("   AND T1.OperationDate = @OperationDate ");
                    sb.Append("   AND T1.OperationShift = @OperationShift ");

                    cmd.CommandText = sb.ToString();

                    CommandText = cmd.CommandText;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@ProcessingType", SqlDbType.Int)).Value = ProcessingType;
                    cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                    cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                logger.Error("SqlException RemoveSpecifiedTimeProcessingTbl", ex);
            }
            catch (Exception ex)
            {
                logger.Error("Exception RemoveSpecifiedTimeProcessingTbl", ex);
            }
        }


        /// <summary>
        /// UpdateSpecifiedTimeProcessingTbl
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="SpecifiedTime"></param>
        /// <param name="ProcessingType"></param>
        /// <param name="ProcessingParamInt1"></param>
        /// <param name="ProcessingParamStr1"></param>
        /// <param name="ProcessingState"></param>
        /// <param name="OperationDate"></param>
        /// <param name="OperationShift"></param>
        private void UpdateSpecifiedTimeProcessingTbl(SqlDBConnector dbConnector
                , DateTime SpecifiedTime, int ProcessingType
                , int ProcessingParamInt1, String ProcessingParamStr1
                , int ProcessingState
                , DateTime OperationDate, int OperationShift
                )
        {
            String CommandText = "";
            StringBuilder sb = new StringBuilder();

            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
            {
                sb.Clear();
                sb.Append("MERGE INTO SpecifiedTimeProcessing_Tbl AS T1  ");
                sb.Append("  USING ");
                sb.Append("    (SELECT ");
                sb.Append("      @OperationDate AS OperationDate ");
                sb.Append("      ,@OperationShift AS OperationShift");
                sb.Append("      ,@ProcessingType AS ProcessingType");
                sb.Append("    ) AS T2");
                sb.Append("  ON (");
                sb.Append("   T1.OperationDate = @OperationDate ");
                sb.Append("   AND T1.OperationShift = @OperationShift ");
                sb.Append("   AND T1.ProcessingType = @ProcessingType ");
                sb.Append("  )");
                sb.Append(" WHEN MATCHED THEN ");
                sb.Append("  UPDATE SET ");
                sb.Append("   SpecifiedTime = @SpecifiedTime");
                sb.Append("   ,ProcessingParamInt1 = @ProcessingParamInt1");
                sb.Append("   ,ProcessingParamStr1 = @ProcessingParamStr1");
                sb.Append("   ,ProcessingState = @ProcessingState");
                sb.Append(" WHEN NOT MATCHED THEN ");
                sb.Append("  INSERT (");
                sb.Append("   SpecifiedTime");
                sb.Append("   ,ProcessingType");
                sb.Append("   ,ProcessingParamInt1");
                sb.Append("   ,ProcessingParamStr1");
                sb.Append("   ,ProcessingState");
                sb.Append("   ,OperationDate");
                sb.Append("   ,OperationShift");
                sb.Append("   ) VALUES (");
                sb.Append("   @SpecifiedTime ");
                sb.Append("   ,@ProcessingType ");
                sb.Append("   ,@ProcessingParamInt1 ");
                sb.Append("   ,@ProcessingParamStr1 ");
                sb.Append("   ,@ProcessingState ");
                sb.Append("   ,@OperationDate ");
                sb.Append("   ,@OperationShift ");
                sb.Append("   )");
                sb.Append(";");

                cmd.CommandText = sb.ToString();

                CommandText = cmd.CommandText;

                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@SpecifiedTime", SqlDbType.DateTime)).Value = SpecifiedTime;
                cmd.Parameters.Add(new SqlParameter("@ProcessingType", SqlDbType.Int)).Value = ProcessingType;
                cmd.Parameters.Add(new SqlParameter("@ProcessingParamInt1", SqlDbType.Int)).Value = ProcessingParamInt1;
                cmd.Parameters.Add(new SqlParameter("@ProcessingParamStr1", SqlDbType.NVarChar)).Value = ProcessingParamStr1;
                cmd.Parameters.Add(new SqlParameter("@ProcessingState", SqlDbType.Int)).Value = ProcessingState;
                cmd.Parameters.Add(new SqlParameter("@OperationDate", SqlDbType.Date)).Value = OperationDate;
                cmd.Parameters.Add(new SqlParameter("@OperationShift", SqlDbType.Int)).Value = OperationShift;

                cmd.ExecuteNonQuery();
            }
        }
        #endregion NO_USE_SQLs

        #endregion "プライベートメソッド"
    }
}
