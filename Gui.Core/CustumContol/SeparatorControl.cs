using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace PLOS.Gui.Core.CustumContol
{
    public enum BorderVectol
    {
        Vertical,
        Horizontail,
    }

    public class SeparatorControl : Control
    {
        public SeparatorControl()
        {
            this.BorderVectol = BorderVectol.Vertical;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        [DefaultValue(typeof(BorderVectol), "Vertical")]
        public BorderVectol BorderVectol { set; get; }

        [DefaultValue(false)]
        public override bool AutoSize
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        protected override Size DefaultMaximumSize
        {
            get
            {
                Size size = base.DefaultMaximumSize;

                if (this.BorderVectol == CustumContol.BorderVectol.Vertical)
                {
                    size.Width = 2;
                }
                if (this.BorderVectol == CustumContol.BorderVectol.Horizontail)
                {
                    size.Height = 2;
                }

                return size;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.BorderVectol == CustumContol.BorderVectol.Vertical)
            {
                ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, 2, e.ClipRectangle.Height) );
            }
            else
            {
                ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, 2) );
            }
        }
    }
}
