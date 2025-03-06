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
	/// 折り畳み表示ユーザコントロール
	/// </summary>
	public partial class UCFoldableBase : UCEditableBase, IComparable
	{
		#region Private Fields

		#endregion

		#region EventHandler(Delegate)

		/// <summary>
		/// EventHandler Toggle Changed
		/// </summary>
		[Browsable(false)]
		public event EventHandler ToggleClicked;

		/// <summary>
		/// EventHandler Toggle Changed
		/// </summary>
		[Browsable(false)]
		public event EventHandler ToggleChanged;

		///// <summary>
		///// EventHandler Fold Panel Changed
		///// </summary>
		//[Browsable(false)]
		//public event EventHandler FoldPanelChanged;
	
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public UCFoldableBase()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
			// For Design Mode (And For Control.DesignMode Dug)
			if (PLOS.Gui.Core.Base.UCBase.IsInDesignMode()) return;

			tlblTitle.ToggleClicked += new System.EventHandler(OnToggleClicked);
			tlblTitle.ToggleChanged += new System.EventHandler(OnToggleChanged);
		}

		#endregion

		#region Event
		#endregion

		#region Property

		/// <summary>
		/// 
		/// </summary>
		public int SortOrder  { get; set; }

		#region Sub Class (Toggle Label) Property

		/// <summary>
		/// ToggleLabel
		/// </summary>
		[Category("UserControl UCFoldableBase")]
		[Description("Toggle Label")]
		public CustumContol.ToggleLabel ToggleLabel
		{
			get
			{
				return tlblTitle;
			}
		}

		/// <summary>
		/// Toggle Status
		/// </summary>
		[Category("UserControl UCFoldableBase")]
		[Description("Toggle Status")]
		public Boolean ToggleFold
		{
			get
			{
				// For Design Mode
				if (this.DesignMode) return true;
				// For Design Mode (And For Control.DesignMode Dug)
				if (PLOS.Gui.Core.Base.UCBase.IsInDesignMode()) return true;

				return tlblTitle.ToggleStatus;
			}
			set
			{
				// For Design Mode
				if (this.DesignMode) return;
				// For Design Mode (And For Control.DesignMode Dug)
				if (PLOS.Gui.Core.Base.UCBase.IsInDesignMode()) return;

				if (tlblTitle.ToggleStatus != value)
				{
					tlblTitle.ToggleStatus = value;
					// Fold Panel Visible Change
					FoldPanelVisible = value;
				}
			}
		}

		/// <summary>
		/// BaseColor
		/// </summary>
		[Category("UserControl UCFoldableBase")]
		[Description("Baseed Color")]
		public System.Drawing.Color ToggleLabelBaseColor
		{
			get
			{
				return tlblTitle.BaseColor;
			}
			set
			{
				tlblTitle.BaseColor = value;
			}
		}

		/// <summary>
		/// ReadOnly Status
		/// </summary>
		[Category("UserControl UCFoldableBase")]
		[Description("Toggle Locked")]
		public Boolean ToggleLocked
		{
			get
			{
				return tlblTitle.ReadOnly;
			}
			set
			{
				tlblTitle.ReadOnly = value;
			}
		}

		#endregion

		/// <summary>
		/// メニュートグル状態とDataGridViewの表示状態を同期
		/// </summary>
		[Category("UserControl UCFoldableBase")]
		[Description("Fold Panel Visible")]
		protected virtual Boolean FoldPanelVisible
		{
			get
			{
				return pnlFold.Visible;
			}
			set
			{
				pnlFold.Visible = value;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// On Toggle Clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnToggleClicked(object sender, EventArgs eArgs)
		{
			NotifyToggleClicked(eArgs);
		}

		/// <summary>
		/// On Toggle Changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnToggleChanged(object sender, EventArgs eArgs)
		{
			FoldPanelVisible = tlblTitle.ToggleStatus;

			NotifyToggleChanged(eArgs);
		}

		#endregion

		#region Public Methods
		#region Notify

		/// <summary>
		/// NotifyToggleClicked
		/// </summary>
		public void NotifyToggleClicked(EventArgs args)
		{
			EventHandler handler = ToggleClicked;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, args);
				}
			}
		}

		/// <summary>
		/// NotifyToggleChanged
		/// </summary>
		public void NotifyToggleChanged(EventArgs args)
		{
			EventHandler handler = ToggleChanged;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, args);
				}
			}
		}

		#endregion
		#endregion

		#region Private Methods
		#endregion

		#region IComparable Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int CompareTo(Object target)
		{
			if (target is UCFoldableBase)
			{
				return ((UCFoldableBase)target).SortOrder - SortOrder;
			}
			else return 0;
		}

		#endregion
	}
}
