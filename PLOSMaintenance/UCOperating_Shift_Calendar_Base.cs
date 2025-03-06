using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLOSMaintenance
{
    public partial class UCOperating_Shift_Calendar_Base : UserControl
    {
        /// <summary>
        /// 選択対象外期間
        /// </summary>
        public class NonSelectablePeriodRange
        {
            public DateTime StartDay { get; set; }
            public DateTime EndDay { get; set; }
        }

        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// 選択月
        /// </summary>
        protected DateTime mTargetMonth;

        /// <summary>
        /// カレンダー開始日
        /// </summary>
        protected DateTime mCalendarStartDate;

        /// <summary>
        /// フォント（日付）
        /// </summary>
        protected Font mDaysFont;

        /// <summary>
        /// 背景色（選択日付）
        /// </summary>
        protected Color mSelectDaysBackColor;

        /// <summary>
        /// 日付リスト
        /// </summary>
        protected List<UCOperating_Shift_Calendar_Day_Base> ucDayList;

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
        /// シフトカレンダーベース
        /// </summary>
        public UCOperating_Shift_Calendar_Base()
        {
            InitializeComponent();

            mTargetMonth = DateTime.Today;
            mCalendarStartDate = DateTime.Today;
            mDaysFont = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            mSelectDaysBackColor = Color.LightBlue;
            ucDayList = new List<UCOperating_Shift_Calendar_Day_Base>();

            for (int iCnt = 0; iCnt < 42; iCnt++)
            {
                UCOperating_Shift_Calendar_Day_Base ucDay = CreateDayControl();

                ucDay.DaysBackColor = System.Drawing.SystemColors.Control;
                ucDay.DaysFont = DaysFont;
                ucDay.DaysForeColor = System.Drawing.SystemColors.ControlText;
                ucDay.Dock = System.Windows.Forms.DockStyle.Fill;
                ucDay.Name = $"ucDay{iCnt:00}";
                ucDay.TabIndex = iCnt;
                ucDay.SelectChanging += UcDay_SelectChanging;
                ucDay.SelectChanged += OnCalendarSelectChanged;

                tableLayoutPanel1.Controls.Add(ucDay, iCnt % 7, (iCnt / 7) + 1);

                ucDayList.Add(ucDay);
            }
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 表示月
        /// </summary>
        [Category("Calendar")]
        [Description("Calendar TargetDate")]
        [DefaultValue("")]
        public DateTime TargetMonth
        {
            get
            {
                return mTargetMonth;
            }
            set
            {
                mTargetMonth = new DateTime(value.Year, value.Month, 1);
                lblYearMonth.Text = $"{mTargetMonth.Year}/{mTargetMonth.Month}月";
                int Offset = ((int)mTargetMonth.DayOfWeek + 6) % 7;

                mCalendarStartDate = mTargetMonth.AddDays(-Offset);
                DateTime pointDay = mCalendarStartDate;
                Boolean SkipF = false;

                foreach (var item in ucDayList.Select((x, index) => new { uc = x, Index = index }))
                {
                    item.uc.CalenderTargetMonth = mTargetMonth;
                    item.uc.Selected = false;
                    if (SkipF || (pointDay.DayOfWeek == DayOfWeek.Monday && mTargetMonth.AddMonths(1) <= pointDay)
                        || (item.Index >= 38 && UseGuide))
                    {
                        SkipF = true;   // もうスキップ

                        item.uc.TargetDate = DateTime.MinValue;
                        item.uc.Selectable = false;
                        item.uc.Visible = false;
                    }
                    else
                    {
                        item.uc.TargetDate = pointDay;
                        item.uc.Selectable = !NonSelectablePeriod.Any(x => x.StartDay <= pointDay && pointDay <= x.EndDay);
                        item.uc.Visible = true;
                        if (WholePeriodSelectDaysList != null)
                        {
                            item.uc.Selected = WholePeriodSelectDaysList.Contains(pointDay);
                        }
                        else
                        { }
                    }
                    pointDay = pointDay.AddDays(1);
                }

                NotifyCalendarChanged(new PLOSMaintenance.Events.CalendarEventArgs(mTargetMonth, false, false));
            }
        }

        /// <summary>
        /// フォント（日付）
        /// </summary>
        [Category("Calendar")]
        [Description("Calendar Day Font")]
        public Font DaysFont
        {
            get { return mDaysFont; }
            set
            {
                mDaysFont = value;
                foreach (var uc in ucDayList)
                {
                    uc.DaysFont = value;
                }
            }
        }

        /// <summary>
        /// 多数選択有無
        /// </summary>
        public Boolean MultiSelect { get; set; } = false;

        /// <summary>
        /// 凡例表示有無
        /// </summary>
        public Boolean UseGuide { get; set; } = false;

        /// <summary>
        /// 背景色（選択日付）
        /// </summary>
        public Color SelectDaysBackColor
        {
            get { return mSelectDaysBackColor; }
            set
            {
                mSelectDaysBackColor = value;
                foreach (var uc in ucDayList)
                {
                    uc.SelectedColor = value;
                }
            }
        }

        /// <summary>
        /// 範囲選択 開始日
        /// </summary>
        public DateTime MultiSelectFromDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 日付リスト
        /// </summary>
        public List<DateTime> DaysList
        {
            get
            {
                return ucDayList.Select(x => x.TargetDate).ToList();
            }
        }

        /// <summary>
        /// 選択日付リスト(表示中のカレンダー分)
        /// </summary>
        public List<DateTime> SelectedDaysList
        {
            get
            {
                return ucDayList.Where(x => x.Selected).Select(x => x.TargetDate).ToList();
            }
            set
            {
                ucDayList.Where(x => x.Selected).ToList().ForEach(x => { x.Selected = false; });

                (
                from uc in ucDayList
                join day in value
                    on uc.TargetDate equals day
                select uc
                ).ToList().ForEach(x =>
                {
                    x.Selected = x.Selectable;
                    NotifyCalendarChanged(new PLOSMaintenance.Events.CalendarEventArgs(x.TargetDate, x.Selected, false));
                });
            }
        }

        /// <summary>
        /// 選択対象外期間リスト
        /// </summary>
        public List<NonSelectablePeriodRange> NonSelectablePeriod { get; } = new List<NonSelectablePeriodRange>();

        /// <summary>
        /// 選択日付リスト（全期間）
        /// </summary>
        public List<DateTime> WholePeriodSelectDaysList { get; set; } = new List<DateTime>();
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更する前に発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcDay_SelectChanging(object sender, Events.CalendarEventArgs e)
        {
            if (SelectChanging != null)
            {
                SelectChanging.Invoke(sender, e);
            }
        }

        /// <summary>
        /// カレンダーの日付が変更されるたび、日付を変更した後に発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalendarSelectChanged(object sender, Events.CalendarEventArgs e)
        {
            ChangeCalendarSelect(e);
        }

        /// <summary>
        /// 翌月ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            ChangeCalendarNextMonth();
        }

        /// <summary>
        /// 前月ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            ChangeCalendarPrevMonth();
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 日のコントロール作成処理
        /// </summary>
        /// <returns></returns>
        protected virtual UCOperating_Shift_Calendar_Day_Base CreateDayControl()
        {
            return new PLOSMaintenance.UCOperating_Shift_Calendar_Day_Base();
        }

        /// <summary>
        /// 選択日変更処理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ChangeCalendarSelect(Events.CalendarEventArgs e)
        {
            if (MultiSelect)
            {
                if (e.Selected && e.PressedShiftKey && MultiSelectFromDate == DateTime.MinValue)
                {
                    // 範囲選択開始日を更新
                    MultiSelectFromDate = e.TargetDate;
                }
                else if (e.Selected && e.PressedShiftKey && MultiSelectFromDate != DateTime.MinValue && MultiSelectFromDate != e.TargetDate)
                {
                    // 範囲選択更新
                    List<DateTime> lastTimeSelected = SelectedDaysList;
                    DateTime from = MultiSelectFromDate < e.TargetDate ? MultiSelectFromDate : e.TargetDate;
                    DateTime to = MultiSelectFromDate < e.TargetDate ? e.TargetDate : MultiSelectFromDate;

                    lastTimeSelected.AddRange(DaysList.Where(x => from <= x.Date && x.Date <= to));
                    SelectedDaysList = lastTimeSelected;

                    int span = (to - from).Days;
                    for (int i = 0; i <= span; i++)
                    {
                        DateTime target = from.AddDays(i);
                        if (!WholePeriodSelectDaysList.Contains(target))
                        {
                            WholePeriodSelectDaysList.Add(target);
                        }
                    }

                    // 範囲選択開始日をリセット
                    MultiSelectFromDate = DateTime.MinValue;
                }
                else
                {
                    if (e.Selected && !WholePeriodSelectDaysList.Contains(e.TargetDate))
                    {
                        WholePeriodSelectDaysList.Add(e.TargetDate);
                    }
                    else if (!e.Selected && WholePeriodSelectDaysList.Contains(e.TargetDate))
                    {
                        WholePeriodSelectDaysList.Remove(e.TargetDate);
                    }

                    // 範囲選択開始日をリセット
                    MultiSelectFromDate = DateTime.MinValue;
                }
            }
            else
            {
                ucDayList.Where(x => x.TargetDate != e.TargetDate
                    && x.Selected).ToList().ForEach(x => { x.Selected = false; });
            }

            NotifyCalendarChanged(e);
        }

        /// <summary>
        /// 日毎状態の更新処理
        /// </summary>
        public virtual void RefreshCalendar()
        {
            ucDayList.ToList().ForEach(x =>
            {
                x.UpdateDailyStatus();
                x.ChangeColor();
            });
        }

        /// <summary>
        /// 表示月変更処理(翌月)
        /// </summary>
        protected virtual void ChangeCalendarNextMonth()
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            today = today.AddMonths(11);
            if (TargetMonth < today)
            {
                TargetMonth = TargetMonth.AddMonths(1);
            }
        }

        /// <summary>
        /// 表示月変更処理(前月)
        /// </summary>
        protected virtual void ChangeCalendarPrevMonth()
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            today = today.AddMonths(-11);
            if (TargetMonth > today)
            {
                TargetMonth = TargetMonth.AddMonths(-1);
            }
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
