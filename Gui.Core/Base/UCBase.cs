using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UCBase : System.Windows.Forms.UserControl
    {
        #region Private Fields
		/// <summary>
		/// Message Result
		/// </summary>
		private DialogResult _MessageResult = DialogResult.Cancel;

		#endregion

        #region EventHandler(Delegate)
		// Invoke() 用デリゲート
		private delegate void ShowMessageDelegate(String message, String caption);
		private delegate DialogResult ShowResultMessageDelegate(String message, String caption, MessageBoxButtons buttons);
		protected delegate void VoidDelegate();
		protected delegate void BooleanDelegate(Boolean bSet);
		protected delegate void ObjectDelegate(Object bSet);

		#endregion

        #region Constructors and Destructor
        /// <summary>
        /// 
        /// </summary>
        public UCBase()
        {
            InitializeComponent();
        }
        #endregion

        #region Event
        #endregion

        #region Property

        #endregion

        #region static Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsInDesignMode()
        {
            bool returnFlag = false;
#if DEBUG  
            if ( System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime )
            {
                returnFlag = true;
            }
            else if ( Process.GetCurrentProcess().ProcessName.ToUpper().Equals( "DEVENV" ) )
            {
                returnFlag = true;
            }
#endif
            return returnFlag;
        }
        #endregion

        #region Public Methods

        #region 描画停止
        public void BeginControlUpdate()
        {
            WinApi.BeginControlUpdate(this);
        }
        #endregion

        #region 描画再開
        public void EndControlUpdate()
        {
            WinApi.EndControlUpdate(this);
        }
        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
		/// ShowErrorMessage
		/// </summary>
		protected void ShowMessage(String message, String caption)
		{
			ShowErrorMessage(message, caption);
		}

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

			_MessageResult = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Information);
			return _MessageResult;
		}

		#endregion

		#region Private Methods
		#endregion
	}
}
