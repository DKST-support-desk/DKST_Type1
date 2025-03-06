using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
	public partial class UCRichMessagePanel : PLOS.Gui.Core.Base.UCBase
	{
		#region Private Fields

		/// <summary>
		/// 同期ｵﾌﾞｼﾞｪｸﾄ
		/// </summary>
		internal static object _asynObject = new object();

		/// <summary>
		/// 
		/// </summary>
		private Control _OwnerControl = null;
		/// <summary>
		/// Invisible Timer
		/// </summary>
		private System.Timers.Timer _InvisibleTimer;
		/// <summary>
		/// Timer Period
		/// </summary>
		private UInt16 _InvisibleTimerPeriod = 3000;
		/// <summary>
		/// 
		/// </summary>
		private Color _RectangleLineColor = System.Drawing.Color.FromArgb(20, 20, 40);
		/// <summary>
		/// 
		/// </summary>
		private Color _RectangleFillColor = System.Drawing.Color.FromArgb(40, 50, 100);
		/// <summary>
		/// 
		/// </summary>
		private int _LinePenWidth = 6;

		/// <summary>
		/// RoundSize
		/// </summary>
		private int _RoundSize = 1;

		///// <summary>
		///// Invisible Time Out ManualResetEvent
		///// </summary>
		//private static System.Threading.ManualResetEvent _InvisibleTimeOutEvent = null;

		#endregion

		#region EventHandler(Delegate)
		/// <summary>
		/// ShowMessageDelegate
		/// </summary>
		/// <param name="text"></param>
		/// <param name="icon"></param>
		/// <param name="AutoInvisibleTimer"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		private delegate void ShowMessageDelegate(String text,
			System.Windows.Forms.MessageBoxIcon icon,
			UInt16 AutoInvisibleTimer, int Width, int Height);

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public UCRichMessagePanel()
		{
			// Supports TransparentBackColor
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			// Used DoubleBuffer
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.BackColor = Color.Transparent;

			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;

			lblMessage.BackColor = _RectangleFillColor;

			// Timer Setting
			_InvisibleTimer = new System.Timers.Timer();
			_InvisibleTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer_Elapsed);
			//new System.Timers.ElapsedEventHandler(CameraOrgPosTimer_ElapsedMethod);
			_InvisibleTimer.AutoReset = false;			// true:周期起動 false:単発起動
			_InvisibleTimer.Interval = _InvisibleTimerPeriod;	// 周期設定
			_InvisibleTimer.Stop();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctrl"></param>
		public UCRichMessagePanel(Control ctrl)
			: this()
		{
			OwnerControl = ctrl;
		}

		#endregion

		#region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_InvisibleTimer.Stop();

			CloseMessage();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlClick(object sender, EventArgs e)
		{
			CloseMessage();
		}

		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			int iInsetWidth = _LinePenWidth / 2 + _LinePenWidth % 2;

			Rectangle targetRectangle = new Rectangle(
								iInsetWidth, iInsetWidth, 
								Width - iInsetWidth * 2,
								Height - iInsetWidth * 2);

			Size roundSize = new Size(_RoundSize, _RoundSize);

			// Fill Rounded Rectangle Draw
			FillRoundedRectangle(e.Graphics, targetRectangle,
				new Pen(_RectangleLineColor, _LinePenWidth),
				new SolidBrush(_RectangleLineColor), new SolidBrush(_RectangleFillColor),
				roundSize);
		}

		#endregion

		#region Property

		/// <summary>
		/// Owner Control
		/// </summary>
		public Control OwnerControl
		{
			set
			{
				_OwnerControl = value;
			}
			get
			{
				return _OwnerControl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color RectangleLineColor
		{
			get
			{
				return this._RectangleLineColor;
			}
			set
			{
				this._RectangleLineColor = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color RectangleFillColor
		{
			get
			{
				return this._RectangleFillColor;
			}
			set
			{
				this._RectangleFillColor = value;
				lblMessage.BackColor = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color TextForeColor
		{
			get
			{
				return lblMessage.ForeColor;
			}
			set
			{
				lblMessage.ForeColor = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int LineWidth
		{
			get
			{
				return _LinePenWidth;
			}
			set
			{
				if(value > 0)
					_LinePenWidth = value;
				else
					_LinePenWidth = 6;
			}
		}

		/// <summary>
		/// TimerPeriod
		/// </summary>
		public UInt16 TimerPeriod
		{
			get
			{
				return _InvisibleTimerPeriod;
			}
			set
			{
				_InvisibleTimerPeriod = value;
				if (_InvisibleTimer != null && _InvisibleTimerPeriod > 0)
				{
					_InvisibleTimer.Interval = _InvisibleTimerPeriod;
				}
			}
		}

		public String MessageText
		{
			set
			{
				lblMessage.Text = value;
			}
		}

		/// <summary>
		/// RoundSize
		/// </summary>
		[Category("CustomUserControl")]
		[DefaultValue(null)]
		[Browsable(true)]
		public int RoundSize
		{
			set { _RoundSize = value; }
			get { return _RoundSize; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="msgIcon"></param>
		/// <param name="AutoInvisibleTimer"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		public void ShowMessage(String text, System.Windows.Forms.MessageBoxIcon msgIcon = MessageBoxIcon.None, UInt16 AutoInvisibleTimer = 3000, int Width = 0, int Height = 0)
		{
			if (_OwnerControl == null) return;
			if (_OwnerControl.InvokeRequired)
			{
				_OwnerControl.Invoke(new ShowMessageDelegate(ShowMessage), text, msgIcon, AutoInvisibleTimer, Width, Height);
				return;
			}

			Bitmap canvas;// = new Bitmap(picIcon.Width, picIcon.Height);
			Graphics grp;
			int IconMargin = 8;
			System.Drawing.Icon sysIcon = null;

			switch (msgIcon)
			{
				case MessageBoxIcon.Error:			// = Hand = Stop
					sysIcon = SystemIcons.Error;
					picIcon.Width = sysIcon.Width + IconMargin * 2;
					break;
				case MessageBoxIcon.Information:	// = Asterisk:
					sysIcon = SystemIcons.Information;
					picIcon.Width = sysIcon.Width + IconMargin * 2;
					break;
				case MessageBoxIcon.Question:
					sysIcon = SystemIcons.Question;
					picIcon.Width = sysIcon.Width + IconMargin * 2;
					break;
				case MessageBoxIcon.Warning:		// = Exclamation
					sysIcon = SystemIcons.Warning;
					picIcon.Width = sysIcon.Width + IconMargin * 2;
					break;
				case MessageBoxIcon.None:
				default:
					picIcon.Width = 0;
					break;
			}

			_InvisibleTimer.Stop();
			CloseMessage();

			MessageText = text;
			lblMessage.AutoSize = true;
			if (Width == 0)
			{
				Width = lblMessage.Size.Width + Padding.Left + Padding.Right + lblMessage.Margin.Left + lblMessage.Margin.Right + picIcon.Width;
			}
			if (Height == 0)
			{
				Height = lblMessage.Size.Height + Padding.Top + Padding.Bottom + lblMessage.Margin.Top + lblMessage.Margin.Bottom +
					pnlOption.Size.Height;
			}
			lblMessage.AutoSize = false;
			Size = new Size(Width, Height);

			if (sysIcon != null)
			{
				canvas = new Bitmap(sysIcon.Width + IconMargin * 2, picIcon.Height);
				grp = Graphics.FromImage(canvas);				//ImageオブジェクトのGraphicsオブジェクト作成

				int iIconPosy = (picIcon.Height - sysIcon.Height) / 2;
				grp.DrawIcon(sysIcon, IconMargin, iIconPosy > 0 ? iIconPosy : 0);
				//grp.DrawIcon(sysIcon, IconMargin, 0);
				picIcon.Image = canvas;
			}

			// メッセージ表示位置
			Location = new Point((this._OwnerControl.Size.Width - this.Size.Width) / 2, (this._OwnerControl.Size.Height - this.Size.Height) / 2);

			// Show this user control
			_OwnerControl.Controls.Add(this);
			BringToFront();
			Visible = true;

			TimerPeriod = AutoInvisibleTimer;
			if (AutoInvisibleTimer > 0)
			{
				_InvisibleTimer.Start();
			}
			else
			{
				_InvisibleTimer.Stop();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enabled"></param>
		public void CloseMessage()
		{
			try
			{
				if (_OwnerControl == null) return;
				if (_OwnerControl.InvokeRequired)
				{
					_OwnerControl.Invoke(new VoidDelegate(CloseMessage));
					return;
				}
			}
			catch { }

			try
			{
				if (_OwnerControl.Controls.Contains(this))
				{
					_OwnerControl.Controls.Remove(this);
				}
			}
			catch { }

			try
			{
				MessageText = "";
				Visible = false;
			}
			catch { }
		}

		#endregion

		#region Protected Methods

		#endregion

		#region Private Methods

		/// <summary>
		/// Fill Rounded Rectangle
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="targetRectangle"></param>
		/// <param name="lineDrawsPen"></param>
		/// <param name="rectLineBrush"></param>
		/// <param name="size"></param>
		private void FillRoundedRectangle(Graphics graphics,
				Rectangle targetRectangle, 
				Pen lineDrawsPen,
				Brush rectLineBrush, Brush rectFillBrush,
				Size roundSize)
		{
			System.Drawing.Drawing2D.SmoothingMode smoothingMode_Push = graphics.SmoothingMode;
			System.Drawing.Drawing2D.PixelOffsetMode pixelOffsetMode_Push = graphics.PixelOffsetMode;
			System.Drawing.Drawing2D.GraphicsPath graphicPath;

			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.PixelOffsetMode =	System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			graphicPath = new System.Drawing.Drawing2D.GraphicsPath();
			graphicPath.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
			graphicPath.AddArc(targetRectangle.Right - roundSize.Width, targetRectangle.Top, roundSize.Width, roundSize.Height, 270, 90);
			graphicPath.AddArc(targetRectangle.Right - roundSize.Width, targetRectangle.Bottom - roundSize.Height, roundSize.Width, roundSize.Height, 0, 90);
			graphicPath.AddArc(targetRectangle.Left, targetRectangle.Bottom - roundSize.Height, roundSize.Width, roundSize.Height, 90, 90);
			graphicPath.AddArc(targetRectangle.Left, targetRectangle.Top, roundSize.Width, roundSize.Height, 180, 90);
			graphicPath.AddArc(targetRectangle.Right - roundSize.Width, targetRectangle.Top, roundSize.Width, roundSize.Height, 270, 90);

			graphics.DrawPath(lineDrawsPen, graphicPath);
			graphics.FillPath(rectFillBrush, graphicPath);

			// Pop Saved Data
			graphics.SmoothingMode = smoothingMode_Push;
			graphics.PixelOffsetMode = pixelOffsetMode_Push;
		}

		#endregion
	}
}
