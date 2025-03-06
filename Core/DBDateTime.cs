using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
	/// <summary>
	/// DBDateTime
	/// </summary>
	[Serializable]
	public class DBDateTime
	{
		#region Private Fields

		/// <summary>
		/// DateTime
		/// </summary>
		private DateTime _Date;

		#endregion

		#region Constructors and Destructor

		/// <summary>
		/// Constructors
		/// </summary>
		public DBDateTime()
		{
			_Date = DateTime.MinValue;
		}

		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="date"></param>
		public DBDateTime(DateTime date)
		{
			_Date = date;
		}

		#endregion

		#region Property

		/// <summary>
		/// 代入時 DateTime --> DBDateTime
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static implicit operator DBDateTime(DateTime date)
		{
			return new DBDateTime(date);
		}

		/// <summary>
		/// 代入時 DBDateTime --> DateTime
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static implicit operator DateTime(DBDateTime date)
		{
			if (date == null)
				throw new Exception("DateTime is null value.");

			return date.Value;
		}

		/// <summary>
		/// 日付を返す
		/// </summary>
		public DateTime Value
		{
			get { return _Date; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _Date.ToString();
		}

		#endregion
	}
}
