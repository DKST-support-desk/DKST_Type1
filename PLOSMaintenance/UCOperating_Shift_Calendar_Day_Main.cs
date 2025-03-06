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

namespace PLOSMaintenance
{
	public partial class UCOperating_Shift_Calendar_Day_Main : UCOperating_Shift_Calendar_Day_Base
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
        public UCOperating_Shift_Calendar_Day_Main()
        {
            InitializeComponent();

            this.picProduction.Click += new System.EventHandler(this.OnPanelClicked);
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 画像
        /// </summary>
        public Image Image
        {
            set
            {
                picProduction.Visible = value != null;
                picProduction.Image = value;
            }
        }
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
        /// <summary>
        /// 日毎状態の更新処理
        /// </summary>
        public override void UpdateDailyStatus()
        {
            base.UpdateDailyStatus();

            picProduction.Visible = false;

            if (mTargetDate != DateTime.MinValue && ExistActual && ExistPlan)
            {
                picProduction.Visible = true;

                if (faceIconFlg)
                {
                    if (ActualProductionQuantity >= PlanProductionList.Sum(x => x[1]))
                    {
                        Bitmap bm = global::PLOSMaintenance.Properties.Resources.StatusGood;
                        bm.MakeTransparent();
                        picProduction.Image = bm;
                    }
                    else
                    {
                        Bitmap bm = global::PLOSMaintenance.Properties.Resources.StatusBad;
                        bm.MakeTransparent();
                        picProduction.Image = bm;
                    }
                }
            }
            //if (mTargetDate != DateTime.MinValue && ExistActual && ExistPlan)
            //{
            //    picProduction.Visible = true;
            //    if (ActualProductionQuantity >= PlanProductionList.Sum(x => x[1]))
            //    {
            //        Bitmap bm = global::PLOSMaintenance.Properties.Resources.StatusGood;
            //        bm.MakeTransparent();
            //        picProduction.Image = bm;
            //    }
            //    else
            //    {
            //        Bitmap bm = global::PLOSMaintenance.Properties.Resources.StatusBad;
            //        bm.MakeTransparent();
            //        picProduction.Image = bm;
            //    }
            //}
        }

        /// <summary>
        /// 背景色変更処理
        /// </summary>
        public override void ChangeColor()
        {
            if (IsSample)
            {
                return;
            }

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
