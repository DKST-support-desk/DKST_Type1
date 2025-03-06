using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	/// <summary>
	/// UACShieldIconButton
	/// </summary>
	public partial class UACShieldIconButton : System.Windows.Forms.Button
	{
		#region DllImport

		/// <summary>
		/// SendMessage
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="Msg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// Constructors
		/// </summary>
		public UACShieldIconButton()
		{
			InitializeComponent();

			//////////// For UAC SHIELD Icon ////////////
			// Button FlatStyle
			this.FlatStyle = FlatStyle.System;
			// Button Handle
			HandleRef hwnd = new HandleRef(this, this.Handle);
			// SHIELD Icon Define 
			uint BCM_SETSHIELD = 0x0000160C;

			SendMessage(hwnd, BCM_SETSHIELD, new IntPtr(0), new IntPtr(1));
		}

		#endregion
	}
}
