using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PLOS.Gui.Core.Base
{
	public partial class FormExpansionImage : PLOS.Gui.Core.Base.FormFixedDialogBase
	{
		#region Private Fields
        
		private float _Magnification = 1.0F;
		private float _AspectRatio = 1.0F;
		private Boolean _FixAspectRatio = true;
        private string _Title;

		#region For Fixed Aspect Ratio

		const int WM_SIZING = 0x214;
		const int WMSZ_LEFT = 1;
		const int WMSZ_RIGHT = 2;
		const int WMSZ_TOP = 3;
		const int WMSZ_TOPLEFT = 4;
		const int WMSZ_TOPRIGHT = 5;
		const int WMSZ_BOTTOM = 6;
		const int WMSZ_BOTTOMLEFT = 7;
		const int WMSZ_BOTTOMRIGHT = 8;
		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		#endregion

		#endregion

		#region EventHandler(Delegate)
        public event EventHandler<CustumContol.ImageChangeEventArgs> ImageChange = null;
        public event EventHandler ImageNext = null;
        public event EventHandler ImagePrev = null;
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public FormExpansionImage()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
            _Title = this.Text;
		}

		#endregion

		#region Event

		/// <summary>
		/// Click Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClick(object sender, EventArgs e)
		{
			Close();
		}

		#region For Fixed Aspect Ratio

		/// <summary>
		/// WndProc
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_SIZING:
					if (_FixAspectRatio)
					{
						RECT r = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
						int w = r.right - r.left - (Size.Width - ClientSize.Width);
						int h = r.bottom - r.top - (Size.Height - ClientSize.Height);
						int dw = (int)(h * _AspectRatio + 0.5) - w;
						int dh = (int)(w / _AspectRatio + 0.5) - h;
						switch (m.WParam.ToInt32())
						{
							case WMSZ_TOP:
							case WMSZ_BOTTOM:
								r.right += dw;
								break;
							case WMSZ_LEFT:
							case WMSZ_RIGHT:
								r.bottom += dh;
								break;
							case WMSZ_TOPLEFT:
								if (dw > 0) r.left -= dw;
								else r.top -= dh;
								break;
							case WMSZ_TOPRIGHT:
								if (dw > 0) r.right += dw;
								else r.top -= dh;
								break;
							case WMSZ_BOTTOMLEFT:
								if (dw > 0) r.left -= dw;
								else r.bottom += dh;
								break;
							case WMSZ_BOTTOMRIGHT:
								if (dw > 0) r.right += dw;
								else r.bottom += dh;
								break;
						}
						Marshal.StructureToPtr(r, m.LParam, false);
					}
					goto default;
				default:
					base.WndProc(ref m);
					break;
			}
		}
		#endregion

        protected void OnImageChange(CustumContol.ImageChangeEventArgs e)
        {
            if (ImageChange != null)
            {
                this.ImageChange(this, e);
            }
        }

        protected void OnImageNext()
        {
            if (ImageNext != null)
            {
                this.ImageNext(this, EventArgs.Empty);
            }
        }

        protected void OnImagePrev()
        {
            if (ImagePrev != null)
            {
                this.ImagePrev(this, EventArgs.Empty);
            }
        }

        private void OnPictureBoxImageChange(object sender, CustumContol.ImageChangeEventArgs e)
        {
            this.OnImageChange(e);
        }

        private void OnPictureBoxImageNext(object sender, EventArgs e)
        {
            this.OnImageNext();
        }

        private void OnPictureBoxImagePrev(object sender, EventArgs e)
        {
            this.OnImagePrev();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (picBoxExpansionImage == null || picBoxExpansionImage.Image == null)
                return;

            float scale = (float)picBoxExpansionImage.Width / (float)picBoxExpansionImage.Image.Width;

            _Magnification = scale;
        }

		#endregion

		#region Property

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		//[Localizable(true)]
		//[Bindable(true)]
		[Category("FormExpansionImage")]
		public System.Drawing.Image Image
		{
			set
			{
				picBoxExpansionImage.Image = value;

				// Set Aspect Ratio
				_AspectRatio = ((float)picBoxExpansionImage.Image.Width) / ((float)picBoxExpansionImage.Image.Height);

				// 
				Size = new Size(Size.Width + (int)(picBoxExpansionImage.Image.Width * _Magnification) - picBoxExpansionImage.Size.Width,
							Size.Height + (int)(picBoxExpansionImage.Image.Height * _Magnification) - picBoxExpansionImage.Size.Height);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Category("FormExpansionImage")]
		public float Magnification
		{
			get
			{
				return _Magnification;
			}
			set
			{
				_Magnification = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Category("FormExpansionImage")]
		public float AspectRatio
		{
			get
			{
				return _AspectRatio;
			}
			set
			{
				_AspectRatio = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Category("FormExpansionImage")]
		public bool FixAspectRatio
		{
			get
			{
				return _FixAspectRatio;
			}
			set
			{
				_FixAspectRatio = value;
			}
		}

        public string Title
        {
            set { _Title = value; }
            get { return _Title; }
        }

		#endregion

		#region Public Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
