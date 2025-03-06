using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
	/// <summary>
	/// 
	/// </summary>
	public partial class UCEditableBase : PLOS.Gui.Core.Base.UCBase
	{
		#region Private Fields

		/// <summary>
		/// EditMode
		/// </summary>
		private PLOS.Core.EditMode _EditMode;

		private Boolean _EditLocked;

		#endregion

		#region Private Fields
		#endregion

		#region EventHandler(Delegate)

		/// <summary>
		/// 編集完了イベント
		/// </summary>
		public event EventHandler<EventArgs> EditCompleted;

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public UCEditableBase()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
			// For Design Mode (And For Control.DesignMode Dug)
			if (PLOS.Gui.Core.Base.UCBase.IsInDesignMode()) return;

			// 編集不可
			_EditMode = PLOS.Core.EditMode.ReadOnly;

			// コントロール編集ロック(見積り解説用に追加)
			_EditLocked = false;	// 編集可能

			// Status Changed Event Handler
			PLOS.Core.StatusManager.GetInstance().StatusChanged += new EventHandler<PLOS.Core.Events.StatusManagerEventArgs>(OnStatusManager_StatusChanged);
		}
		#endregion

		#region Event

		/// <summary>
		/// Status Manager Status Change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnStatusManager_StatusChanged(object sender, PLOS.Core.Events.StatusManagerEventArgs e)
		{
			if (e.Type == PLOS.Core.Events.StatusManagerEventArgs.EventArgsType.EditModeChanged)
			{
				if (_EditLocked)
					EditMode = PLOS.Core.EditMode.ReadOnly;
				else
					EditMode = PLOS.Core.StatusManager.GetInstance().EditMode;
			}
		}

		#endregion

		#region Property
		/// <summary>
		/// EditMode
		/// </summary>
		public virtual PLOS.Core.EditMode EditMode
		{
			get
			{
				return _EditMode;
			}
			set
			{
				// 編集完了
				if (_EditMode == PLOS.Core.EditMode.Editable &&
					value == PLOS.Core.EditMode.ReadOnly)
				{
					// 編集完了通知
					NotifyEditCompleted();
				}
				_EditMode = value;
			}
		}

		/// <summary>
		/// EditLocked
		/// </summary>
		public virtual Boolean EditLocked
		{
			get
			{
				return _EditLocked;
			}
			set
			{
				_EditLocked = value;
				if (value) EditMode = PLOS.Core.EditMode.ReadOnly;
			}
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// 編集完了通知
		/// </summary>
		private void NotifyEditCompleted()
		{
			EventHandler<EventArgs> handler = EditCompleted;
			if (handler != null)
			{
				EventArgs args = new EventArgs();

				foreach (EventHandler<EventArgs> evhd in handler.GetInvocationList())
				{
					evhd(this, args);
				}
			}
		}
		#endregion
	}
}
