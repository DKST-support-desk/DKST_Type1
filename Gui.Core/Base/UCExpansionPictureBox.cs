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
	/// 
	/// </summary>
	public partial class UCExpansionPictureBox : PLOS.Gui.Core.Base.UCBase
	{
		#region Private Fields

		/// <summary>
		/// 拡大イメージ格納
		/// </summary>
		private System.Drawing.Image _ExpansionImage = null;

		/// <summary>
		/// 番号
		/// </summary>
		private UInt32 _Index = 0;

		/// <summary>
		/// 番号表示
		/// </summary>
		private Boolean _VisibleIndex = false;

		/// <summary>
		/// 番号フォーマット
		/// </summary>
		private String _FormatIndex = "{0:000}";

		/// <summary>
		/// イメージダイアログタイトル
		/// </summary>
		private String _ImageDialogTitle;

		#endregion

		#region EventHandler(Delegate)
        public event EventHandler<CustumContol.ImageChangeEventArgs> ImageChange = null;
        public event EventHandler ImageNext = null;
        public event EventHandler ImagePrev = null;
        public event EventHandler PreviewClick = null;
        #endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public UCExpansionPictureBox()
		{
			InitializeComponent();

			txtIndex.Text = String.Format(_FormatIndex, _Index);
			_ImageDialogTitle = "";
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
            if (this.PreviewClick != null)
            {
                this.PreviewClick(this, EventArgs.Empty);
            }

			FormExpansionImage frm = new FormExpansionImage();

            frm.ImageChange +=new EventHandler<CustumContol.ImageChangeEventArgs>(OnImageChange);
            frm.ImageNext +=new EventHandler(OnImageNext);
            frm.ImagePrev +=new EventHandler(OnImagePrev);

			if (_ExpansionImage != null)
			{
				frm.Image = _ExpansionImage;
				frm.FixAspectRatio = true;
				frm.Magnification = 1.0F;

				if(_Index > 0)
					frm.Text = _ImageDialogTitle + String.Format(":" + _FormatIndex, _Index);
			}

			frm.ShowDialog(this);
		}

		/// <summary>
		/// Image Change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected virtual void OnImageChange(object sender, CustumContol.ImageChangeEventArgs e)
        {
            if(this.ImageChange != null)
            {
                this.ImageChange(sender, e);
            }
        }

        protected virtual void OnImageNext(object sender, EventArgs e)
        {
            if (this.ImageNext != null)
            {
                this.ImageNext(sender, e);
            }
        }

        protected virtual void OnImagePrev(object sender, EventArgs e)
        {
            if (this.ImagePrev != null)
            {
                this.ImagePrev(sender, e);
            }
        }		
        #endregion

		#region Property

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Localizable(true)]
		[Bindable(true)]
		[Category("ExpansionPictureBox")]
		public System.Drawing.Image ExpansionImage
		{
			get
			{
				return _ExpansionImage;
			}
			set
			{
				_ExpansionImage = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Localizable(true)]
		[Bindable(true)]
		[Category("ExpansionPictureBox")]
		public System.Drawing.Image PictureBoxImage
		{
			get
			{
				return this.pictureBox.Image;
			}
			set
			{
				this.pictureBox.Image = value;
			} 
		}

		/// <summary>
		/// PictureBox
		/// </summary>
		[Category("ExpansionPictureBox")]
		public System.Windows.Forms.PictureBox PictureBox
		{
			get
			{
				return this.pictureBox;
			}
		}

		/// <summary>
		/// PictureIndex
		/// </summary>
		[Category("ExpansionPictureBox")]
		public UInt32 PictureIndex
		{
			get
			{
				return _Index;
			}
			set
			{
				_Index = value;
				txtIndex.Text = String.Format(_FormatIndex, _Index);
			}
		}

		/// <summary>
		/// VisibleIndex
		/// </summary>
		[Category("ExpansionPictureBox")]
		public Boolean VisibleIndex
		{
			get
			{
				return _VisibleIndex;
			}
			set
			{
				txtIndex.Visible = value;
				_VisibleIndex = value;
			}
		}

		/// <summary>
		/// FormatIndex
		/// </summary>
		[Category("ExpansionPictureBox")]
		public String FormatIndex
		{
			get
			{
				return _FormatIndex;
			}
			set
			{
				_FormatIndex = value;
				txtIndex.Text = String.Format(_FormatIndex, _Index);
			}
		}

		/// <summary>
		/// _ImageDialogTitle
		/// </summary>
		[Category("ExpansionPictureBox")]
		public String ImageDialogTitle
		{
			get
			{
				return _ImageDialogTitle;
			}
			set
			{
				_ImageDialogTitle = value;
			}
		}

		#endregion

    }
}
