using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core.Interface
{
	/// <summary>
	/// IDataManager
	/// </summary>
	public interface ISystemDefManager
	{
		#region Event
		#endregion

		#region Property
		#endregion

		#region Public Methods

		/// <summary>
		/// GetSysDefItem
		/// </summary>
		/// <param name="catgry1"></param>
		/// <param name="categry2"></param>
		/// <param name="categry_idx"></param>
		/// <returns></returns>
		ISystemDefDataRecode GetSysDefItem(int catgry1, int categry2, int categry_idx);

		/// <summary>
		/// GetSysDefList
		/// </summary>
		/// <param name="catgry1"></param>
		/// <param name="categry2"></param>
		/// <param name="withBlank"></param>
		/// <returns></returns>
		IList<ISystemDefDataRecode> GetSysDefList(int catgry1, int categry2, Boolean withBlank = false);

		#endregion
	}
}
