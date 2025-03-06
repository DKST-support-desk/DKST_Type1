using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace PLOSMaintenance
{
	public partial class UCOperating_Shift_Calendar_Main : PLOSMaintenance.UCOperating_Shift_Calendar_Base
	{
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// 翌月表示イベントハンドラ
        /// </summary>
        public event EventHandler NextManthChanged;

        /// <summary>
        /// 前月表示イベントハンドラ
        /// </summary>
        public event EventHandler PrevMonthChanged;

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        public UCOperating_Shift_Calendar_Main()
        {
            UseGuide = true;
            InitializeComponent();
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        public bool IsChangeManth { get; set; } = true;
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        protected override UCOperating_Shift_Calendar_Day_Base CreateDayControl()
        {
            return new PLOSMaintenance.UCOperating_Shift_Calendar_Day_Main();
        }

        /// <summary>
        /// 翌月を表示します。
        /// </summary>
        protected override void ChangeCalendarNextMonth()
        {
            logger.Info("稼働登録　翌月表示ボタン押下");

            NotifyNextManthChanged(new EventArgs());

            if(IsChangeManth)
            {
                base.ChangeCalendarNextMonth();
            }            
        }

        /// <summary>
        /// 前月を表示します。
        /// </summary>
        protected override void ChangeCalendarPrevMonth()
        {
            logger.Info("稼働登録　前月表示ボタン押下");

            NotifyPrevMonthChanged(new EventArgs());

            if(IsChangeManth)
            {　
                base.ChangeCalendarPrevMonth();
            }            
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// 
        /// </summary>
        private void NotifyNextManthChanged(EventArgs args)
        {
            EventHandler handler = NextManthChanged;
            if (handler != null)
            {
                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void NotifyPrevMonthChanged(EventArgs args)
        {
            EventHandler handler = PrevMonthChanged;
            if (handler != null)
            {
                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }
        #endregion "プライベートメソッド"
    }
}
