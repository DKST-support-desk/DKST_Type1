using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
	/// <summary>
	/// ListSelectionItem
	/// </summary>
	[Serializable]
	public class ListSelectionItem
	{
		#region Private Fields
		/// <summary>
		/// 
		/// </summary>
		private String _Id;

		/// <summary>
		/// 
		/// </summary>
		private String _Name;

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		public ListSelectionItem(String id, String name)
		{
			_Id = id;
			_Name = name;
		}

		#endregion

		#region Property
		/// <summary>
		/// ID
		/// </summary>
		public String Id
		{
			get
			{
				return _Id;
			}
		}

		/// <summary>
		/// Name
		/// </summary>
		public String Name
		{
			get
			{
				return _Name;
			}
		}

		#endregion
	}
}
