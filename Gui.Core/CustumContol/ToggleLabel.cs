using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PLOS.Gui.Core.CustumContol
{
	public partial class ToggleLabel : Panel	//Component
	{
		#region Private Fields

		private readonly int C_ControlItemWidth = 20;

		/// <summary>
		/// Anti Alias Label
		/// </summary>
		private AntiAliasLabel _Label;

		/// <summary>
		/// Toggle 表示 Panel
		/// </summary>
		private Panel _TogglePanel;

		/// <summary>
		/// Row Control Panel
		/// </summary>
		private RowControlPanel _RowControlPanel;

		/// <summary>
		/// Toggle Image PictureBox
		/// </summary>
		private PictureBox _ToggleImagePictureBox;

		/// <summary>
		/// Toggle Status
		/// </summary>
		private Boolean _ToggleStatus;

		/// <summary>
		/// Image @Status
		/// </summary>
		private System.Drawing.Image _TrueImage = null;
		private System.Drawing.Image _FalseImage = null;

		/// <summary>
		/// Row Control Panel Visible
		/// <remarks>
		/// (Default = false)
		/// </remarks>
		/// </summary>
		private Boolean _RowControlPanelVisible = false;

		/// <summary>
		/// ReadOnly 
		/// <remarks>
		/// (Default = true)
		/// </remarks>
		/// </summary>
		private Boolean _ReadOnly = true;

        #endregion

		#region EventHandler(Delegate)

		/// <summary>
		/// EventHandler ToggleClicked
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl ToggleLabel")]
		public event EventHandler ToggleClicked;

		/// <summary>
		/// EventHandler Toggle Changed
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl ToggleLabel")]
		public event EventHandler ToggleChanged;

		/// <summary>
		/// EventHandler AddRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler RowAdd;

		/// <summary>
		/// EventHandler DelRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler RowDelete;

		/// <summary>
		/// EventHandler UpRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler RowMoveUp;

		/// <summary>
		/// EventHandler DownRow Click
		/// </summary>
		[Browsable(true)]
		[Category("CustomControl RowControlPanel")]
		public event EventHandler RowMoveDown;

		#endregion

        #region Constructors and Destructor
 
		/// <summary>
		/// Constructors
		/// </summary>
		public ToggleLabel()
		{
			InitializeComponentCustum();

			InitializeComponent();
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="container"></param>
		public ToggleLabel(IContainer container)
		{
			container.Add(this);

			InitializeComponentCustum();

			InitializeComponent();
		}

        #endregion

        #region Event

		/// <summary>
		/// On Toggle Clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnToggleClicked(object sender, EventArgs eArgs)
		{
			ToggleStatus = !ToggleStatus;

			if (ToggleClicked != null)
			{
				foreach (EventHandler evhd in ToggleClicked.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}

			if (ToggleChanged != null)
			{
				foreach (EventHandler evhd in ToggleChanged.GetInvocationList())
				{
					evhd(this, eArgs);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			SetPictureAtStatus();
		}

		/// <summary>
		/// OnSizeChanged
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			InitializeControlPanel();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		private void OnAddRow_Click(object sender, EventArgs eArgs)
		{
			EventHandler handler = RowAdd;
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
			EventHandler handler = RowDelete;
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
			EventHandler handler = RowMoveUp;
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
			EventHandler handler = RowMoveDown;
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
		/// AutoSize Override
		/// </summary>
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				_Label.AutoSize = value;
				_TogglePanel.AutoSize = value;

				//this.PreferredSize = new System.Drawing.Size(_Label.Size.Width + _TogglePanel.Width, Math.Max(_Label.Height, _TogglePanel.Height));

				base.AutoSize = value;
			}
		}

		/// <summary>
		/// Toggle Status
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Description("Toggle Status")]
		public Boolean ToggleStatus
		{
			get
			{
				return _ToggleStatus;
			}
			set
			{
				_ToggleStatus = value;

				SetPictureAtStatus();
			}
		}

		/// <summary>
		/// Toggle True's Image
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Localizable(false)]
		[Description("Toggle True's Image")]
		public System.Drawing.Image TrueImage
		{
			get
			{
				return _TrueImage;
			}
			set
			{
				if (value != null)
				{
					_TrueImage = value;
					SetPictureAtStatus();
				}
			}
		}

		/// <summary>
		/// Toggle True's Image
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Localizable(false)]
		[Description("Toggle False's Image")]
		public System.Drawing.Image FalseImage
		{
			get
			{
				return _FalseImage;
			}
			set
			{
				if (value != null)
				{
					_FalseImage = value;
					SetPictureAtStatus();
				}
			}
		}

		/// <summary>
		/// Toggle Width
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		//[Localizable(true)]
		[Description("Toggle Width")]
		public int ToggleWidth
		{
			get
			{
				return _TogglePanel.Size.Width;
			}
			set
			{
				if (value >= 0)
				{
					_TogglePanel.Size = new System.Drawing.Size(value, _TogglePanel.Size.Height);
				}
			}
		}

		/// <summary>
		/// Toggle Padding
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Description("Toggle Padding")]
		public Padding TogglePadding
		{
			get
			{
				return _TogglePanel.Padding;
			}
			set
			{
				_TogglePanel.Padding = value;
			}
		}

		/// <summary>
		/// Toggle Image SizeMode
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Description("Toggle Image SizeMode")]
		public System.Windows.Forms.PictureBoxSizeMode ToggleSizeMode
		{
			get
			{
				return _ToggleImagePictureBox.SizeMode;
			}
			set
			{
				_ToggleImagePictureBox.SizeMode = value;
			}
		}

		/// <summary>
		/// LabelText
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		public String LabelText
		{
			get
			{
				return _Label.Text;
			}
			set
			{
				_Label.Text = value;
			}
		}

		/// <summary>
		/// TextAlign
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Localizable(true)]
		[Description("LabelTextAlignDescr")]
		public System.Drawing.ContentAlignment LabelTextAlign
		{
			get
			{
				return _Label.TextAlign;
			}
			set
			{
				_Label.TextAlign = value;
			}
		}

		//
		// 概要:
		//     この System.Drawing.Graphics に関連付けられているテキストのレンダリング モードを取得または設定します。
		//
		// 戻り値:
		//     System.Drawing.Text.TextRenderingHint 値の 1 つ。
		[Category("CustomControl ToggleLabel")]
		public System.Drawing.Text.TextRenderingHint LabelTextRenderingHint
		{
			get
			{
				return _Label.TextRenderingHint;
			}
			set
			{
				_Label.TextRenderingHint = value;
			}
		}

		/// <summary>
		/// Label RotateAngle
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		public float LabelRotateAngle
		{
			get
			{
				return _Label.RotateAngle;
			}
			set
			{
				_Label.RotateAngle = value;
			}
		}

		/// <summary>
		/// Label Translate X
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		public float LabelTranslateX
		{
			get
			{
				return _Label.TranslateX;
			}
			set
			{
				_Label.TranslateX = value;
			}
		}

		/// <summary>
		/// Label Translate Y
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		public float LabelTranslateY
		{
			get
			{
				return _Label.TranslateY;
			}
			set
			{
				_Label.TranslateY = value;
			}
		}

		/// <summary>
		/// BaseColor
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Description("Baseed Color")]
		public System.Drawing.Color BaseColor
		{
			get
			{
				return _Label.BackColor;
			}
			set
			{
				_Label.BackColor = value;
				_TogglePanel.BackColor = value;
				_RowControlPanel.BackColor = value;
			}
		}

		/// <summary>
		/// ReadOnly Status
		/// </summary>
		[Category("CustomControl ToggleLabel")]
		[Description("ReadOnly")]
		public Boolean ReadOnly
		{
			get
			{
				return _ReadOnly;
			}
			set
			{
				_ReadOnly = value;
				_RowControlPanel.Visible = !_ReadOnly;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// InitializeComponent Custum
		/// </summary>
		private void InitializeComponentCustum()
		{
			_Label = new AntiAliasLabel();
			_TogglePanel = new System.Windows.Forms.Panel();
			_ToggleImagePictureBox = new PictureBox();
			_RowControlPanel = new RowControlPanel();
			// 2017/06 Add Delete 無
			//_RowControlPanel.EnableAdd = false;
			//_RowControlPanel.EnableDel = false;

			// Event Handler Setting
			_RowControlPanel.AddRowClick += new EventHandler(OnAddRow_Click);
			_RowControlPanel.DelRowClick += new EventHandler(OnDelRow_Click);
			_RowControlPanel.UpRowClick += new EventHandler(OnUpRow_Click);
			_RowControlPanel.DownRowClick += new EventHandler(OnDownRow_Click);

			///////////////////
			_TogglePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_ToggleImagePictureBox).BeginInit();
			SuspendLayout();

			///////////////////
			_Label.Dock = System.Windows.Forms.DockStyle.Fill;
			_Label.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			_Label.Name = "_Label";
			_Label.TabIndex = 0;
			_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			_Label.Click += new System.EventHandler(this.OnToggleClicked);
			_Label.DoubleClick += new System.EventHandler(this.OnToggleClicked);

			_TogglePanel.Controls.Add(_ToggleImagePictureBox);
			_TogglePanel.Dock = System.Windows.Forms.DockStyle.Left;
			_TogglePanel.Location = new System.Drawing.Point(0, 0);
			_TogglePanel.Name = "CheckPanel";
			//_TogglePanel.Size = new System.Drawing.Size(24, 8);
			_TogglePanel.Padding = new System.Windows.Forms.Padding(1);

			_ToggleImagePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			_ToggleImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_ToggleImagePictureBox.Name = "ToggleImage";
			_ToggleImagePictureBox.TabStop = false;
			_ToggleImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
			_ToggleImagePictureBox.Click += new System.EventHandler(this.OnToggleClicked);
			_ToggleImagePictureBox.DoubleClick += new System.EventHandler(this.OnToggleClicked);

			_RowControlPanel.Dock = System.Windows.Forms.DockStyle.Right;
			_RowControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_RowControlPanel.Name = "RowControlPanel";
			_RowControlPanel.TabStop = false;
			_RowControlPanel.ControlItemSize = new System.Drawing.Size(C_ControlItemWidth, C_ControlItemWidth);
			//((RowControlPanel)_RowControlPanel).ControlItemMargin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			//_RowControlPanel.Size = new System.Drawing.Size(
			//        ((RowControlPanel)_RowControlPanel).ControlItemSize.Width * 4 +
			//        ((RowControlPanel)_RowControlPanel).ControlItemMargin.Left * 4 +
			//        ((RowControlPanel)_RowControlPanel).ControlItemMargin.Right * 4, this.Size.Height);
			_RowControlPanel.BackColor = this.BackColor;
			_RowControlPanel.Visible = _RowControlPanelVisible;

			// Default Image Set
			_TrueImage = global::PLOS.Gui.Core.Properties.Resources.ToggleTrueImage;
			_FalseImage = global::PLOS.Gui.Core.Properties.Resources.ToggleFalseImage;

			this.Controls.Add(_Label);
			this.Controls.Add(_RowControlPanel);
			this.Controls.Add(_TogglePanel);

			this.Size = new System.Drawing.Size(this.Size.Width, 24);

			InitializeControlPanel();

			///////////////////
			_TogglePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_ToggleImagePictureBox).EndInit();
			ResumeLayout(false);
		}

		/// <summary>
		/// 
		/// </summary>
		private void SetPictureAtStatus()
		{
			if (_ToggleStatus)
			{
				if (_TrueImage != null)
				{
					SetPictureBoxImage(_TrueImage);
				}
			}
			else
			{
				if (_FalseImage != null)
				{
					SetPictureBoxImage(_FalseImage);
				}
			}
		}

		/// <summary>
		/// Set PictureBox Image
		/// </summary>
		/// <param name="image"></param>
		private void SetPictureBoxImage(System.Drawing.Image image)
		{
			System.Drawing.Bitmap bmpWork = new System.Drawing.Bitmap(image);

			bmpWork.MakeTransparent();

			_ToggleImagePictureBox.Image = bmpWork;
		}

		/// <summary>
		/// InitializeControlPanel
		/// </summary>
		private void InitializeControlPanel()
		{
			_RowControlPanel.ControlItemMargin = new System.Windows.Forms.Padding(2,
				Convert.ToInt16((this.Height - _RowControlPanel.ControlItemSize.Height)  / 2), 2, 
				Convert.ToInt16((this.Height - _RowControlPanel.ControlItemSize.Height)  / 2));
			_RowControlPanel.Size = new System.Drawing.Size(
					_RowControlPanel.ControlItemSize.Width * 4 +
					_RowControlPanel.ControlItemMargin.Left * 4 +
					_RowControlPanel.ControlItemMargin.Right * 4, this.Size.Height);
		}
		#endregion

	}
}
