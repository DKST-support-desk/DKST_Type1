using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace PLOS.Gui.Core.Extensions
{
    public static class ControlExtension
    {
        public static void DoubleBufferOn(this Control control)
        {
            Type tp = control.GetType();
            MethodInfo info = tp.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);

            object[] param = {
                (ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint),
                true
            };

            info.Invoke(control, param);
        }

        public static void DoubleBufferOff(this Control control)
        {
            Type tp = control.GetType();
            MethodInfo info = tp.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);

            object[] param = {
                (ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint),
                false
            };

            info.Invoke(control, param);
        }

        public static void BeginControlUpdate(this Control control)
        {
            WinApi.BeginControlUpdate(control);
        }

        public static void EndControlUpdate(this Control control)
        {
            WinApi.EndControlUpdate(control);
        }
    }
}
