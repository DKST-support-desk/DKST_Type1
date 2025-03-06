using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLOS.Gui.Core.Extensions
{
    public static class DataGridViewExtension
    {
        public static void BeginControlUpdate(this DataGridView control)
        {
            WinApi.BeginControlUpdate(control);
        }

        public static void EndControlUpdate(this DataGridView control)
        {
            WinApi.EndControlUpdate(control);
        }
    }
}
