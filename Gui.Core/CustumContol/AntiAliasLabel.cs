using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PLOS.Gui.Core.CustumContol
{
	/// <summary>
	/// AntiAliasLabel
	/// </summary>
	public partial class AntiAliasLabel : System.Windows.Forms.Label
	{
        #region Private Fields

        /// <summary>
		/// TextRenderingHint
        /// </summary>
        private System.Drawing.Text.TextRenderingHint _TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		/// <summary>
		/// RotateTransform Angle
		/// </summary>
		private float _RotateAngle = 0.0F;

		/// <summary>
		/// TranslateTransform X
		/// </summary>
		private float _TranslateX = 0.0F;
		/// <summary>
		/// TranslateTransform Y
		/// </summary>
		private float _TranslateY = 0.0F;

        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// 
        /// </summary>
        public AntiAliasLabel()
        {
            InitializeComponent();
        }

        #endregion

        #region Event

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.base.OnPaint(e);
            // Call methods of the System.Drawing.Graphics object.
            e.Graphics.TextRenderingHint = _TextRenderingHint;

			if (_RotateAngle != 0.0F)
			{
				// ワールド座標を移動
				e.Graphics.TranslateTransform(_TranslateX, _TranslateY);

				// RotateTransform
				e.Graphics.RotateTransform(_RotateAngle);

				e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 0, 0);
			}
			else
			{
				// Non Rotate
				base.OnPaint(e);
			}
        }

        #endregion

        #region Property

        //
        // 概要:
        //     この System.Drawing.Graphics に関連付けられているテキストのレンダリング モードを取得または設定します。
        //
        // 戻り値:
        //     System.Drawing.Text.TextRenderingHint 値の 1 つ。
		[Category("CustomControl AntiAlias")]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return _TextRenderingHint;
            }
            set
            {
                _TextRenderingHint = value;
            }
        }

		/// <summary>
		/// RotateAngle
		/// </summary>
		[Category("CustomControl AntiAlias")]
		public float RotateAngle
        {
            get
            {
				return _RotateAngle;
            }
            set
            {
				_RotateAngle = value;
            }
        }

		/// <summary>
		/// Translate X
		/// </summary>
		[Category("CustomControl AntiAlias")]
		public float TranslateX
		{
			get
			{
				return _TranslateX;
			}
			set
			{
				_TranslateX = value;
			}
		}

		/// <summary>
		/// Translate Y
		/// </summary>
		[Category("CustomControl AntiAlias")]
		public float TranslateY
		{
			get
			{
				return _TranslateY;
			}
			set
			{
				_TranslateY = value;
			}
		}
        #endregion
	}
}
