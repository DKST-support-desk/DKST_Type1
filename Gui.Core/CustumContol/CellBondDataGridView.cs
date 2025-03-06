using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	public partial class CellBondDataGridView : DataGridView
	{
		#region Private Fields

		/// <summary>
		/// 
		/// </summary>
		private IList<Int32> _PermitBondColumn = new List<Int32>();

		#endregion

		#region EventHandler(Delegate)
		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// Constructors
		/// </summary>
		public CellBondDataGridView()
		{
			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="container"></param>
		public CellBondDataGridView(IContainer container)
		{
			container.Add(this);

			InitializeComponent();

			// For Design Mode
			if (this.DesignMode) return;
		}
		#endregion

		#region Event

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
		{
			//if (!_PermitBondColumn.Contains(e.ColumnIndex))
			//{
			//    base.OnCellFormatting(e);
			//    return;
			//}

			if (e.RowIndex == 0) return;

			if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
			{
				e.Value = "";
				e.FormattingApplied = true; // 以降の書式設定は不要
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
		{
			//if (!_PermitBondColumn.Contains(e.ColumnIndex))
			//{
			//    base.OnCellPainting(e);
			//    return;
			//} 
			
			e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

			// 1行目や列ヘッダ、行ヘッダの場合は何もしない
			if (e.RowIndex < 1 || e.ColumnIndex < 0)
				return;

			if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
			{
				// セルの上側の境界線を「境界線なし」に設定
				e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
			}
			else
			{
				// セルの上側の境界線を既定の境界線に設定
				e.AdvancedBorderStyle.Top = AdvancedCellBorderStyle.Top;
			}
		}

		#endregion

		#region Property

		/// <summary>
		/// PermitBondColumn
		/// </summary>
		[Category("CustomControl")]
		public IList<Int32> PermitBondColumn
		{
			get
			{
				return this._PermitBondColumn;
			}

			set
			{
				this._PermitBondColumn = value;
			}
		}


		#endregion

		#region Public Methods
		#endregion

		#region Private Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		private bool IsTheSameCellValue(int column, int row)
		{
			if (_PermitBondColumn.Contains(column))
			{
				DataGridViewCell cell1 = this[column, row];
				DataGridViewCell cell2 = this[column, row - 1];

				if (cell1.Value == null || cell2.Value == null) return false;

				return cell1.Value.ToString().Equals(cell2.Value.ToString());
			}
			return false;
		}

		#endregion

	}
}
