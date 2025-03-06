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
    public partial class FrmCalendar_Plan : Form
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
		/// 計画数一覧画面
		/// </summary>
        public FrmCalendar_Plan()
        {
            InitializeComponent();

            ucCalendar_Plan.DaysFont =
                new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            ucCalendar_Plan.TargetMonth = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0);
        }
		#endregion "コンストラクタ"

		//********************************************
		//* プロパティ
		//********************************************
		#region "プロパティ"
		/// <summary>
		/// 選択日付リスト
		/// </summary>
		public List<DateTime> SelectedDaysList
		{
			get
			{
				return ucCalendar_Plan.SelectedDaysList;
			}
		}
		#endregion "プロパティ"

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// 戻るボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
			logger.Info("計画数一覧 戻るボタン押下");

            DialogResult = DialogResult.Cancel;
            Close();
        }
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
		#endregion "プライベートメソッド"
    }
}
