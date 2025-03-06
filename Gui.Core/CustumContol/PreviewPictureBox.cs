using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace PLOS.Gui.Core.CustumContol
{

    public class PreviewPictureBox : PictureBox
    {
        private Image _nextImage = null;
        private Image _prevImage = null;
        private float _opacity = 0.5f;
        private bool _nextHit = false;
        private bool _prevHit = false;
        private Rectangle _nextRect = new Rectangle();
        private Rectangle _prevRect = new Rectangle();
        private bool _drawNext = false;
        private bool _drawPrev = false;
        private bool _visiblePrevNext = true;
        private bool _visibleAction = false;

        public event EventHandler<ImageChangeEventArgs> ImageChange = null;
        public event EventHandler ImageNext = null;
        public event EventHandler ImagePrev = null;

        public PreviewPictureBox()
        {
        }

        #region Public Properties

        [DefaultValue(null)]
        public Image PrevImage
        {
            set
            {
                _prevImage = value;
                this.UpdatePrevPosition();
            }
            get { return _prevImage; }
        }

        [DefaultValue(null)]
        public Image NextImage
        {
            set
            {
                _nextImage = value;
                this.UpdateNextPosition();
            }
            get { return _nextImage; }
        }

        [DefaultValue(null)]
        public new Image Image
        {
            set
            {
                base.Image = value;
                this.OnImageChange();
            }
            get { return base.Image; }
        }

        [DefaultValue(0.5f)]
        public float DefaultOpacity
        {
            set { _opacity = value; }
            get { return _opacity; }
        }

        /// <summary>
        /// 前へと次への表示
        /// </summary>
        [DefaultValue(true)]
        public bool VisiblePrevNext
        {
            set { _visiblePrevNext = value; }
            get { return _visiblePrevNext; }
        }

        #endregion

        #region Private Methods

        private void UpdateNextPosition()
        {
            if (!_visiblePrevNext)
                return;

            _nextRect.X = this.Width - _nextImage.Width - 10;
            _nextRect.Y = (this.Height / 2) - (_nextImage.Height / 2);
            _nextRect.Width = _nextImage.Width;
            _nextRect.Height = _nextImage.Height;
        }

        private void UpdatePrevPosition()
        {
            if (!_visiblePrevNext)
                return;

            _prevRect.X = 10;
            _prevRect.Y = (this.Height / 2) - (_prevImage.Height / 2);
            _prevRect.Width = _prevImage.Width;
            _prevRect.Height = _prevImage.Height;

        }

        #endregion

        #region OnResize
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.UpdateNextPosition();
            this.UpdatePrevPosition();
        }
        #endregion

        #region OnImageChange
        protected void OnImageChange()
        {
            if (ImageChange != null)
            {
                ImageChangeEventArgs e = new ImageChangeEventArgs();
                this.ImageChange(this, e);

                if (this.Image != null)
                {
                    //新しいイメージをセット
                    _drawNext = e.IsNext;
                    _drawPrev = e.IsPrev;
                    //コントロール再描画
                    this.Refresh();
                }
            }
        }
        #endregion

        #region OnImageNext
        protected void OnImageNext()
        {
            if (ImageNext != null)
            {
                this.ImageNext(this, EventArgs.Empty);
            }
        }
        #endregion

        #region OnImagePrev
        protected void OnImagePrev()
        {
            if (ImagePrev != null)
            {
                this.ImagePrev(this, EventArgs.Empty);
            }
        }
        #endregion

        #region OnPaint
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (base.Image == null)
                return;

            if (!_visiblePrevNext)
                return;

            if (!_visibleAction)
                return;

            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix44 = 1;

            //ImageAttributesオブジェクトの作成
            System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();

            if (_nextImage != null && _drawNext)
            {
                if (!_nextHit)
                    cm.Matrix33 = _opacity;
                else
                    cm.Matrix33 = 1;

                //ColorMatrixを設定する
                ia.SetColorMatrix(cm);

                pe.Graphics.DrawImage(_nextImage, _nextRect, 0, 0, _nextImage.Width, _nextImage.Height, GraphicsUnit.Pixel, ia);
            }

            if (_prevImage != null && _drawPrev)
            {
                if (!_prevHit)
                    cm.Matrix33 = _opacity;
                else
                    cm.Matrix33 = 1;

                //ColorMatrixを設定する
                ia.SetColorMatrix(cm);

                pe.Graphics.DrawImage(_prevImage, _prevRect, 0, 0, _prevImage.Width, _prevImage.Height, GraphicsUnit.Pixel, ia);
            }
            
        }
        #endregion

        #region OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_visiblePrevNext)
                return;

            //次へを押す処理
            if (_nextRect.X <= e.X && _nextRect.Right >= e.X &&
                _nextRect.Y <= e.Y && _nextRect.Bottom >= e.Y &&
                _drawNext)
            {
                if (!_nextHit)
                {
                    _nextHit = true;
                    this.Refresh();
                    this.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if (_nextHit)
                {
                    _nextHit = false;
                    this.Refresh();
                    this.Cursor = Cursors.Default;

                }
            }
            //前へを押す処理
            if (_prevRect.X <= e.X && _prevRect.Right >= e.X &&
                _prevRect.Y <= e.Y && _prevRect.Bottom >= e.Y &&
                _drawPrev)
            {
                if (!_prevHit)
                {
                    _prevHit = true;
                    this.Refresh();

                    this.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if (_prevHit)
                {
                    _prevHit = false;
                    this.Refresh();
                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        #region OnMouseUp

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!_visiblePrevNext)
                return;

            //次へを押す処理
            if (_nextRect.X <= e.X && _nextRect.Right >= e.X &&
                _nextRect.Y <= e.Y && _nextRect.Bottom >= e.Y &&
                _drawNext)
            {
                this.OnImageNext();
            }
            //前へを押す処理
            if (_prevRect.X <= e.X && _prevRect.Right >= e.X &&
                _prevRect.Y <= e.Y && _prevRect.Bottom >= e.Y &&
                _drawPrev)
            {
                this.OnImagePrev();
            }
        }
        #endregion

        #region OnMouseEnter
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            _visibleAction = true;

            this.Refresh();
        }
        #endregion

        #region OnMouseLeave
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _visibleAction = false;

            this.Refresh();
        }
        #endregion
    }

    public class ImageChangeEventArgs : EventArgs
    {
        private bool _isNext = false;
        private bool _isPrev = false;

        public ImageChangeEventArgs()
        {
        }

        public bool IsPrev
        {
            set { _isPrev = value; }
            get { return _isPrev; }
        }

        public bool IsNext
        {
            set { _isNext = value; }
            get { return _isNext; }
        }
    }
}
