using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLOSMaintenance.Events
{
	public class Operating_Shift_PatternEventArgs : EventArgs
	{
		public int PatternId { get; set; }
		/// <summary> true:Upsert, false:DELETE </summary>
		public Boolean IsUpdate { get; set; }
		public Operating_Shift_PatternEventArgs(int patternId, Boolean isUpdate)
		{
			PatternId = patternId;
			IsUpdate = isUpdate;
		}
	}
}
