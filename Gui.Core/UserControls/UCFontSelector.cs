using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.UserControls
{
	public partial class UCFontSelector : PLOS.Gui.Core.Base.UCBase
	{
		#region Private Fields

		/// <summary>
		/// Selected Font
		/// </summary>
		private System.Drawing.Font _SelectFont = null;

		/// <summary>
		/// 
		/// </summary>
		private Boolean _EnableFontSelectButton = true;

		#endregion

		#region EventHandler(Delegate)
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public UCFontSelector()
		{
			InitializeComponent();
		}

		#endregion

		#region Event

		/// <summary>
		/// On Font Dialog Button Clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFontDialogButtonClicked(object sender, EventArgs e)
		{
			System.Windows.Forms.FontDialog dlg = new FontDialog();

			dlg.Font = _SelectFont != null ? _SelectFont: this.Font;
			dlg.ShowEffects = false;
			if (dlg.ShowDialog(this) != DialogResult.Cancel)
			{
				this.SelectFont = dlg.Font;
			}
		}
	
		#endregion

		#region Property

		/// <summary>
		/// Font
		/// </summary>
		public System.Drawing.Font SelectFont
		{
			get
			{
				return _SelectFont;
			}
			set
			{
				_SelectFont = value;
				if (_SelectFont != null)
				{
					this.lblFontName.Text = _SelectFont.Name;
					this.lblFontName.Font = _SelectFont;
					this.lblSize.Text = String.Format("{0} Point", _SelectFont.SizeInPoints);
				}
				else
				{
					this.lblFontName.Font = this.Font;
				}
			}
		}

		/// <summary>
		/// Font Selecter Button Text
		/// </summary>
		public String FontSelectButtonText
		{
			set
			{
				this.btnFontDialog.Text = value;
			}
		}

		/// <summary>
		/// Enable Font Select Button
		/// </summary>
		public Boolean EnableFontSelectButton
		{
			get
			{
				return _EnableFontSelectButton;
			}
			set
			{
				_EnableFontSelectButton = value;
				btnFontDialog.Enabled = value;
			}
		}

		#endregion

		#region Public Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
