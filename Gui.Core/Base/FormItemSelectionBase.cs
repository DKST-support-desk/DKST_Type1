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
	public partial class FormItemSelectionBase : PLOS.Gui.Core.Base.FormFixedDialogBase
	{
		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public FormItemSelectionBase()
		{
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
		/// <param name="e"></param>
		private void OnSelectionListDoubleClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				if (lstbxSelection.SelectedIndex == -1)
				{
					DialogResult = System.Windows.Forms.DialogResult.Cancel;
					e.Cancel = true;
					return;
				}
			}
		}

		/// <summary>
		/// Ok button Clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOkClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		#endregion

		#region Property

		/// <summary>
		/// Selections
		/// </summary>
		public System.Collections.ArrayList Selections
		{
			set
			{
				lstbxSelection.Items.Clear();
				lstbxSelection.FormattingEnabled = true;
				lstbxSelection.DisplayMember = "Id";
				lstbxSelection.ValueMember = "Name";
				lstbxSelection.DataSource = value;
			}
		}

		/// <summary>
		/// SelectionMode
		/// </summary>
		public SelectionMode SelectionMode
		{
			set
			{
				lstbxSelection.SelectionMode = value;
			}
			get
			{
				return lstbxSelection.SelectionMode;
			}
		}

		/// <summary>
		/// SelectedIndex
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return lstbxSelection.SelectedIndex;
			}
		}

		/// <summary>
		/// SelectedIndices
		/// </summary>
		public ListBox.SelectedIndexCollection SelectedIndices
		{
			get
			{
				return lstbxSelection.SelectedIndices;
			}
		}

		/// <summary>
		/// SelectedItem
		/// </summary>
		public object SelectedItem
		{
			get
			{
				return lstbxSelection.SelectedItem;
			}
		}

		/// <summary>
		/// SelectedItems
		/// </summary>
		public ListBox.SelectedObjectCollection SelectedItems
		{
			get
			{
				return lstbxSelection.SelectedItems;
			}
		}

		/// <summary>
		/// SelectedValue
		/// </summary>
		public object SelectedValue
		{
			get
			{
				return lstbxSelection.SelectedValue;
			}
		}

		#endregion
	}
}
