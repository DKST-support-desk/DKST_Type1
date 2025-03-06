using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
	/// <summary>
	/// Status　Manager Class
	/// <remarks>
	/// Singleton Model
	/// </remarks>
	/// </summary>
	public class StatusManager
	{
		#region Static Fields
		/// <summary>
		/// Self instance
		/// </summary>
		internal static StatusManager _instance = null;

		/// <summary>
		/// 同期ｵﾌﾞｼﾞｪｸﾄ
		/// </summary>
		internal static object _asynObject_Create = new object();
		#endregion

		#region Static Methods

		#region GetInstance Methods

		/// <summary>
		/// static GetInstance
		/// </summary>
		/// <returns></returns>
		static public StatusManager GetInstance()
		{
			lock (_asynObject_Create)
			{
				//if (_instance == null)
				//{
				//    _instance = new StatusManager();
				//}
				// 

				// null-coalescing operator
				_instance = _instance ?? new StatusManager();
				return _instance;
			}
		}
		#endregion

		#endregion

		#region Private Fields

		/// <summary>
		/// EditMode
		/// </summary>
		private EditMode _EditMode;

		#endregion

		#region EventHandler(Delegate)

		/// <summary>
		/// For Notify Data Change
		/// </summary>
		public event EventHandler<Events.StatusManagerEventArgs> StatusChanged;

		#endregion

		#region Constructors and Destructor

        /// <summary>
		/// Constructors
        /// </summary>
		internal StatusManager()
        {

		}
		#endregion

		#region Event
		#endregion

		#region Property

		/// <summary>
		/// EditMode
		/// </summary>
		public EditMode EditMode
		{
			get
			{
				return _EditMode;
			}
			set
			{
				_EditMode = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Notify Status Changed
		/// </summary>
		public void NotifyStatusChange(Events.StatusManagerEventArgs args)
		{
			EventHandler<Events.StatusManagerEventArgs> handler = StatusChanged;
			if (handler != null)
			{
				foreach (EventHandler<Events.StatusManagerEventArgs> evhd in handler.GetInvocationList())
				{
					evhd(this, args);
				}
			}
		}

		#endregion

		#region Private Methods
		#endregion
	}
}
