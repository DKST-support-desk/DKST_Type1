using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PLOSMaintenance
{
    public partial class UCOperating_Shift_Calendar_Plan : PLOSMaintenance.UCOperating_Shift_Calendar_Base
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        public UCOperating_Shift_Calendar_Plan()
        {
            InitializeComponent();
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
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
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        protected override UCOperating_Shift_Calendar_Day_Base CreateDayControl()
        {
            return new PLOSMaintenance.UCOperating_Shift_Calendar_Day_Plan();
        }
        #endregion "プライベートメソッド"
    }
}
