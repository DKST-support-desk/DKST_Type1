using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	public partial class RowControlPanel2 : Panel
	{
		#region Private Fields

		/// <summary>
		/// PictureBox
		/// </summary>
		private Control _ctrlAddRow;
		private Control _ctrlDelRow;
		private Control _ctrlUpRow;
		private Control _ctrlDownRow;

		/// <summary>
		/// Size
		/// </summary>
		private System.Drawing.Size _ControlItemSize;

		/// <summary>
		/// Margin
		/// </summary>
		private System.Windows.Forms.Padding _ControlItemMargin;

		#endregion

		#region EventHandler(Delegate)

		/// <summary>
		/// EventHandler AddRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler AddRowClick;

		/// <summary>
		/// EventHandler DelRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler DelRowClick;

		/// <summary>
		/// EventHandler UpRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler UpRowClick;

		/// <summary>
		/// EventHandler DownRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler DownRowClick;

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public RowControlPanel2()
		{
			_ControlItemSize = new System.Drawing.Size(24, 24);

			// For Design Mode
			if (this.DesignMode) return;

			InitializeComponentCustum();

			InitializeComponent();
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="container"></param>
		public RowControlPanel2(IContainer container)
		{
			_ControlItemSize = new System.Drawing.Size(24, 24);

			// For Design Mode
			if (this.DesignMode) return;

			container.Add(this);

			InitializeComponentCustum();

			InitializeComponent();
		}

		#endregion

		#region Event
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		private void OnAddRow_Click(object sender, EventArgs eArgs)
		{
			EventHandler handler = AddRowClick;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		private void OnDelRow_Click(object sender, EventArgs eArgs)
		{
			EventHandler handler = DelRowClick;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		private void OnUpRow_Click(object sender, EventArgs eArgs)
		{
			EventHandler handler = UpRowClick;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		private void OnDownRow_Click(object sender, EventArgs eArgs)
		{
			EventHandler handler = DownRowClick;
			if (handler != null)
			{
				foreach (EventHandler evhd in handler.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}
		}

		#endregion

		#region Property

		/// <summary>
		/// Toggle True's Image
		/// </summary>
		[Category("CustomControl RowControlPanel")]
		[Localizable(false)]
		[Description("Control Item Size")]
		public System.Drawing.Size ControlItemSize
		{
			get
			{
				return _ControlItemSize;
			}
			set
			{
				if (value != null)
				{
					_ControlItemSize = value;
					_ctrlAddRow.Size = _ControlItemSize;
					_ctrlDelRow.Size = _ControlItemSize;
					_ctrlUpRow.Size = _ControlItemSize;
					_ctrlDownRow.Size = _ControlItemSize;
					InitializeControlItem();
				}
			}
		}

		/// <summary>
		/// ControlItemMargin
		/// </summary>
		[Category("CustomControl RowControlPanel")]
		[Localizable(true)]
		public System.Windows.Forms.Padding ControlItemMargin
		{
			get
			{
				return _ControlItemMargin;
			}
			set
			{
				if (value != null)
				{
					_ControlItemMargin = value;
					InitializeControlItem();
				}
			}
		}

		#endregion

		#region Public Methods
		#endregion

		#region Private Methods

		/// <summary>
		/// InitializeComponent Custum
		/// </summary>
		private void InitializeComponentCustum()
		{
			if (this.DesignMode) return;

			_ctrlAddRow = new System.Windows.Forms.Button();
			_ctrlDelRow = new System.Windows.Forms.Button();
			_ctrlUpRow = new System.Windows.Forms.Button();
			_ctrlDownRow = new System.Windows.Forms.Button();

			///////////////////
			//((System.ComponentModel.ISupportInitialize)_ctrlAddRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_ctrlDelRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_ctrlUpRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_ctrlDownRow).BeginInit();
			SuspendLayout();

			_ctrlAddRow.Size = _ControlItemSize;
			_ctrlAddRow.Name = "ImgAddRow";
			_ctrlAddRow.TabStop = true;
			((System.Windows.Forms.Button)_ctrlAddRow).FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			((System.Windows.Forms.Button)_ctrlAddRow).ForeColor = System.Drawing.Color.CadetBlue;
			((System.Windows.Forms.Button)_ctrlAddRow).Margin = new System.Windows.Forms.Padding(0);
			((System.Windows.Forms.Button)_ctrlAddRow).UseVisualStyleBackColor = false;
			_ctrlAddRow.Click += new EventHandler(OnAddRow_Click);
			_ctrlAddRow.DoubleClick += new EventHandler(OnAddRow_Click);

			_ctrlDelRow.Size = _ControlItemSize;
			_ctrlDelRow.Name = "ImgDelRow";
			_ctrlDelRow.TabStop = true;
			((System.Windows.Forms.Button)_ctrlDelRow).FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			((System.Windows.Forms.Button)_ctrlDelRow).ForeColor = System.Drawing.Color.CadetBlue;
			((System.Windows.Forms.Button)_ctrlDelRow).Margin = new System.Windows.Forms.Padding(0);
			((System.Windows.Forms.Button)_ctrlDelRow).Text = "  ";
			((System.Windows.Forms.Button)_ctrlDelRow).UseVisualStyleBackColor = false;
			_ctrlDelRow.Click += new EventHandler(OnDelRow_Click);
			_ctrlDelRow.DoubleClick += new EventHandler(OnDelRow_Click);

			_ctrlUpRow.Size = _ControlItemSize;
			_ctrlUpRow.Name = "ImgUpRow";
			_ctrlUpRow.TabStop = true;
			((System.Windows.Forms.Button)_ctrlUpRow).FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			((System.Windows.Forms.Button)_ctrlUpRow).ForeColor = System.Drawing.Color.CadetBlue;
			((System.Windows.Forms.Button)_ctrlUpRow).Margin = new System.Windows.Forms.Padding(0);
			((System.Windows.Forms.Button)_ctrlUpRow).UseVisualStyleBackColor = false;
			_ctrlUpRow.Click += new EventHandler(OnUpRow_Click);
			_ctrlUpRow.DoubleClick += new EventHandler(OnUpRow_Click);

			_ctrlDownRow.Size = _ControlItemSize;
			_ctrlDownRow.Name = "ImgDownRow";
			_ctrlDownRow.TabStop = true;
			((System.Windows.Forms.Button)_ctrlDownRow).FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			((System.Windows.Forms.Button)_ctrlDownRow).ForeColor = System.Drawing.Color.CadetBlue;
			((System.Windows.Forms.Button)_ctrlDownRow).Margin = new System.Windows.Forms.Padding(0);
			((System.Windows.Forms.Button)_ctrlDownRow).UseVisualStyleBackColor = false;
			_ctrlDownRow.Click += new EventHandler(OnDownRow_Click);
			_ctrlDownRow.DoubleClick += new EventHandler(OnDownRow_Click);

			((System.Windows.Forms.Button)_ctrlAddRow).Image = GetZoomBottomBitMap(global::PLOS.Gui.Core.Properties.Resources.ImgAddRow);
			((System.Windows.Forms.Button)_ctrlDelRow).Image = GetZoomBottomBitMap(global::PLOS.Gui.Core.Properties.Resources.ImgDelRow);
			((System.Windows.Forms.Button)_ctrlUpRow).Image = GetZoomBottomBitMap(global::PLOS.Gui.Core.Properties.Resources.ImgUpRow);
			((System.Windows.Forms.Button)_ctrlDownRow).Image = GetZoomBottomBitMap(global::PLOS.Gui.Core.Properties.Resources.ImgDownRow);

			this.Controls.Add(_ctrlAddRow);
			this.Controls.Add(_ctrlDelRow);
			this.Controls.Add(_ctrlUpRow);
			this.Controls.Add(_ctrlDownRow);

			InitializeControlItem();

			ResumeLayout(false);
		}

		private System.Drawing.Bitmap GetZoomBottomBitMap(System.Drawing.Bitmap SrcImg)
		{
			System.Drawing.Bitmap BmpCanvas = new System.Drawing.Bitmap(_ControlItemSize.Width, _ControlItemSize.Height);
			System.Drawing.Graphics grp = System.Drawing.Graphics.FromImage(BmpCanvas);
			grp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

			System.Drawing.Imaging.ImageAttributes ImgAttr = new System.Drawing.Imaging.ImageAttributes();
			ImgAttr.SetColorKey(System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(254, 254, 254));

			grp.DrawImage(SrcImg, 
					new System.Drawing.Rectangle(1, 1, _ControlItemSize.Width - 4, _ControlItemSize.Height - 4),
					0, 0, SrcImg.Width, SrcImg.Height, 
					System.Drawing.GraphicsUnit.Pixel, ImgAttr);
			grp.Dispose();

			BmpCanvas.MakeTransparent();

			return BmpCanvas;
		}

		/// <summary>
		/// InitializeControlItem
		/// </summary>
		private void InitializeControlItem()
		{
			int iXpos = 0;

			iXpos = _ControlItemMargin.Left;
			_ctrlAddRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _ctrlAddRow.Size.Width;
			_ctrlDelRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _ctrlDelRow.Size.Width;
			_ctrlUpRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _ctrlUpRow.Size.Width;
			_ctrlDownRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
		}

		#endregion
	}
}
