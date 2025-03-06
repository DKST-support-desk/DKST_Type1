using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PLOS.Gui.Core.CustumContol
{
	/// <summary>
	/// PictureBoxEx
	/// </summary>
	public partial class PictureBoxEx : System.Windows.Forms.PictureBox
	{
        #region Private Fields

		/// <summary>
		/// Mouse Leave Image
		/// </summary>
		private System.Drawing.Image _LeaveImage = null;

		/// <summary>
		/// Mouse Enter Image
		/// </summary>
		private System.Drawing.Image _EnterImage = null;

		/// <summary>
		/// Clicked Image
		/// </summary>
		private System.Drawing.Image _ClickedImage = null;

		#endregion

        #region Constructors and Destructor
        /// <summary>
        /// 
        /// </summary>
		public PictureBoxEx()
        {
            InitializeComponent();

			MouseEnter += new EventHandler(OnMouseEnter);
			MouseLeave += new EventHandler(OnMouseLeave);
        }

        #endregion

        #region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseEnter(object sender, EventArgs e)
		{
			base.Image = EnterImage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseLeave(object sender, EventArgs e)
		{
			base.Image = LeaveImage;
			base.Refresh();
		}

        #endregion

        #region Property

		/// <summary>
		/// 
		/// </summary>
		public new System.Drawing.Image Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
				_LeaveImage = value;
			}
		}

		/// <summary>
		/// Mouse Leave Image
		/// </summary>
		[Category("CustomControl PictureBoxEx")]
		public System.Drawing.Image LeaveImage
		{
			get
			{
				return _LeaveImage;
			}
			set
			{
				_LeaveImage = value;
			}
		}

		/// <summary>
		/// Mouse Enter Image
		/// </summary>
		[Category("CustomControl PictureBoxEx")]
		public System.Drawing.Image EnterImage
		{
			get
            {
				return _EnterImage;
            }
            set
            {
				_EnterImage = value;
            }
		}

		/// <summary>
		/// Clicked Image
		/// </summary>
		[Category("CustomControl PictureBoxEx")]
		public System.Drawing.Image ClickedImage
		{
			get
			{
				return _ClickedImage;
			}
			set
			{
				_ClickedImage = value;
			}
		}

        #endregion	
	}
}
