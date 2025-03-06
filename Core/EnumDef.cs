using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
	/// <summary>
	/// Gui Edit Mode
	/// </summary>
	public enum EditMode
	{
		Editable,
		ReadOnly,
	}

	public enum RowDispStat
	{
		Normal = 0,
		/// <summary>
		/// 非表示
		///  2012/07/10 未使用
		/// </summary>
		Hide = 1,

		/// <summary>
		/// カラー指定レンジ
		/// </summary>
		ColorRange = 0xf0,
		/// <summary>
		/// Red
		/// </summary>
		Red = 0x10,
		/// <summary>
		/// LightBlue
		/// </summary>
		LightBlue = 0x20,
		Blue = 0x30,
		LightYellow = 0x40,
		Yellow = 0x50,
	}

    /// <summary>
    /// 見積り練習モード
    /// </summary>
    public enum EstimationMode
    {
        /// <summary>
        /// 見積り問題無し
        /// </summary>
        Clean_Estimation = 0,
        /// <summary>
        /// 見積り問題読込
        /// </summary>
        Load_Estimation = 1,
        /// <summary>
        /// 見積り練習開始
        /// </summary>
        Start_Answer = 2,
        /// <summary>
        /// 見積り練習中断
        /// </summary>
        Pause_Answer = 3,
        /// <summary>
        /// 見積り練習完了
        /// </summary>
        Finish_Answer = 4,
        /// <summary>
        /// 見積り練習採点済み
        /// </summary>
        Grading_Answer = 5,
    }
}
