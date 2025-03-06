using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstDef
    {
        public const String C_SystemVersion_Full = "1.0.0";
        public const String C_SystemVersion_Sub = "1.0";
        public const String C_SystemCompanySubFolder = "Toyota";
		public const String C_ApplicationName = "EstimationExamination";
		//public static String[] C_MoveSupportSystemVersion_Sub = {"1.0"}; //データベースの移行を保障するバージョン一覧


		public const String C_PARTS_ID_FORMAT = "{0:0000}";
		public const int C_PARTS_ID_INNER_LENGTH = 4;
		public const int C_WORK_CD_INNER_LENGTH = 10;

		///// <summary>
		///// Auda Link File Path
		///// </summary>
		//public const String C_AudaLinkFilePath = @"C:\EstimatedDB";

		///// <summary>
		///// システム更新要求月数 0:無し
		///// </summary>
		////public const int C_SystemUpdateRequest_Month = 6;
		//public const int C_SystemUpdateRequest_Month = 0;

		//public const int C_Trial_Limit_Count = 10;
	}
}
