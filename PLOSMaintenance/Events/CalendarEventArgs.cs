using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLOSMaintenance.Events
{
	public class CalendarEventArgs : EventArgs
	{
		public DateTime TargetDate { get; set; }

		public Boolean Selected { get; set; }

		public Boolean PressedShiftKey { get; set; }

		/// <summary>
		/// 日付の変更をキャンセルするかを設定/取得します。
		/// </summary>
		public bool Cancel { get; set; } = false;

		public CalendarEventArgs()
		{

		}

		public CalendarEventArgs(DateTime targetDate, Boolean selected, Boolean pressedShiftKey)
		{
			TargetDate = targetDate;
			Selected = selected;
			PressedShiftKey = pressedShiftKey;
		}
	}
}
