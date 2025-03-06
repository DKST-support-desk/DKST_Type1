using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	/// <summary>
	/// VisualTabControl
	/// </summary>
	/// <remarks>
	/// from Dobon.net
	/// http://dobon.net/vb/dotnet/control/tabsidebug.html
	/// </remarks>
	public partial class VisualTabControl : TabControl
	{
		[System.Runtime.InteropServices.DllImportAttribute("uxtheme.dll")]
		private static extern int SetWindowTheme(IntPtr hwnd, string subAppName, string subIdList);

		#region Private Fields

		/// <summary>
		/// TextRenderingHint
		/// </summary>
		private System.Drawing.Text.TextRenderingHint _TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// Constructors
		/// </summary>
		public VisualTabControl()
			: base()
		{
			//InitializeComponent();

			// VisualStyles 
			if (Application.RenderWithVisualStyles)
			{
				// Enable Paint Event
				this.SetStyle(ControlStyles.UserPaint, true);
			}

			// Enable DoubleBuffered
			this.DoubleBuffered = true;

			//this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			////this.SetStyle(ControlStyles.DoubleBuffer, true);

			// Resize Redraw
			this.ResizeRedraw = true;

			//this.SetStyle(ControlStyles.ResizeRedraw, true);
			// If ControlStyles.UserPaint==True then fixed changed SizeMode
			// SizeMode = TabSizeMode.Fixed
			this.SizeMode = TabSizeMode.Fixed;
			this.ItemSize = new Size(60, 14);
			this.Appearance = TabAppearance.Normal;
			this.Multiline = true;
		}
		#endregion

		#region Event

		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="pe"></param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			TabPage tabPage = null;

			// TabControl FillRectangle background
			pe.Graphics.FillRectangle(SystemBrushes.Control, this.ClientRectangle);
			if (this.TabPages.Count == 0) return;

			// TabPage Draw Selected Tab Rectangle
			if (this.SelectedIndex >= 0)
			{
				tabPage = this.TabPages[this.SelectedIndex];
				Rectangle pageRect = new Rectangle(
					tabPage.Bounds.X - 2, tabPage.Bounds.Y - 2,
					tabPage.Bounds.Width + 5, tabPage.Bounds.Height + 5);	// '2', '5' is adjust
				if (TabRenderer.IsSupported)
				{
					TabRenderer.DrawTabPage(pe.Graphics, pageRect);
				}
			}

			// Tab Draw
			for (int iCnt = 0; iCnt < this.TabPages.Count; iCnt++)
			{
				tabPage = this.TabPages[iCnt];
				Rectangle tabRect = this.GetTabRect(iCnt);

				// Select tab status
				System.Windows.Forms.VisualStyles.TabItemState tabItemState;
				if (!this.Enabled)
				{
					// Tab is disabled
					tabItemState = System.Windows.Forms.VisualStyles.TabItemState.Disabled;
				}
				else if (this.SelectedIndex == iCnt)
				{
					// Tab is selected
					tabItemState = System.Windows.Forms.VisualStyles.TabItemState.Selected;
				}
				else
				{
					// Tab is Normal (Enabled & Not selected)
					tabItemState = System.Windows.Forms.VisualStyles.TabItemState.Normal;
				}

				// For Selected tab control
				//if (this.SelectedIndex == iCnt)
				if (tabItemState == System.Windows.Forms.VisualStyles.TabItemState.Selected)
				{
					if (this.Alignment == TabAlignment.Top)
					{
						tabRect.Height += 1;
					}
					else if (this.Alignment == TabAlignment.Bottom)
					{
						tabRect.Y -= 2;
						tabRect.Height += 2;
					}
					else if (this.Alignment == TabAlignment.Left)
					{
						tabRect.Width += 1;
					}
					else if (this.Alignment == TabAlignment.Right)
					{
						tabRect.X -= 2;
						tabRect.Width += 2;
					}
				}

				// Set ImageSize
				Size imgSize;
				if (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right)
				{
					imgSize = new Size(tabRect.Height, tabRect.Width);
				}
				else
				{
					imgSize = tabRect.Size;
				}

				Bitmap bmp = null;
				try
				{
					if (imgSize.Width > 0 && imgSize.Height > 0)
					{
						// Draw bitmap image
						bmp = new Bitmap(imgSize.Width, imgSize.Height);
						Graphics graphics = Graphics.FromImage(bmp);

						// '1' is Adjust
						// Text isn't draw, because after Draw String for tab's (because , Has any bug. @English arial font)
						if (TabRenderer.IsSupported)
						{
							TabRenderer.DrawTabItem(graphics, new Rectangle(0, 0, bmp.Width, bmp.Height + 1),
									"", tabPage.Font, false, tabItemState);
						}
						graphics.Dispose();

						// Bitmap Rotate
						switch (this.Alignment)
						{
							case TabAlignment.Bottom:
								bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
								// Draw Tabs String After Rotate
								DrawTabsString(bmp, tabPage);
								break;
							case TabAlignment.Left:
								// Draw Tabs String Before Rotate
								DrawTabsString(bmp, tabPage);
								bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
								break;
							case TabAlignment.Right:
								// Draw Tabs String Before Rotate
								DrawTabsString(bmp, tabPage);
								bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
								break;
							default:
								// Top
								break;
						}

						// Draw Image
						pe.Graphics.DrawImage(bmp, tabRect.X, tabRect.Y, bmp.Width, bmp.Height);
					}
				}
				finally
				{
					if (bmp != null)
					{
						bmp.Dispose();
						bmp = null;
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// DrawTabsString
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="tabPage"></param>
		private void DrawTabsString(Bitmap bmp, TabPage tabPage)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			Graphics graphics = Graphics.FromImage(bmp);

			// Call the OnPaint method of the base class.base.OnPaint(e);
			// Call methods of the System.Drawing.Graphics object.
			graphics.TextRenderingHint = _TextRenderingHint;

			graphics.DrawString(tabPage.Text, tabPage.Font,
				SystemBrushes.ControlText, new RectangleF(0, 0, bmp.Width, bmp.Height), stringFormat);

			// Dispose
			graphics.Dispose();
			stringFormat.Dispose();
		}

		#endregion

		#region Property

		//
		// 概要:
		//     この System.Drawing.Graphics に関連付けられているテキストのレンダリング モードを取得または設定します。
		//
		// 戻り値:
		//     System.Drawing.Text.TextRenderingHint 値の 1 つ。
		[Category("AntiAlias")]
		public System.Drawing.Text.TextRenderingHint TextRenderingHint
		{
			get
			{
				return _TextRenderingHint;
			}
			set
			{
				_TextRenderingHint = value;
			}
		}

		#endregion
	}
}
