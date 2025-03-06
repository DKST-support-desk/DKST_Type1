using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
    /// <summary>
	/// FormBase
    /// </summary>
    public partial class FormBase : Form
    {
        #region Private Fields

		/// <summary>
		/// Message Result
		/// </summary>
		private DialogResult _MessageResult = DialogResult.Cancel;

        #endregion

        #region EventHandler(Delegate)
		// --------- for Invoke Delegate ---------
        private delegate void ShowMessageDelegate(String message, String caption);
		private delegate DialogResult ShowResultMessageDelegate(String message, String caption, MessageBoxButtons buttons);
		protected delegate void VoidDelegate();
		protected delegate void BooleanDelegate(Boolean bSet);
		protected delegate void DoubleDelegate(Double dSet);
		protected delegate void StringDelegate(String str);

        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormBase()
        {
            InitializeComponent();

        }

        #endregion

        #region Property

		/// <summary>
		/// OperationEnable
		/// </summary>
		public Boolean OperationEnable
		{
			set
			{
				SetOperationEnable(value);
			}
		}

        #endregion

        #region Protected Methods

        #region ShowMessage

		/// <summary>
		/// ShowInfoMessage
		/// </summary>
		protected void ShowInfoMessage(String message, String caption)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new ShowMessageDelegate(ShowInfoMessage), message, caption);
				return;
			}

			MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

        /// <summary>
        /// ShowErrorMessage
        /// </summary>
        protected void ShowErrorMessage(String message, String caption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowMessageDelegate(ShowErrorMessage), message, caption);
                return;
            }

            MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		/// <summary>
		/// ShowResultMessage
		/// </summary>
		protected DialogResult ShowResultMessage(String message, String caption, MessageBoxButtons buttons)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new ShowResultMessageDelegate(ShowResultMessage), message, caption, buttons);
				return _MessageResult;
			}

			_MessageResult = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);
			return _MessageResult;
		}

		#endregion

		#region SetOperationEnable
		/// <summary>
		/// OperationEnable
		/// </summary>
		/// <param name="bEnable"></param>
		protected virtual void SetOperationEnable(Boolean bEnable)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new BooleanDelegate(SetOperationEnable), bEnable);
				return;
			}
		}
		#endregion

		#region Load/Save WindowStatusFromProperties

		/// <summary>
		/// Load Window Status From Properties
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="PropertiesNamePrefix"></param>
		/// <remarks>
		/// Need Properties Setting "xxxxxWindowState", "xxxxxSize" , "xxxxxLocation"
		/// "xxxxx" define PropertiesNamePrefix
		/// </remarks>
		protected void LoadWindowStatusFromProperties(System.Configuration.ApplicationSettingsBase settings, String PropertiesNamePrefix)
		{
			try
			{
				FormWindowState state = (FormWindowState)settings[PropertiesNamePrefix + "WindowState"];
				switch (state)
				{
					case FormWindowState.Normal:
					case FormWindowState.Maximized:
					case FormWindowState.Minimized:
						this.WindowState = state;
						break;
					default:
						break;
				}
			}
			catch{}

			try
			{
				Size windowSize = (Size)settings[PropertiesNamePrefix + "Size"];
				if (windowSize.Height > 0 && windowSize.Width > 0)
				{
					this.Size = windowSize;
				}
			}
			catch { }

			try
			{
				Point windowLocation = (Point)settings[PropertiesNamePrefix + "Location"];
				if (windowLocation.X >= 0 && windowLocation.Y >= 0)
				{
					this.Location = windowLocation;
					this.StartPosition = FormStartPosition.Manual;
				}
			}
			catch { }
		}

		/// <summary>
		/// Save Window Status From Properties
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="PropertiesNamePrefix"></param>
		/// <remarks>
		/// Need Properties Setting "xxxxxWindowState", "xxxxxSize" , "xxxxxLocation"
		/// "xxxxx" define PropertiesNamePrefix
		/// </remarks>
		protected void SaveWindowStatusFromProperties(System.Configuration.ApplicationSettingsBase settings, String PropertiesNamePrefix)
		{
			try
			{
				settings[PropertiesNamePrefix + "WindowState"] = (int)this.WindowState;
			}
			catch { }
			try
			{
				// 最大化、最小化時にはサイズの書込みしない
				// 最大化、最小化時に書込みすると、
				// 通常表示にそのサイズに最大又は最小になってしまう
				switch (this.WindowState)
				{
					case FormWindowState.Maximized:
					case FormWindowState.Minimized:
						break;
					case FormWindowState.Normal:
					default:
						settings[PropertiesNamePrefix + "Size"] = this.Size;
						break;
				}
			}
			catch { }
			try
			{
				// 最大化、最小化時には位置の書込みしない
				// 最大化、最小化時に書込みすると、
				// 通常表示にそのサイズに最大又は最小になってしまう
				switch (this.WindowState)
				{
					case FormWindowState.Maximized:
					case FormWindowState.Minimized:
						break;
					case FormWindowState.Normal:
					default:
						settings[PropertiesNamePrefix + "Location"] = this.Location;
						break;
				}
			}
			catch { }
			settings.Save();
		}

		#endregion

		#endregion
	}
}
