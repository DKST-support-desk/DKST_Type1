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
    [Designer(typeof(PagePanel.PagePanelControlDesigner))]
    [DesignTimeVisible(false)]
    public class PagePanel : Panel
    {
        #region Designer
        internal class PagePanelControlDesigner : ParentControlDesigner
        {
            public void OnAddPage(object sender, EventArgs e)
            {
                PagePanel ctrl = this.Control as PagePanel;

                if (ctrl != null && ctrl.Parent != null)
                {
                    PagePanel page = new PagePanel();
                    page.Dock = DockStyle.Fill;
                    this.Component.Site.Container.Add(page);

                    ((PageControl)ctrl.Parent).Controls.Add(page);
                }
            }

            public void OnRemovePage(object sender, EventArgs e)
            {
                PagePanel ctrl = this.Control as PagePanel;

                if (ctrl != null && ctrl.Parent != null)
                {
                    ((PageControl)ctrl.Parent).Controls.Remove(ctrl);
                    this.Component.Site.Container.Remove(ctrl);

                }
            }

            public void OnNextPage(object sender, EventArgs e)
            {
                PagePanel ctrl = this.Control as PagePanel;

                if (ctrl != null && ctrl.Parent != null)
                {
                    ((PageControl)ctrl.Parent).ActivePageIndex++;
                }
            }

            public void OnPrevPage(object sender, EventArgs e)
            {
                PagePanel ctrl = this.Control as PagePanel;

                if (ctrl != null && ctrl.Parent != null)
                {
                    ((PageControl)ctrl.Parent).ActivePageIndex--;
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

            protected override void PostFilterProperties(System.Collections.IDictionary properties)
            {
                properties.Remove("Visible");

                base.PostFilterProperties(properties);
            }
        }
        #endregion

        public PagePanel()
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.DesignMode)
            {
                Brush br = new SolidBrush(Color.MediumVioletRed);
                e.Graphics.FillRectangle(br, 0, 0, this.Width, this.Height);
            }
        }
    }
}
