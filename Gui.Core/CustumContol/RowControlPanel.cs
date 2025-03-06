using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	public partial class RowControlPanel : Panel
	{
		#region Private Fields

		/// <summary>
		/// PictureBox
		/// </summary>
		private PictureBoxEx _picboxAddRow;
		private PictureBoxEx _picboxDelRow;
		private PictureBoxEx _picboxUpRow;
		private PictureBoxEx _picboxDownRow;

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
		public RowControlPanel()
		{
			InitializeComponentCustum();

			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="container"></param>
		public RowControlPanel(IContainer container)
		{
			container.Add(this);

			InitializeComponentCustum();

			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
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
					_picboxAddRow.Size = _ControlItemSize;
					_picboxDelRow.Size = _ControlItemSize;
					_picboxUpRow.Size = _ControlItemSize;
					_picboxDownRow.Size = _ControlItemSize;
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

		/// <summary>
		/// 
		/// </summary>
		[Category("CustomControl RowControlPanel")]
		[Localizable(false)]
		[Description("Control Item Size")]
		public Boolean EnableAdd
		{
			get
			{
				return _picboxAddRow.Visible;
			}
			set
			{
				_picboxAddRow.Visible = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("CustomControl RowControlPanel")]
		[Localizable(false)]
		[Description("Control Item Size")]
		public Boolean EnableDel
		{
			get
			{
				return _picboxDelRow.Visible;
			}
			set
			{
				_picboxDelRow.Visible = value;
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
			System.Drawing.Bitmap bmpWork;

			_ControlItemSize = new System.Drawing.Size(24, 24);

			_picboxAddRow = new PictureBoxEx();
			_picboxDelRow = new PictureBoxEx();
			_picboxUpRow = new PictureBoxEx();
			_picboxDownRow = new PictureBoxEx();

			///////////////////
			//((System.ComponentModel.ISupportInitialize)_picboxAddRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_picboxDelRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_picboxUpRow).BeginInit();
			//((System.ComponentModel.ISupportInitialize)_picboxDownRow).BeginInit();
			SuspendLayout();

			_picboxAddRow.Size = _ControlItemSize;
			_picboxAddRow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_picboxAddRow.Name = "ImgAddRow";
			_picboxAddRow.TabStop = true;
			_picboxAddRow.SizeMode = PictureBoxSizeMode.Zoom;
			_picboxAddRow.Click += new EventHandler(OnAddRow_Click);
			_picboxAddRow.DoubleClick += new EventHandler(OnAddRow_Click);

			_picboxDelRow.Size = _ControlItemSize;
			_picboxDelRow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_picboxDelRow.Name = "ImgDelRow";
			_picboxDelRow.TabStop = true;
			_picboxDelRow.SizeMode = PictureBoxSizeMode.Zoom;
			_picboxDelRow.Click += new EventHandler(OnDelRow_Click);
			_picboxDelRow.DoubleClick += new EventHandler(OnDelRow_Click);

			_picboxUpRow.Size = _ControlItemSize;
			_picboxUpRow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_picboxUpRow.Name = "ImgUpRow";
			_picboxUpRow.TabStop = true;
			_picboxUpRow.SizeMode = PictureBoxSizeMode.Zoom;
			_picboxUpRow.Click += new EventHandler(OnUpRow_Click);
			_picboxUpRow.DoubleClick += new EventHandler(OnUpRow_Click);

			_picboxDownRow.Size = _ControlItemSize;
			_picboxDownRow.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_picboxDownRow.Name = "ImgDownRow";
			_picboxDownRow.TabStop = true;
			_picboxDownRow.SizeMode = PictureBoxSizeMode.Zoom;
			_picboxDownRow.Click += new EventHandler(OnDownRow_Click);
			_picboxDownRow.DoubleClick += new EventHandler(OnDownRow_Click);

			// Default Image Set
			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgAddRow);
			bmpWork.MakeTransparent();
			_picboxAddRow.Image = bmpWork;
			_picboxAddRow.LeaveImage = bmpWork;
			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgAddRow_MouseEnter);
			bmpWork.MakeTransparent(bmpWork.GetPixel(1,1));
			_picboxAddRow.EnterImage = bmpWork;

			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgDelRow);
			bmpWork.MakeTransparent();
			_picboxDelRow.Image = bmpWork;
			_picboxDelRow.LeaveImage = bmpWork;
			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgDelRow_MouseEnter);
			bmpWork.MakeTransparent(bmpWork.GetPixel(1, 1));
			_picboxDelRow.EnterImage = bmpWork;

			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgUpRow);
			bmpWork.MakeTransparent();
			_picboxUpRow.Image = bmpWork;
			_picboxUpRow.LeaveImage = bmpWork;
			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgUpRow_MouseEnter);
			bmpWork.MakeTransparent(bmpWork.GetPixel(1, 1));
			_picboxUpRow.EnterImage = bmpWork;

			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgDownRow);
			bmpWork.MakeTransparent();
			_picboxDownRow.Image = bmpWork;
			_picboxDownRow.LeaveImage = bmpWork;
			bmpWork = new System.Drawing.Bitmap(global::PLOS.Gui.Core.Properties.Resources.ImgDownRow_MouseEnter);
			bmpWork.MakeTransparent(bmpWork.GetPixel(1, 1));
			_picboxDownRow.EnterImage = bmpWork;

			this.Controls.Add(_picboxAddRow);
			this.Controls.Add(_picboxDelRow);
			this.Controls.Add(_picboxUpRow);
			this.Controls.Add(_picboxDownRow);

			InitializeControlItem();

			///////////////////
			////_TogglePanel.ResumeLayout(false);
			//((System.ComponentModel.ISupportInitialize)_picboxAddRow).EndInit();
			ResumeLayout(false);
		}

		/// <summary>
		/// InitializeControlItem
		/// </summary>
		private void InitializeControlItem()
		{
			int iXpos = 0;

			iXpos = _ControlItemMargin.Left;
			_picboxAddRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _picboxAddRow.Size.Width;
			_picboxDelRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _picboxDelRow.Size.Width;
			_picboxUpRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
			iXpos += _ControlItemMargin.Left + _ControlItemMargin.Right + _picboxUpRow.Size.Width;
			_picboxDownRow.Location = new System.Drawing.Point(iXpos, _ControlItemMargin.Top);
		}

		#endregion
	}
}
