using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;

namespace PLOS.Gui.Core.CustumContol
{
    [Designer(typeof(PageControl.PageControlDesigner))]
    public class PageControl : Panel
    {
        #region Designer
        internal class PageControlDesigner : ParentControlDesigner
        {
            public void OnAddPage(object sender, EventArgs e)
            {
                PageControl ctrl = this.Control as PageControl;

                if (ctrl != null)
                {
                    PagePanel page = new PagePanel();
                    page.Dock = DockStyle.Fill;
                    this.Component.Site.Container.Add(page);

                    ctrl.Controls.Add(page);
                }
            }

            public void OnRemovePage(object sender, EventArgs e)
            {
                PageControl ctrl = this.Control as PageControl;

                if (ctrl != null)
                {
                    PagePanel page = ctrl.ActivePage;
                    ctrl.Controls.Remove(page);
                    this.Component.Site.Container.Remove(page);
                }
            }

            public void OnNextPage(object sender, EventArgs e)
            {
                PageControl ctrl = this.Control as PageControl;

                if (ctrl != null)
                {
                    ctrl.ActivePageIndex++;
                }
            }

            public void OnPrevPage(object sender, EventArgs e)
            {
                PageControl ctrl = this.Control as PageControl;

                if (ctrl != null)
                {
                    ctrl.ActivePageIndex--;
                }
            }

            public override void Initialize(IComponent component)
            {
                this.Verbs.Add(new DesignerVerb("ページ追加", new EventHandler(OnAddPage)));
                this.Verbs.Add(new DesignerVerb("ページ削除", new EventHandler(OnRemovePage)));
                this.Verbs.Add(new DesignerVerb("次のページ", new EventHandler(OnNextPage)));
                this.Verbs.Add(new DesignerVerb("前のページ", new EventHandler(OnPrevPage)));

                base.Initialize(component);
            }
        }
        #endregion

        List<PagePanel> _Pages;
        int _ActivePage = 0;

        public PageControl()
        {
            _Pages = new List<PagePanel>();
        }

        [Browsable(false)]
        public int Count
        {
            get { return _Pages.Count; }
        }

        public List<PagePanel> Pages
        {
            get { return _Pages; }
        }

        public int ActivePageIndex
        {
            set
            {
                if (value >= 0 && value < _Pages.Count)
                {
                    _ActivePage = value;

                    _Pages[_ActivePage].BringToFront();
                }
            }
            get
            {
                return _ActivePage;
            }
        }

        public PagePanel ActivePage
        {
            set
            {
                if (_Pages.Contains(value))
                {
                    this.ActivePageIndex = _Pages.IndexOf(value);
                }
            }
            get
            {
                if (_ActivePage < 0 || _ActivePage >= _Pages.Count)
                    return null;

                return _Pages[_ActivePage];
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is PagePanel)
            {
                ((PagePanel)e.Control).Parent = this;

                _Pages.Add(e.Control as PagePanel);

                base.OnControlAdded(e);
            }
            else
            {
                this.Controls.Remove(e.Control);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DesignMode)
            {
                Brush br = new SolidBrush(Color.PaleVioletRed);

                e.Graphics.FillRectangle(br, 0, 0, this.Width, this.Height);
            }
        }
    }
}
