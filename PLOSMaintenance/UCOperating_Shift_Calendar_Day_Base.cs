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

namespace PLOSMaintenance
{
	public partial class UCOperating_Shift_Calendar_Day_Base : UserControl
	{
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// 選択日
        /// </summary>
        protected DateTime mTargetDate;

        /// <summary>
        /// 選択状態
        /// </summary>
        protected Boolean mSelected;

        /// <summary>
        /// 背景色（日付）
        /// </summary>
        protected Color mDaysBackColor;

        /// <summary>
        /// 文字色（日付）
        /// </summary>
        protected Color mDaysForeColor;

        /// <summary>
        /// フォント（日付）
        /// </summary>
        protected Font mDayFontBase;

        /// <summary>
        /// 計画稼働シフトテーブルアクセス
        /// </summary>
        protected SqlPlanOperatingShiftTbl mSqlPlanOperatingShiftTbl;

        /// <summary>
        /// 稼働シフト品番タイプ毎生産数テーブルアクセス
        /// </summary>
        protected SqlOperatingShiftProductionQuantityTbl mSqlOperatingShiftProductionQuantityTbl;

        /// <summary>
        /// 顔アイコン表示フラグ
        /// </summary>
        protected bool faceIconFlg;

        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更する前に発生します。
        /// </summary>
        public event EventHandler<Events.CalendarEventArgs> SelectChanging;

        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更した後に発生します。
        /// </summary>
        public event EventHandler<Events.CalendarEventArgs> SelectChanged;
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        /// <summary>
        /// シフトカレンダー（日付）ベース
        /// </summary>
        public UCOperating_Shift_Calendar_Day_Base()
        {
            InitializeComponent();

            mTargetDate = DateTime.Today;
            mSelected = false;
            mDaysBackColor = SystemColors.Control;
            mDaysForeColor = Color.Black;
            mSqlPlanOperatingShiftTbl = new SqlPlanOperatingShiftTbl(Properties.Settings.Default.ConnectionString_New);
            mSqlOperatingShiftProductionQuantityTbl = new SqlOperatingShiftProductionQuantityTbl(Properties.Settings.Default.ConnectionString_New);
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        ///  選択日付
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
                mTargetDate = value;
                ExistActual = ExistPlan = false;

                lblDay.Text = mTargetDate != DateTime.MinValue ? mTargetDate.Day.ToString() : "";
                DaysFont = mDayFontBase;
                UpdateDailyStatus();
                ChangeColor();
            }
        }

        /// <summary>
        /// 表示月
        /// </summary>
        public DateTime CalenderTargetMonth { get; set; }

        /// <summary>
        /// フォント（日付）
        /// </summary>
        [Category("Calendar_Day")]
        [Description("Calendar_Day Font")]
        public Font DaysFont
        {
            get
            {
                return mDayFontBase;
            }
            set
            {
                mDayFontBase = value;
                if (mTargetDate == DateTime.Today)
                {
                    lblDay.Font = new System.Drawing.Font(
                                value.FontFamily,
                                value.Size,
                                System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline,
                                value.Unit,
                                value.GdiCharSet);
                }
                else
                {
                    lblDay.Font = mDayFontBase;
                }
            }
        }

        /// <summary>
        /// 文字色（日付）
        /// </summary>
        [Category("Calendar_Day")]
        [Description("Calendar_Day ForeColor")]
        public Color DaysForeColor
        {
            get
            {
                return mDaysForeColor;

            }
            set
            {
                lblDay.ForeColor = value;
                mDaysForeColor = value;
            }
        }

        /// <summary>
        /// 背景色（日付）
        /// </summary>
        [Category("Calendar_Day")]
        [Description("Calendar_Day BackColor")]
        public Color DaysBackColor
        {
            get
            {
                return mDaysBackColor;
            }
            set
            {
                pnlMain.BackColor = value;
                mDaysBackColor = value;
            }
        }

        /// <summary>
        /// 計画登録/未登録
        /// </summary>
        public Boolean ExistPlan { get; set; } = false;

        /// <summary>
        /// 実績登録/未登録
        /// </summary>
        public Boolean ExistActual { get; set; } = false;

