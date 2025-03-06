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
	public partial class FrmCalendar_MultiSelect : Form
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
		/// 計画複製画面
		/// </summary>
		public FrmCalendar_MultiSelect()
		{
			InitializeComponent();

			UCOperating_Shift_Calendar_Base.NonSelectablePeriodRange range = new UCOperating_Shift_Calendar_Base.NonSelectablePeriodRange();
			range.StartDay = DateTime.MinValue;
			range.EndDay = DateTime.Now.AddDays(-1);
			ucCalendar_MultiSelect.NonSelectablePeriod.Add(range);

			ucCalendar_MultiSelect.DaysFont =
				new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));

			ucCalendar_MultiSelect.SelectDaysBackColor = Color.LightYellow;
			ucCalendar_MultiSelect.TargetMonth = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0);
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
				return ucCalendar_MultiSelect.WholePeriodSelectDaysList.OrderBy(x => x).ToList();
			}
		}
		#endregion "プロパティ"

		//********************************************
		//* イベント
		//********************************************
		#region "イベント"
		/// <summary>
		/// 平日全選択ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAllWeekdaysClicked(object sender, EventArgs e)
		{
			logger.Info("計画複製 平日全選択ボタン押下");

			List<DateTime> lastTimeSelected = ucCalendar_MultiSelect.SelectedDaysList;

			lastTimeSelected.AddRange(ucCalendar_MultiSelect.DaysList.Where(x => (x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday) && x.Month == ucCalendar_MultiSelect.TargetMonth.Month));
			ucCalendar_MultiSelect.SelectedDaysList = lastTimeSelected;

			ucCalendar_MultiSelect.WholePeriodSelectDaysList.AddRange(ucCalendar_MultiSelect.SelectedDaysList.Where(x => !ucCalendar_MultiSelect.WholePeriodSelectDaysList.Contains(x.Date)));

			// 範囲選択開始日をリセット
			ucCalendar_MultiSelect.MultiSelectFromDate = DateTime.MinValue;
		}

		/// <summary>
		/// 土曜日全選択ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAllSaturdayClicked(object sender, EventArgs e)
		{
			logger.Info("計画複製 土曜日全選択ボタン押下");

			List<DateTime> lastTimeSelected = ucCalendar_MultiSelect.SelectedDaysList;

			lastTimeSelected.AddRange(ucCalendar_MultiSelect.DaysList.Where(x => x.DayOfWeek == DayOfWeek.Saturday && x.Month == ucCalendar_MultiSelect.TargetMonth.Month));
			ucCalendar_MultiSelect.SelectedDaysList = lastTimeSelected;

			ucCalendar_MultiSelect.WholePeriodSelectDaysList.AddRange(ucCalendar_MultiSelect.SelectedDaysList.Where(x => !ucCalendar_MultiSelect.WholePeriodSelectDaysList.Contains(x.Date)));

			// 範囲選択開始日をリセット
			ucCalendar_MultiSelect.MultiSelectFromDate = DateTime.MinValue;
		}

		/// <summary>
		/// 全選択解除ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnResetSelectClicked(object sender, EventArgs e)
		{
			logger.Info("計画複製 全選択解除ボタン押下");

			ucCalendar_MultiSelect.SelectedDaysList = new List<DateTime>();

			ucCalendar_MultiSelect.WholePeriodSelectDaysList.Clear();

			// 範囲選択開始日をリセット
			ucCalendar_MultiSelect.MultiSelectFromDate = DateTime.MinValue;
		}

		/// <summary>
		/// 計画登録ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOkClicked(object sender, EventArgs e)
		{
			logger.Info("計画複製 計画登録ボタン押下");

			DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// キャンセルボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClicked(object sender, EventArgs e)
		{
			logger.Info("計画複製 キャンセルボタン押下");

			DialogResult = DialogResult.Cancel;
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
