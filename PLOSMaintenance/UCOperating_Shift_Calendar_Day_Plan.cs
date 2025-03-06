using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace PLOSMaintenance
{
    public partial class UCOperating_Shift_Calendar_Day_Plan : UCOperating_Shift_Calendar_Day_Base
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
        ///  生産計画シフトカレンダー（日付）
        /// </summary>
        public UCOperating_Shift_Calendar_Day_Plan()
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
        /// <summary>
        /// ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlblShiftClicked(object sender, EventArgs e)
        {
            logger.Info("計画数一覧　日付パネル押下");
            OnPanelClicked(sender, e);
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        /// <summary>
        /// 日毎状態の更新処理
        /// </summary>
        public override void UpdateDailyStatus()
        {
            base.UpdateDailyStatus();

            if (mTargetDate != DateTime.MinValue && ExistPlan)
            {
                lblShift1.Text = $"1直 {PlanProductionList.FirstOrDefault(x => x[0] == 1)?[1].ToString() ?? "---"}";
                lblShift2.Text = $"2直 {PlanProductionList.FirstOrDefault(x => x[0] == 2)?[1].ToString() ?? "---"}";
                lblShift3.Text = $"3直 {PlanProductionList.FirstOrDefault(x => x[0] == 3)?[1].ToString() ?? "---"}";
            }
            else
            {
                lblShift1.Text = "";
                lblShift2.Text = "";
                lblShift3.Text = "";
            }
        }

        /// <summary>
        /// 背景色変更処理
        /// </summary>
        public override void ChangeColor()
        {
            if (!ExistActual && !ExistPlan)
            {
                pnlMain.BackColor = Selected ? Color.LightBlue : Color.LightGray;
                lblDay.ForeColor = Color.Gray;
            }
            else if (ExistPlan)
            {
                pnlMain.BackColor = Selected ? Color.LightBlue : Color.White;
                lblDay.ForeColor = Color.Black;
            }
            else
            {
                pnlMain.BackColor = Selected ? SelectedColor : DaysBackColor;
                lblDay.ForeColor = DaysForeColor;
            }
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        #endregion "プライベートメソッド"
    }
}