        /// <summary>
        /// サンプル
        /// </summary>
        public Boolean IsSample { get; set; } = false;
        
        /// <summary>
        /// 実生産量
        /// </summary>
        public int ActualProductionQuantity { get; set; }

        /// <summary>
        /// 生産計画リスト
        /// </summary>
        public List<int[]> PlanProductionList { get; set; } = new List<int[]>();

        /// <summary>
        /// 同月確認
        /// </summary>
        public Boolean SameMonth { get { return TargetDate.Month == CalenderTargetMonth.Month; } }

        /// <summary>
        /// 日付選択
        /// </summary>
        public Boolean Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                mSelected = value;
                ChangeColor();
            }
        }

        /// <summary>
        /// 背景色（選択日付）
        /// </summary>
        public Color SelectedColor { get; set; } = Color.LightBlue;

        /// <summary>
        /// 選択可能
        /// </summary>
        public Boolean Selectable { get; set; } = true;
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 日付パネルクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPanelClicked(object sender, EventArgs e)
        {
            if (!Selectable) return;

            Events.CalendarEventArgs args = new Events.CalendarEventArgs(TargetDate, Selected, (Control.ModifierKeys & Keys.Shift) == Keys.Shift);
            NotifyCalendarChanging(args);

            if (args.Cancel)
            {
                return;
            }

            Selected = !Selected;

            NotifyCalendarChanged(new PLOSMaintenance.Events.CalendarEventArgs(
                    TargetDate, Selected,
                    ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)));
        }

        /// <summary>
        /// 日付パネルリサイズイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPanelResized(object sender, EventArgs e)
        {
            lblDay.Size = new Size(lblDay.Size.Width, pnlMain.Size.Height * 45 / 100);
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 日毎状態の更新処理
        /// </summary>
        public virtual void UpdateDailyStatus()
        {
            ExistPlan = false;
            ExistActual = false;
            
            PlanProductionList.Clear();
            ActualProductionQuantity = 0;

            if (mTargetDate != DateTime.MinValue)
            {
                var dtPlan = mSqlPlanOperatingShiftTbl.Select(mTargetDate);
                var dtActual = mSqlOperatingShiftProductionQuantityTbl.Select(mTargetDate);
                var dtActual1 = mSqlOperatingShiftProductionQuantityTbl.Select(mTargetDate, 1);
                var dtActual2 = mSqlOperatingShiftProductionQuantityTbl.Select(mTargetDate, 2);
                var dtActual3 = mSqlOperatingShiftProductionQuantityTbl.Select(mTargetDate, 3);

                faceIconFlg = false;
                int? actualQuantity = null;

                ExistPlan = 0 < dtPlan.Rows.Count;
                ExistActual = (0 < dtActual1.Rows.Count) || (0 < dtActual2.Rows.Count) || (0 < dtActual3.Rows.Count);

                PlanProductionList = dtPlan.AsEnumerable().Select(x =>
                            new int[] { x.Field<Int32>(PlanOperatingShiftTblColumn.OPERATION_SHIFT), x.Field<Int32>(PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY) }).ToList();
                
                foreach (DataRow row in dtActual.Rows)
                {
                    actualQuantity = row.Field<int?>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);
                    if(actualQuantity != null)
                    {
                        faceIconFlg = true;
                        break;
                    }
                }

                ActualProductionQuantity = dtActual.AsEnumerable().Sum(x => x.Field<Int32>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE));
            }
        }

        /// <summary>
        /// 背景色変更処理
        /// </summary>
        public virtual void ChangeColor()
        {
            pnlMain.BackColor = Selected ? SelectedColor : DaysBackColor;
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更する前に発生します。
        /// </summary>
        private void NotifyCalendarChanging(Events.CalendarEventArgs args)
        {
            SelectChanging.Invoke(this, args);
        }

        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更した後に発生します。
        /// </summary>
        private void NotifyCalendarChanged(Events.CalendarEventArgs args)
        {
            EventHandler<Events.CalendarEventArgs> handler = SelectChanged;
            if (handler != null)
            {
                foreach (EventHandler<Events.CalendarEventArgs> evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }
        #endregion "プライベートメソッド"
    }
}
