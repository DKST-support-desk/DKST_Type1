using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class UACRunElevated
	{
		#region Show Splash Methods

		/// <summary>
		/// Run Elevated Administrator
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="arguments"></param>
		/// <param name="IsExistParent"></param>
		/// <param name="parentHandle"></param>
		/// <param name="waitExit"></param>
		/// <returns> True: success</returns>
		public static Boolean RunElevated(string fileName, string arguments,
			Form parentForm, bool waitExit)
		{
			// Exists Execute file
			if (!System.IO.File.Exists(fileName))
			{
				throw new System.IO.FileNotFoundException();
			}

			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			// Use ShellExecute (Default = True, Not need)
			psi.UseShellExecute = true;
			// Set Execute Path
			psi.FileName = fileName;
			// StartInfo.Verb = "runas";
			psi.Verb = "runas";
			// Set Argument
			psi.Arguments = arguments;

			if (parentForm != null)
			{
				// Set Parent Handle
				psi.ErrorDialog = true;
				psi.ErrorDialogParentHandle = parentForm.Handle;
			}

			try
			{
				// Start
				System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
				if (waitExit)
				{
					Application.DoEvents();
					// Wait For Exit
					p.WaitForExit();
					Application.DoEvents();
				}

				if (p.ExitCode != 0)
				{
					return false;
				}
			}
			catch (System.ComponentModel.Win32Exception)
			{
				// Canceled
				return false;
			}

			return true;
		}

		#endregion
	}
}
