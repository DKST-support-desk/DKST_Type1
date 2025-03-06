using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	/// <summary>
	/// 
	/// </summary>
	public partial class DualTitleDataGridView : DataGridView
	{
		#region Private Fields

		/// <summary>
		/// 
		/// </summary>
		private DockingCell[] _DockingCells;

		/// <summary>
		/// Header Border Color
		/// </summary>
		private Color _HeaderBorderColor = SystemColors.ControlDark;

		/// <summary>
		/// ColorMatrix
		/// </summary>
		private System.Drawing.Imaging.ColorMatrix _ColorMatrix =
			new System.Drawing.Imaging.ColorMatrix(
				new float[][] {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0}, 
                new float[] {0, 0, 0, 1, 0},
                new float[] {-0.1f, -0.1f, -0.1f, 0, 1}
            });

		/// <summary>
		/// TextRenderingHint
		/// </summary>
		private System.Drawing.Text.TextRenderingHint _TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		#endregion

		#region Public Fields

		/// <summary>
		/// Docking Cell (Inner Class)
		/// </summary>
		/// <remarks>
		///  第0カラムから2カラム分結合し、上段テキストを"A1"とする場合、
		///  new DataGridViewEx.DockingCell(0, 2, "A1")のようにインスタンスを作成
		///  さらに位置決めの場合
		///  new DataGridViewEx.DockingCell(0, 2, "A1", DataGridViewContentAlignment.MiddleCenter)
		/// </remarks>
		[Serializable]
		public class DockingCell
		{
			#region Private Fields
			/// <summary>
			/// Docking start column position (0 to 
			/// </summary>
			private int _StartColumn;

			/// <summary>
			/// Docking Column count
			/// </summary>
			private int _ColumnCount;

			/// <summary>
			/// Header Text
			/// </summary>
			private string _Text;

			/// <summary>
			/// Ratio
			/// </summary>
			private float _Ratio = 0.5f;

			/// <summary>
			/// DataGridViewContentAlignment
			/// </summary>
			private DataGridViewContentAlignment _Alignment = DataGridViewContentAlignment.MiddleCenter;
			#endregion

			#region Constructors and Destructor
			/// <summary>
			/// Constructors
			/// </summary>
			/// <param name="start"></param>
			/// <param name="count"></param>
			/// <param name="text"></param>
			public DockingCell(int start, int count, string text)
			{
				_StartColumn = start;
				_ColumnCount = count;
				_Text = text;
			}

			/// <summary>
			/// Constructors
			/// </summary>
			/// <param name="start"></param>
			/// <param name="count"></param>
			/// <param name="text"></param>
			/// <param name="alignment"></param>
			public DockingCell(int start, int count, string text, float ratio, DataGridViewContentAlignment alignment)
				: this(start, count, text)
			{
				_Ratio = ratio;
				_Alignment = alignment;
			}
			#endregion

			#region Property
			/// <summary>
			/// Docking start column position (0 to 
			/// </summary>
			public int Start
			{
				get { return _StartColumn; }
				set { _StartColumn = value; }
			}

			/// <summary>
			/// Docking Column count
			/// </summary>
			public int Count
			{
				get { return _ColumnCount; }
				set { _ColumnCount = value; }
			}

			/// <summary>
			/// Header Text
			/// </summary>
			public string Text
			{
				get { return _Text; }
				set { _Text = value; }
			}

			/// <summary>
			/// Ratio
			/// </summary>
			private float Ratio
			{
				get { return _Ratio; }
			}

			/// <summary>
			/// DataGridViewContentAlignment
			/// </summary>
			public DataGridViewContentAlignment Alignment
			{
				get { return _Alignment; }
			}
			#endregion
		}

		#endregion

		#region EventHandler(Delegate)
		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public DualTitleDataGridView()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="container"></param>
		public DualTitleDataGridView(IContainer container)
		{
			container.Add(this);

			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
		}

		#endregion

		#region Event

		/// <summary>
		/// On Cell Painting Event Override
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
		{
			// For Design Mode
			if (this.DesignMode)
			{
				base.OnCellPainting(e);
				return;
			}

			if (e.ColumnIndex >= 0 && e.RowIndex == -1)
			{
				//_ColorMatrix
				_HeaderBorderColor = Color.DarkGray;

				// Find "DockingCell" List
				int FindIndex = -1;
				for (int i = 0; i < _DockingCells.Length; i++)
				{
					if (e.ColumnIndex >= _DockingCells[i].Start && e.ColumnIndex < _DockingCells[i].Start + _DockingCells[i].Count)
					{
						FindIndex = i;
						break;
					}
				}

				if (FindIndex >= 0)		// Find "DockingCell" 
				{
					int xPos = e.CellBounds.X;
					for (int i = e.ColumnIndex - 1; i >= _DockingCells[FindIndex].Start; i--)
					{
						xPos -= this.Columns[i].Width;
					}
					int width = 0;
					for (int i = DockingCells[FindIndex].Start; i < _DockingCells[FindIndex].Start + _DockingCells[FindIndex].Count; i++)
					{
						width += this.Columns[i].Width;
					}

					// Upper rectangle
					Rectangle rectUpper =
						new Rectangle(xPos, e.CellBounds.Y + 1, width, e.CellBounds.Height / 2 - 1);

					// Lower rectangle
					Rectangle rectLower =
						new Rectangle(e.CellBounds.X, e.CellBounds.Y + e.CellBounds.Height / 2 + 1, e.CellBounds.Width, e.CellBounds.Height / 2 - 1);

					// Call the OnPaint method of the base class.base.OnPaint(e);
					// Call methods of the System.Drawing.Graphics object.
					// Paint Cell Without ContentForeground
					e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.All);

					////////e.Graphics.DrawImage(bmp, new Point(width - 5, e.CellBounds.Y));
					// Gradient Brush Create
					LinearGradientBrush grdBrushUpper = new LinearGradientBrush(
							rectUpper,
							Color.White,
							Color.LightGray, 90F);
					grdBrushUpper.GammaCorrection = true;

					LinearGradientBrush grdBrushLower = new LinearGradientBrush(
							rectLower,
							Color.White,
							Color.LightGray, 90F);
					grdBrushLower.GammaCorrection = true;

					FillRoundedRectangle(e.Graphics, rectUpper,
						new Pen(Color.White, 1),
						new SolidBrush(Color.White), grdBrushUpper);

					FillRoundedRectangle(e.Graphics, rectLower,
						new Pen(Color.White, 1),
						new SolidBrush(Color.White), grdBrushLower);

					// Upper & Lower Draw Border
					DrawHeaderBorder(e.Graphics, rectUpper);
					DrawHeaderBorder(e.Graphics, rectLower);

					e.Graphics.TextRenderingHint = _TextRenderingHint;

					// Draw Text @ DrawString
					e.Graphics.DrawString(
						_DockingCells[FindIndex].Text, e.CellStyle.Font, Brushes.Black,
						rectUpper, CellStyle2StringFormatAlignment(_DockingCells[FindIndex].Alignment));
					e.Graphics.DrawString(
						e.Value.ToString(), e.CellStyle.Font, Brushes.Black, 
						rectLower, CellStyle2StringFormatAlignment(e.CellStyle.Alignment));
				}
				else 		// No Find "DockingCell" 
				{
					Rectangle rect =
						new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
					LinearGradientBrush grdBrush = new LinearGradientBrush(
							rect,
							Color.White,
							Color.LightGray, 90F);
					grdBrush.GammaCorrection = true;

					// Paint Cell
					e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.All);

					FillRoundedRectangle(e.Graphics, rect,
						new Pen(Color.White, 1),
						new SolidBrush(Color.White), grdBrush);
					DrawHeaderBorder(e.Graphics, rect);

					e.Graphics.TextRenderingHint = _TextRenderingHint;
					e.Graphics.DrawString(
						e.Value.ToString(), e.CellStyle.Font, Brushes.Black,
						rect, CellStyle2StringFormatAlignment(e.CellStyle.Alignment));
				}

				// Drawing completion notice
				e.Handled = true;
			}
		}

		#endregion

		#region Property

		/// <summary>
		/// DockingCellList
		/// </summary>
		/// <remarks>
		///  第0カラムから2カラム分結合し、上段テキストを"A1"とする場合、
		///  new DataGridViewEx.DockingCell(0, 2, "A1")のようにインスタンスを作成
		///  さらに位置決めの場合
		///  new DataGridViewEx.DockingCell(0, 2, "A1", DataGridViewContentAlignment.MiddleCenter)
		/// </remarks>
		//[Category("CustomControl")]
		[DefaultValue(null)]
		[Browsable(false)]
		public DockingCell[] DockingCells
		{
			get { return _DockingCells; }
			set { _DockingCells = value; }
		}

		[Category("CustomControl")]
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

		#region Private Methods

		/// <summary>
		/// Draw Header Border
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		private void DrawHeaderBorder(Graphics g, Rectangle rect)
		{
			g.DrawLine(Pens.White, rect.Left, rect.Top + 3, rect.Left, rect.Bottom - 1);
			g.DrawLine(new Pen(_HeaderBorderColor), rect.Right - 1, rect.Top + 4, rect.Right - 1, rect.Bottom - 1);
		}

		/// <summary>
		/// CellStyle Alignment --> StringFormat Alignment
		/// </summary>
		/// <param name="Alignment"></param>
		/// <returns></returns>
		private StringFormat CellStyle2StringFormatAlignment(DataGridViewContentAlignment Alignment)
		{
			StringFormat stringFormat = new StringFormat();

			switch (Alignment)
			{
				case DataGridViewContentAlignment.TopLeft:
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Near;
					break;
				case DataGridViewContentAlignment.TopCenter:
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Near;
					break;
				case DataGridViewContentAlignment.TopRight:
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Near;
					break;
				case DataGridViewContentAlignment.MiddleLeft:
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Center;
					break;
				case DataGridViewContentAlignment.MiddleCenter:
				default:
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					break;
				case DataGridViewContentAlignment.MiddleRight:
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Center;
					break;
				case DataGridViewContentAlignment.BottomLeft:
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Far;
					break;
				case DataGridViewContentAlignment.BottomCenter:
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Far;
					break;
				case DataGridViewContentAlignment.BottomRight:
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Far;
					break;
			}

			return stringFormat;
		}

		#region FillRoundedRectangle Methods

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
				Brush rectLineBrush, Brush rectFillBrush)
		{
			System.Drawing.Drawing2D.SmoothingMode smoothingMode_Push = graphics.SmoothingMode;
			System.Drawing.Drawing2D.PixelOffsetMode pixelOffsetMode_Push = graphics.PixelOffsetMode;
			System.Drawing.Drawing2D.GraphicsPath graphicPath;

			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

			graphicPath = new System.Drawing.Drawing2D.GraphicsPath();
			graphicPath.FillMode = System.Drawing.Drawing2D.FillMode.Winding;

			graphicPath.AddRectangle(targetRectangle);

			graphics.DrawPath(lineDrawsPen, graphicPath);
			graphics.FillPath(rectFillBrush, graphicPath);

			// Pop Saved Data
			graphics.SmoothingMode = smoothingMode_Push;
			graphics.PixelOffsetMode = pixelOffsetMode_Push;
		}

		#endregion

		#endregion
	}
}
