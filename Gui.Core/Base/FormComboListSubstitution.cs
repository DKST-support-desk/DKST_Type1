using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Base
{
    public partial class FormComboListSubstitution : Form
    {
        #region Private Fields
        protected int _MaxDispItems;

        #endregion

        #region EventHandler(Delegate)
        #endregion

        #region Constructors and Destructor
        public FormComboListSubstitution()
        {
            InitializeComponent();
        }
        #endregion

        #region Event
        #endregion

        #region Property
        /// <summary>
        /// MaxDispItems
        /// </summary>
        public int MaxDispItems
        {
            get
            {
                return _MaxDispItems;
            }
            set
            {
                _MaxDispItems = value;
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

    }
}
