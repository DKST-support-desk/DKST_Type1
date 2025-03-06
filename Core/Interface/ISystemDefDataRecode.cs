using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core.Interface
{
	/// <summary>
	/// System Define Data Recode Interface
	/// </summary>
	public interface ISystemDefDataRecode : IComparable<int>
	{
		#region Property

		/// <summary>
		/// Id
		/// </summary>
		int Id
		{ get; }

		/// <summary>
		/// Def String
		/// </summary>
		String DefString
		{
			get;
		}

		/// <summary>
		/// Disp Order
		/// </summary>
		int DispOrder
		{
			get;
		}

		/// <summary>
		/// Comment
		/// </summary>
		String Comment
		{
			get;
		}

		/// <summary>
		/// Query Tag
		/// </summary>
		String QueryTag
		{
			get;
		}

		/// <summary>
		/// Seq Number
		/// </summary>
		int SeqNumber
		{
			get;
		}

		/// <summary>
		/// Logical Stat
		/// </summary>
		int LogicalStat
		{
			get;
		}

        /// <summary>
        /// Is Error
        /// </summary>
        bool IsError
        {
            get;
        }

		#endregion

		#region Public Methods
		#endregion
	}
}
