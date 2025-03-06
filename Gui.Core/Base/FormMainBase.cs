using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
	public partial class FormMainBase : PLOS.Gui.Core.Base.FormBase
	{
		#region Private Fields

		/// <summary>
		/// Rich Message Panel Ex
		/// </summary>
		private UCRichMessagePanel _RichMessagePanel = null;

		#endregion

		#region EventHandler(Delegate)
		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// Constructors
		/// </summary>
		public FormMainBase()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;

		}
		#endregion

		#region Event
		#endregion

		#region Property

		/// <summary>
		/// UCRichMessagePanel
		/// </summary>
		private UCRichMessagePanel RichMessagePanel
		{
			get
			{
				if (_RichMessagePanel != null) return _RichMessagePanel;
				return _RichMessagePanel = new UCRichMessagePanel(this);
			}
			set
			{
				_RichMessagePanel = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Show Rich Message
		/// </summary>
		/// <param name="text"></param>
		/// <param name="msgIcon"></param>
		/// <param name="AutoInvisibleTimer"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		public void ShowRichMessage(String text, System.Windows.Forms.MessageBoxIcon msgIcon = MessageBoxIcon.None, UInt16 AutoInvisibleTimer = 3000, int Width = 0, int Height = 0)
		{
			RichMessagePanel.ShowMessage(text, msgIcon, AutoInvisibleTimer, Width, Height);
			Application.DoEvents();
		}

		/// <summary>
		/// Close Rich Message
		/// </summary>
		/// <param name="enabled"></param>
		public void CloseRichMessage()
		{
			RichMessagePanel.CloseMessage();
			Application.DoEvents();
		}

		#endregion

		#region Private Methods
		#endregion
	}
}
