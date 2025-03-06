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
	public partial class FormSplashBase : PLOS.Gui.Core.Base.FormBase
	{
		#region Static Fields
		/// <summary>
		/// Self instance
		/// </summary>
		protected static FormSplashBase _SplashForm = null;

		/// <summary>
		/// Application Main Form
		/// </summary>
		protected static Form _ApplicationMainForm = null;

		/// <summary>
		/// Thread Object
		/// </summary>
		protected static System.Threading.Thread _SplashThread = null;

		/// <summary>
		/// Asyn Object
		/// </summary>
		protected static readonly object _AsynObject = new object();

		/// <summary>
		/// Splash Shown ManualResetEvent
		/// </summary>
		private static System.Threading.ManualResetEvent _SplashShownEvent = null;

		///// <summary>
		///// Minimum Splash Screen DisplayTime ManualResetEvent
		///// </summary>
		//private static System.Threading.ManualResetEvent _MinimumSplashDisplayTimeEvent = null;

		/// <summary>
		/// Splash Display Timer
		/// </summary>
		private static System.Timers.Timer _SplashDisplayTimer = null;

		/// <summary>
		/// FadeIn / FadeOut Interval Timer
		/// </summary>
		private static System.Timers.Timer _FadeTimer = null;

		#endregion

		#region Static Methods

		#region Static Public Methods

		#region Show Splash Methods

		/// <summary>
		/// Show Splash
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="appMainForm">Application Main Form Object</param>
		public static void ShowSplash<T>(Form appMainForm)
			where T : FormSplashBase, new()
		{
			lock (_AsynObject)
			{
				if (_SplashForm != null || _SplashThread != null) return;

				_ApplicationMainForm = appMainForm;

				// Application Main Form の"Activated" Event で SplashWindowForm を Close
				if (_ApplicationMainForm != null)
				{
					_ApplicationMainForm.Activated += new EventHandler(OnApplicationMainFormActivated);
				}

				// Create Wait Handler
				_SplashShownEvent = new System.Threading.ManualResetEvent(false);

				//// For Wait Timer Flag
				//_MinimumSplashDisplayTimeEvent = new System.Threading.ManualResetEvent(false);

				_SplashThread =
					new System.Threading.Thread(new System.Threading.ThreadStart(StartSplashThread<T>));
				_SplashThread.Name = "FormSplashWindowBase";
				_SplashThread.IsBackground = true;
				_SplashThread.SetApartmentState(System.Threading.ApartmentState.STA);

				// Start Thread 
				_SplashThread.Start();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void ShowSplash<T>()
			where T : FormSplashBase, new()
		{
			ShowSplash<T>(null);
		}

		#endregion

		#region Close Splash Methods

		/// <summary>
		/// Close Splash
		/// </summary>
		public static void CloseSplash()
		{
			lock (_AsynObject)
			{
				if (_SplashThread == null) return;

				if (_ApplicationMainForm != null)
				{
					_ApplicationMainForm.Activated -= new EventHandler(OnApplicationMainFormActivated);
				}

				// Wait Splash Shown Event 
				if (_SplashShownEvent != null)
				{
					_SplashShownEvent.WaitOne();
					_SplashShownEvent.Close();
					_SplashShownEvent = null;
				}

				//// Wait Minimum Splash Screen DisplayTime ManualResetEvent
				//if (_MinimumSplashScreenDisplayTime > 0 && _MinimumSplashDisplayTimeEvent != null)
				//{
				//    _MinimumSplashDisplayTimeEvent.WaitOne();
				//    _MinimumSplashDisplayTimeEvent.Close();
				//    _MinimumSplashDisplayTimeEvent = null;
				//}

				// Activate "ApplicationMainForm"
				ActivateApplicationMainForm();
				// Close Splash Form
				CloseSplashForm();

				//_SplashWindow.Owner.RemoveOwnedForm(_ApplicationMainForm);
				if (_FadeTimer != null)
				{
					_FadeTimer.Stop();
					_FadeTimer = null;
				}
				if (_SplashDisplayTimer != null)
				{
					_SplashDisplayTimer.Stop();
					_SplashDisplayTimer = null;
				}

				_SplashForm = null;
				_SplashThread = null;

				// Activate "ApplicationMainForm"
				ActivateApplicationMainForm();

				_ApplicationMainForm = null;
			}
		}

		#endregion

		#endregion

		#region Static Private Methods

		/// <summary>
		/// Start Splash Thread
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private static void StartSplashThread<T>()
			where T : FormSplashBase, new()
		{
			_SplashForm = new T();

			if (_SplashForm != null)
			{
				// Add Owner Form
				//_SplashForm.AddOwnedForm(_ApplicationMainForm);

				_SplashForm._Opacity = _SplashForm.Opacity;

				// Show Splash Form
				Application.Run(_SplashForm);
			}
		}

		/// <summary>
		/// Close Splash Form with InvokeRequired
		/// </summary>
		private static void CloseSplashForm()
		{
			if (_SplashForm == null) return;

			// For multi thread 
			// Check InvokeRequired
			if (_SplashForm.InvokeRequired)
			{
				_SplashForm.Invoke(new MethodInvoker(CloseSplashForm));
				return;
			}

			if (!_SplashForm.IsDisposed) _SplashForm.Close();
		}

		/// <summary>
		/// Activate "ApplicationMainForm" with InvokeRequired
		/// </summary>
		private static void ActivateApplicationMainForm()
		{
			if (_ApplicationMainForm == null) return;

			if (_ApplicationMainForm.InvokeRequired)
			{
				_ApplicationMainForm.Invoke(new MethodInvoker(ActivateApplicationMainForm));
				return;
			}

			if (!_ApplicationMainForm.IsDisposed) _ApplicationMainForm.Activate();
		}

		/// <summary>
		/// Set Splash Display Timer
		/// </summary>
		private static void SetSplashDisplayTimer()
		{
			if (_SplashForm == null) return;

			if (_SplashForm.MinimumSplashScreenDisplayTime > 0 && (_SplashDisplayTimer == null))
			{
				// Wait Timer Setting
				_SplashDisplayTimer = new System.Timers.Timer();
				_SplashDisplayTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnDisplayTimeup_Elapsed);
				_SplashDisplayTimer.AutoReset = false;			// true:周期起動 false:単発起動
				_SplashDisplayTimer.Interval = _SplashForm.MinimumSplashScreenDisplayTime;	// 周期設定
				_SplashDisplayTimer.Start();
			}
		}

		#endregion

		#endregion

		#region Static Event

		/// <summary>
		/// Application Main Form Activated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnApplicationMainFormActivated(object sender, EventArgs e)
		{
			if (_SplashForm != null && _SplashForm.MinimumSplashScreenDisplayTime > 0)
			{
				SetSplashDisplayTimer();
			}
			else
			{
				// Close Splash
				CloseSplash();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnDisplayTimeup_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Wait Timer Stop
			_SplashDisplayTimer.Stop();
			_SplashDisplayTimer = null;

			if (_SplashForm != null && _SplashForm.FadeOutTime > 0)
			{
				if (_FadeTimer == null)
				{
					// Fade Timer Setting
					_FadeTimer = new System.Timers.Timer();
					_FadeTimer.Elapsed += new System.Timers.ElapsedEventHandler(_SplashForm.OnFadeOutTimer_Elapsed);
					_FadeTimer.AutoReset = true;			// true:周期起動 false:単発起動
					_FadeTimer.Interval = _SplashForm._FadeTimerPeriod;	// 周期設定
					_FadeTimer.Start();
				}
			}
			else
			{
				// Close Splash
				CloseSplash();
			}
		}

		#endregion

		#region Static Property
		/// <summary>
		/// FormSplashWindowBase
		/// </summary>
		public static FormSplashBase Form
		{
			get { return _SplashForm; }
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Minimum Splash Screen DisplayTime (Mili second)
		/// </summary>
		private int _MinimumSplashScreenDisplayTime = 2000;

		/// <summary>
		/// FadeIn / FadeOut Interval Timer Period
		/// </summary>
		private UInt16 _FadeTimerPeriod = 20;

		/// <summary>
		/// Fade In Time (Mili second)
		/// </summary>
		private UInt16 _FadeInTime = 200;

		/// <summary>
		/// Fade Out Time (Mili second)
		/// </summary>
		private UInt16 _FadeOutTime = 200;

		/// <summary>
		/// _Opacity
		/// </summary>
		private double _Opacity = 0.0D;
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public FormSplashBase()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;

			TopMost = true;

			// Add "OnClicked" Event handler
			Click += new EventHandler(OnSplashFormClicked);

			// Add "OnActivated" Event handler
			Activated += new EventHandler(OnSplashFormActivated);
		}

		#endregion

		#region Event

		/// <summary>
		/// On Clicked Splash Form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSplashFormClicked(object sender, EventArgs e)
		{
			// Close Splash Form
			CloseSplash();
		}

		/// <summary>
		/// On Activated Splash Form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSplashFormActivated(object sender, EventArgs e)
		{
			_SplashForm.Activated -= new EventHandler(OnSplashFormActivated);

			// Waiting "Splash Form" Shown At ManualResetEvent
			if (_SplashShownEvent != null)
			{
				_SplashShownEvent.Set();

				if ((_SplashForm != null && _SplashForm.MinimumSplashScreenDisplayTime > 0) && (_SplashDisplayTimer == null))
				{
					SetSplashDisplayTimer();
				}
			}

			// FadeIn
			if (_FadeInTime > 0)
			{
				if (_FadeTimer == null)
				{
					// Fade Timer Setting
					_FadeTimer = new System.Timers.Timer();
					_FadeTimer.Elapsed += new System.Timers.ElapsedEventHandler(_SplashForm.OnFadeInTimer_Elapsed);
					_FadeTimer.AutoReset = true;			// true:周期起動 false:単発起動
					_FadeTimer.Interval = _SplashForm._FadeTimerPeriod;	// 周期設定
					_FadeTimer.Start();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFadeInTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_Opacity += 1D / (_FadeInTime / _FadeTimerPeriod);
			if (_Opacity > 1)
			{
				try
				{
					_FadeTimer.Stop();
					_FadeTimer = null;
				}
				catch { }
				finally
				{
					_Opacity = 1;
					SetOpacity(_Opacity);
				}
			}
			else
			{
				try
				{
					SetOpacity(_Opacity);
					_FadeTimer.Start();
				}
				catch
				{
					_Opacity = 1;
					SetOpacity(_Opacity);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFadeOutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_Opacity -= 1D / (_FadeOutTime / _FadeTimerPeriod);
			if (_Opacity < 0)
			{
				_FadeTimer.Stop();
				_FadeTimer = null;

				_Opacity = 0D;
				SetOpacity(_Opacity);

				// Close Splash
				CloseSplash();
			}
			else
			{
				try
				{
					SetOpacity(_Opacity);
					_FadeTimer.Start();
				}
				catch
				{
					// Close Splash
					CloseSplash();
				}
			}
		}

		#endregion

		#region Property

		/// <summary>
		/// Minimum Splash Screen DisplayTime (Mili second)
		/// </summary>
		[Category("Splash")]
		public int MinimumSplashScreenDisplayTime
		{
			get { return _MinimumSplashScreenDisplayTime; }
			set { _MinimumSplashScreenDisplayTime = value; }
		}

		/// <summary>
		/// Fade In Time (Mili second)
		/// </summary>
		[Category("Splash")]
		public UInt16 FadeInTime
		{
			get { return _FadeInTime; }
			set { _FadeInTime = value; }
		}

		/// <summary>
		/// Fade Out Time (Mili second)
		/// </summary>
		[Category("Splash")]
		public UInt16 FadeOutTime
		{
			get { return _FadeOutTime; }
			set { _FadeOutTime = value; }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Set Opacity
		/// </summary>
		/// <param name="enabled"></param>
		private void SetOpacity(Double opacity)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new DoubleDelegate(SetOpacity), opacity);
				return;
			}

			this.Opacity = opacity;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Fill Rounded Rectangle
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="targetRectangle"></param>
		/// <param name="lineDrawsPen"></param>
		/// <param name="rectLineBrush"></param>
		/// <param name="size"></param>
		protected void FillRoundedRectangle(Graphics graphics,
				Rectangle targetRectangle,
				Pen lineDrawsPen,
				Brush rectLineBrush, Brush rectFillBrush,
				Size roundSize)
		{
			System.Drawing.Drawing2D.SmoothingMode smoothingMode_Push = graphics.SmoothingMode;
			System.Drawing.Drawing2D.PixelOffsetMode pixelOffsetMode_Push = graphics.PixelOffsetMode;
			System.Drawing.Drawing2D.GraphicsPath graphicPath;

			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

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
