using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core.Events
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class StatusManagerEventArgs : EventArgs
	{
		#region Inner Class/Enum

		/// <summary>
		/// 
		/// </summary>
		public enum EventArgsType
		{
			StatusChanged,	// 汎用
			EditModeChanged,


		}

		#endregion

		#region Private Fields
		/// <summary>
		/// EventArgsType
		/// </summary>
		private EventArgsType _type;

		#endregion

		#region Event Handler
		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// ｺﾝｽﾄﾗｸﾀ
		/// </summary>
		internal StatusManagerEventArgs()
		{
		}

		public StatusManagerEventArgs(StatusManagerEventArgs.EventArgsType type)
			: this()
		{
			_type = type;
		}

		#endregion

		#region Property
		/// <summary>
		/// EventArgsType
		/// </summary>
		public StatusManagerEventArgs.EventArgsType Type
		{
			get
			{
				return _type;
			}
		}

		#endregion
	}
}
