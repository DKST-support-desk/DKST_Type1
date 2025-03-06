using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace PLOS.Gui.Core.DataGridViews
{
	/// <summary>
	/// 
	/// </summary>
	public class SyncScrollDataGridView : DataGridView
	{
		#region Private Fields

		private SyncScrollDataGridView _SyncScrollGridView = null;

		#endregion

		#region EventHandler(Delegate)


		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// 
		/// </summary>
		public SyncScrollDataGridView()
		{
			Scroll += OnScroll;
			Syncable = true;
		}

		#endregion

		#region Property

		public Boolean Syncable { get; set; }

		public SyncScrollDataGridView SyncScrollGridView
		{
			private get
			{
				return _SyncScrollGridView;
			}
			set
			{
				if (_SyncScrollGridView != this) _SyncScrollGridView = value;
			}
		}

		#endregion

		#region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScroll(object sender, ScrollEventArgs e)
		{
			if (FirstDisplayedScrollingRowIndex >= 0 &&
				_SyncScrollGridView != null &&
				_SyncScrollGridView.RowCount == this.RowCount &&
				Syncable && _SyncScrollGridView.Syncable)
			{
				try
				{
					//指定位置までスクロール
					_SyncScrollGridView.FirstDisplayedScrollingRowIndex = FirstDisplayedScrollingRowIndex;
				}
				catch (System.ArgumentOutOfRangeException ex)
				{
#if DEBUG
					String exStr = ex.ToString();
					int RowIndex = FirstDisplayedScrollingRowIndex;
#endif
				}
			}
			//フォーカスが別のコントロールに移動しないようにする
			this.Focus();
		}

#endregion
	}
}
