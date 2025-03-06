using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace PLOS.Gui.Core.DataGridViews
{
    /// <summary>
    /// This represents a numeric text box control that can be hosted in a 
    /// <see cref="DataGridViewNumericCell"/>. It is implemented with the
    /// <see cref="NumericTextBox"/> control and provides all its features in
    /// editing.
    /// </summary>
	public class DataGridViewNumericEditingControl : PLOS.Gui.Core.CustumContol.NumericTextBox,
        IDataGridViewEditingControl
    {
        #region Private Fields

        private int          rowIndex;
        private bool         valueChanged;
        private bool         repositionOnValueChange;
        private DataGridView dataGridView;

        private static readonly DataGridViewContentAlignment anyCenter;
        private static readonly DataGridViewContentAlignment anyRight;
        private static readonly DataGridViewContentAlignment anyTop;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewNumericEditingControl"/>
        /// class.
        /// </summary>
        public DataGridViewNumericEditingControl()
        {
            base.TabStop = false;
        }

        /// <summary>
        /// 
        /// </summary>
        static DataGridViewNumericEditingControl()
        {
            anyTop    = DataGridViewContentAlignment.TopRight | 
                DataGridViewContentAlignment.TopCenter | 
                DataGridViewContentAlignment.TopLeft;
            anyRight  = DataGridViewContentAlignment.BottomRight | 
                DataGridViewContentAlignment.MiddleRight |
                DataGridViewContentAlignment.TopRight;
            anyCenter = DataGridViewContentAlignment.BottomCenter | 
                DataGridViewContentAlignment.MiddleCenter | 
                DataGridViewContentAlignment.TopCenter;
        }

        #endregion

        #region IDataGridViewEditingControl Members

        /// <summary>
        /// Changes the control's user interface (UI) to be consistent with 
        /// the specified cell style.
        /// </summary>
        /// <param name="dataGridViewCellStyle">
        /// The <see cref="DataGridViewCellStyle"/> to use as a pattern for 
        /// the UI.
        /// </param>
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 0xff)
            {
                Color colorBack = Color.FromArgb(0xff, 
                    dataGridViewCellStyle.BackColor);
                this.BackColor  = colorBack;
                this.dataGridView.EditingPanel.BackColor = colorBack;
            }
            else
            {
                this.BackColor = dataGridViewCellStyle.BackColor;
            }

            this.ForeColor = dataGridViewCellStyle.ForeColor;
            if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
            {
                base.WordWrap = true;
            }

            base.TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
            
            repositionOnValueChange = (dataGridViewCellStyle.WrapMode == 
                DataGridViewTriState.True) && 
                ((dataGridViewCellStyle.Alignment & 
                DataGridViewNumericEditingControl.anyTop) == 
                DataGridViewContentAlignment.NotSet);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridView"/> that contains the 
        /// combo box control.
        /// </summary>
        /// <value>
        /// The <see cref="DataGridView"/> that contains the 
        /// <see cref="DataGridViewComboBoxCell"/> that contains this control; 
        /// otherwise, <see langword="null"/> if there is no associated 
        /// <see cref="DataGridView"/>.
        /// </value>
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return this.dataGridView;
            }

            set
            {
                this.dataGridView = value;
            }
        }

        /// <summary>
        /// Gets or sets the formatted representation of the current value of 
        /// the control.
        /// </summary>
        /// <value>
        /// An object representing the current value of this control.
        /// </value>
        public object EditingControlFormattedValue
        {
            get
            {
                return this.GetEditingControlFormattedValue(
                    DataGridViewDataErrorContexts.Formatting);
            }

            set
            {
                this.Text = (string)value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the owning cell's parent row.
        /// </summary>
        /// <value>
        /// The index of the row that contains the owning cell; <c>-1</c> if 
        /// there is no owning row.
        /// </value>
        public int EditingControlRowIndex
        {
            get
            {
                return this.rowIndex;
            }

            set
            {
                this.rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current value of the 
        /// control has changed.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the value of the control has 
        /// changed; otherwise, <see langword="false"/>.
        /// </value>
        public bool EditingControlValueChanged
        {
            get
            {
                return this.valueChanged;
            }

            set
            {
                this.valueChanged = value;
            }
        }

        /// <summary>
        /// 編集コントロールは、コントロールによって処理する必要がある入力キーと、 
		/// DataGridView によって処理する必要がある入力キーを判断するために、
		/// このメソッドを実装します。
		/// EditingControlWantsInputKey メソッドは、 DataGridView によって呼び出されます。 
		/// DataGridView は、自身が keyData を処理できる場合には、 dataGridViewWantsInputKey を true に設定します。 
		/// 編集コントロールが keyData の処理を DataGridView に委任できる場合、 dataGridViewWantsInputKey が true で
		/// あれば、 EditingControlWantsInputKey は false を返します。 EditingControlWantsInputKey の実装によっては、 
		/// dataGridViewWantsInputKey の値が true であってもこれを無視し、編集コントロールで keyData を処理する場合があります。
		/// <param name="keyData">押されたキーを表す Keys。 </param>
		/// <param name="dataGridViewWantsInputKey">keyData に格納された Keys を、 DataGridView に処理させる場合は true。それ以外の場合は false。 </param>
		/// <returns></returns>
        public bool EditingControlWantsInputKey(Keys keyData,
            bool dataGridViewWantsInputKey)
        {
            switch ((keyData & Keys.KeyCode))
            {
                case Keys.Prior:
                case Keys.Next:
                    if (this.valueChanged)
                    {
                        return true;
                    }
                    break;

                case Keys.End:
                case Keys.Home:
                    if (this.SelectionLength != this.Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Left:
                    if (((this.RightToLeft == RightToLeft.No) && 
                        ((this.SelectionLength != 0) || 
                        (base.SelectionStart != 0))) || 
                        ((this.RightToLeft == RightToLeft.Yes) && 
                        ((this.SelectionLength != 0) || 
                        (base.SelectionStart != this.Text.Length))))
                    {
                        return true;
                    }
                    break;

                case Keys.Up:
                    if ((this.Text.IndexOf("\r\n") >= 0) && 
                        ((base.SelectionStart + this.SelectionLength) >= 
                        this.Text.IndexOf("\r\n")))
                    {
                        return true;
                    }
                    break;

                case Keys.Right:
                    if (((this.RightToLeft == RightToLeft.No) && 
                        ((this.SelectionLength != 0) || 
                        (base.SelectionStart != this.Text.Length))) || 
                        ((this.RightToLeft == RightToLeft.Yes) && 
                        ((this.SelectionLength != 0) || 
                        (base.SelectionStart != 0))))
                    {
                        return true;
                    }
                    break;

                case Keys.Down:
                    {
                        int num1 = base.SelectionStart + this.SelectionLength;
                        if (this.Text.IndexOf("\r\n", num1) != -1)
                        {
                            return true;
                        }
                        break;
                    }
                case Keys.Delete:
                    if ((this.SelectionLength > 0) || 
                        (base.SelectionStart < this.Text.Length))
                    {
                        return true;
                    }
                    break;

                case Keys.Return:
                    if ((((keyData & (Keys.Alt | Keys.Control | Keys.Shift)) 
                        == Keys.Shift) && this.Multiline) && base.AcceptsReturn)
                    {
                        return true;
                    }
                    break;

				case Keys.V:
					//if ((keyData & Keys.Control) == Keys.Shift)
					//{
						return false;
					//}
					//break;

            }

            return !dataGridViewWantsInputKey;
        }

        /// <summary>Gets the cursor used during editing.</summary>
        /// <value>
        /// A <see cref="System.Windows.Forms.Cursor"/> that represents the 
        /// cursor image used by the mouse pointer during editing.
        /// </value>
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>Retrieves the formatted value of the cell.</summary>
        /// <param name="context">
        /// A bitwise combination of <see cref="DataGridViewDataErrorContexts"/> 
        /// values that specifies the data error context.
        /// </param>
        /// <returns>
        /// An <see cref="System.Object"/> that represents the formatted 
        /// version of the cell contents.
        /// </returns>
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        /// <summary>Prepares the currently selected cell for editing.</summary>
        /// <param name="selectAll">
        /// This is <see langword="true"/> to select all of the cell's 
        /// content; otherwise, <see langword="false"/>.
        /// </param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                base.SelectAll();
            }
            else
            {
                base.SelectionStart = this.Text.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cell contents need to be 
        /// repositioned whenever the value changes.
        /// </summary>
        /// <value>This returns <see langword="false"/> in all cases.</value>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return this.repositionOnValueChange;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This raises the <see cref="Control.Leave"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLeave(System.EventArgs e)
        {
            base.OnLeave(e);

            NotifyDataGridViewOfValueChange();
        }

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        /// <param name="e">
        /// An argument that are associated with the 
        /// <see cref="Control.TextChanged"/> event.
        /// </param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.NotifyDataGridViewOfValueChange();
        }

        /// <summary>
        /// Processes a key message and generates the appropriate control 
        /// events.
        /// </summary>
        /// <param name="m">
        /// A <see cref="Message"/>, passed by reference, that represents the 
        /// window message to process.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the message was processed 
        /// by the control; otherwise, <see langword="false"/>.
        /// </returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            Keys keysData = (Keys)((int)m.WParam);

            if (keysData == Keys.LineFeed)
            {
                if (((m.Msg == 0x102) && 
                    (Control.ModifierKeys == Keys.Control)) && 
                    (this.Multiline && base.AcceptsReturn))
                {
                    return true;
                }
            }
            else if (keysData == Keys.Return)
            {
                if ((m.Msg == 0x102) && 
                    (((Control.ModifierKeys != Keys.Shift) || !this.Multiline) 
                    || !base.AcceptsReturn))
                {
                    return true;
                }
            }
			else if ((keysData == Keys.A) && ((m.Msg == 0x100) &&
				(Control.ModifierKeys == Keys.Control)))
			{
				base.SelectAll();
				return true;
			}
			else if ((keysData == Keys.V) && ((m.Msg == 0x100) &&
				(Control.ModifierKeys == Keys.Control)))
			{
				// Ctrl + V 押下処理
				//base.SelectAll();
				return true;
			}
			//else if (keysData == Keys.V) 
			//{	
			//    if((m.Msg == 0x100) &&
			//    (Control.ModifierKeys == Keys.Control))
			//    {
			//        base.SelectAll();
			//        return true;
			//    }
			//}

 
            return base.ProcessKeyEventArgs(ref m);
        }

        #endregion

        #region Private Methods

        private void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            this.dataGridView.NotifyCurrentCellDirty(true);
        }

        private HorizontalAlignment TranslateAlignment(
            DataGridViewContentAlignment align)
        {
            if ((align & DataGridViewNumericEditingControl.anyRight) != 
                DataGridViewContentAlignment.NotSet)
            {
                return HorizontalAlignment.Right;
            }
            if ((align & DataGridViewNumericEditingControl.anyCenter) != 
                DataGridViewContentAlignment.NotSet)
            {
                return HorizontalAlignment.Center;
            }

            return HorizontalAlignment.Left;
        }

        #endregion
    }
}
